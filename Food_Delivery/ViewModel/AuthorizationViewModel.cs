using Food_Delivery.Data;
using Food_Delivery.Helper;
using Food_Delivery.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client.NativeInterop;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;


namespace Food_Delivery.ViewModel
{
    public class AuthorizationViewModel : INotifyPropertyChanged
    {
        // путь к json файлу авторизации
        readonly string path = @"E:\\3comm\\Documents\\Предметы\\Курс 3.2\\Курсовая\\Приложение\\Программа\\Food_Delivery\\Food_Delivery\\Data\\Authorization\\AuthorizationStatus.json";
        public AuthorizationViewModel()
        {

        }

        // метод выхода в аккаунт
        public void LogOutYourAccount()
        {
            // передаём в JSON состояние, что мы вышли из аккаунта
            AuthorizationEntrance authorizationEntrance = new AuthorizationEntrance(); // класс авторизации
            authorizationEntrance.Entrance = false; // пользователь вышел из аккаунта
            authorizationEntrance.UserId = null;
            authorizationEntrance.UserRole = "";

            try
            {
                var jsonAuthorization = JsonConvert.SerializeObject(authorizationEntrance); //  перзапись данных в формате json
                                                                                            // записываем обновленные данные в JSON
                File.WriteAllText(path, jsonAuthorization);

                // событие перехода на главную страницу и вызов метода перехода на страницу авторизации
                WorkingWithData.ExitPageFromAccount();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка записи в json файла /n" + ex.Message);
            }
        }

        // подготовка страницы
        #region PreparingPage

        public bool IsAuthorization {  get; set; } // состояние авторизации = true; состояние регистарции = false;

        // начальное состояние страницы
        public async Task InitialPageSetup()
        {
            IsAuthorization = true; // режим авторизации

            // изменяем цвета кнопки
            btnAuthorization.Background = Brushes.Green;
        }

        // кнопка авторизации
        private RelayCommand _btn_Authorization { get; set; }
        public RelayCommand Btn_Authorization
        {
            get
            {
                return _btn_Authorization ??
                    (_btn_Authorization = new RelayCommand(async (obj) =>
                    {
                        IsAuthorization = true; // режим авторизации
                                                // изменяем цвета кнопки
                        btnAuthorization.Background = Brushes.Green;
                        btnRegistration.Background = Brushes.Gray;

                    }, (obj) => true));
            }
        }

        // кнопка регистрации
        private RelayCommand _btn_Registration { get; set; }
        public RelayCommand Btn_Registration
        {
            get
            {
                return _btn_Registration ??
                    (_btn_Registration = new RelayCommand(async (obj) =>
                    {
                        IsAuthorization = false; // режим регистрации
                        btnRegistration.Background = Brushes.Green;
                        btnAuthorization.Background = Brushes.Gray;

                    }, (obj) => true));
            }
        }


        // метод входа в аккаунт
        private RelayCommand _entrance { get; set; }
        public RelayCommand Entrance
        {
            get
            {
                return _entrance ??
                    (_entrance = new RelayCommand(async (obj) =>
                    {
                        AuthorizationError = "";

                        // проверка наличия обязательных данных
                        if (!String.IsNullOrWhiteSpace(AnimationOutLogin.Text) &&
                        !String.IsNullOrWhiteSpace(AnimationOutPassword.Password.ToString()))
                        {
                            using (FoodDeliveryContext foodDeliveryContext = new FoodDeliveryContext())
                            {
                                List<Model.Account> accounts = await foodDeliveryContext.Accounts.ToListAsync();
                                List<Role> roles = await foodDeliveryContext.Roles.ToListAsync();

                                string login = AnimationOutLogin.Text.ToString().Trim();
                                string password = AnimationOutPassword.Password.ToString().Trim();

                                if (IsAuthorization) // авторизация
                                {
                                    // проверяем введенный логин
                                    Model.Account account = await Task.Run(() => accounts.FirstOrDefault(a => a.login == login));
                                    if (account != null)
                                    {
                                        // получаем роль пользователя
                                        Role role = await Task.Run(() => roles.FirstOrDefault(r => r.id == account.roleId));

                                        // проверяем пароль
                                        // шифруем введенный пользователем старыйы пароль и проверяем на совпадение
                                        if (PasswordHasher.VerifyPassword(password, account.password))
                                        {
                                            // передаём в JSON состояние, что мы вошли в аккаунт
                                            AuthorizationEntrance authorizationEntrance = new AuthorizationEntrance(); // класс авторизации
                                            authorizationEntrance.Entrance = true; // пользователь вошёл в аккаунт
                                            authorizationEntrance.UserId = account.id;
                                            if(role != null)
                                            {
                                                authorizationEntrance.UserRole = role.name;
                                            }

                                            try
                                            {
                                                var jsonAuthorization = JsonConvert.SerializeObject(authorizationEntrance); //  перзапись данных в формате json
                                                                                                                            // записываем обновленные данные в JSON
                                                File.WriteAllText(path, jsonAuthorization);

                                                // успешная авторизация в аккаунт
                                                WorkingWithData.SuccessfulLoginAccount();
                                            }
                                            catch (Exception ex)
                                            {
                                                MessageBox.Show("Ошибка записи в json файла /n" + ex.Message);
                                            }
                                        }
                                        else
                                        {
                                            StartFieldIllumination(AnimationOutPassword); // подсветка поля
                                            AuthorizationError = "Пароль не верный!"; // сообщение с ошибкой
                                        }
                                    }
                                    else
                                    {
                                        StartFieldIllumination(AnimationOutLogin); // подсветка поля
                                        AuthorizationError = "Пользователь с данным \nлогином отсутствует!"; // сообщение с ошибкой
                                    }
                                }
                                else // регистрация
                                {
                                    // проверка логина
                                    if (Regex.IsMatch(login, @"^[a-zA-Z0-9]{5,}$"))
                                    {
                                        // проверка правилам введенного нового пароля
                                        if (Regex.IsMatch(password, @"^[a-zA-Z0-9]{8,}$"))
                                        {
                                            // сохраняем данные в БД и осуществляем авторизацию пользователя
                                            Model.Account account = new Model.Account();
                                            account.registrationDate = DateTime.Now;
                                            account.roleId = 3;
                                            account.login = login;
                                            account.password = PasswordHasher.HashPassword(password);
                                            await foodDeliveryContext.Accounts.AddAsync(account); // добавляем данные в список БД
                                            await foodDeliveryContext.SaveChangesAsync(); // cохраняем изменения в базе данных

                                            // передаём в JSON состояние, что мы вошли в аккаунт
                                            AuthorizationEntrance authorizationEntrance = new AuthorizationEntrance(); // класс авторизации
                                            authorizationEntrance.Entrance = true; // пользователь вошёл в аккаунт
                                            authorizationEntrance.UserId = account.id;

                                            // получаем роль "пользователь"
                                            Role role = await Task.Run(() => roles.FirstOrDefault(r => r.name == "Пользователь"));

                                            if (role != null)
                                            {
                                                authorizationEntrance.UserRole = role.name;
                                            }

                                            try
                                            {
                                                var jsonAuthorization = JsonConvert.SerializeObject(authorizationEntrance); //  перзапись данных в формате json
                                                                                                                            // записываем обновленные данные в JSON
                                                File.WriteAllText(path, jsonAuthorization);

                                                // успешная авторизация в аккаунт
                                                WorkingWithData.SuccessfulLoginAccount();
                                            }
                                            catch (Exception ex)
                                            {
                                                MessageBox.Show("Ошибка записи в json файла /n" + ex.Message);
                                            }
                                        }
                                        else
                                        {
                                            StartFieldIllumination(AnimationOutPassword); // подсветка поля
                                            AuthorizationError = "Пароль должен содержать:\r"; // сообщение с ошибкой

                                            if (AnimationOutPassword.Password.Trim().Count() < 8)
                                            {
                                                AuthorizationError += "- Не менее 8 символов!\r";
                                            }

                                            if (!Regex.IsMatch(AnimationOutPassword.Password.Trim(), @"^[a-zA-Z0-9]$"))
                                            {
                                                AuthorizationError += "- Только латинские буквы и арабские цифры!\r";
                                            }

                                            BeginFadeAnimation(AnimationErrorInput); // исчезание инфорамации об ошибке
                                        }
                                    }
                                    else
                                    {
                                        StartFieldIllumination(AnimationOutLogin); // подсветка поля
                                        AuthorizationError = "Логин должен содержать:\r"; // сообщение с ошибкой

                                        if (AnimationOutLogin.Text.Trim().Count() < 5)
                                        {
                                            AuthorizationError += "- Не менее 5 символов!\r";
                                        }

                                        if (!Regex.IsMatch(AnimationOutLogin.Text.Trim(), @"^[a-zA-Z0-9]$"))
                                        {
                                            AuthorizationError += "- Только латинские буквы и арабские цифры!\r";
                                        }

                                        BeginFadeAnimation(AnimationErrorInput); // исчезание инфорамации об ошибке
                                    }
                                }

                                BeginFadeAnimation(AnimationErrorInput); // затухание сообщения об ошибке
                            }
                        }
                        else
                        {
                            AuthorizationError = ""; // очищаем сообщение об ошибке

                            if (String.IsNullOrWhiteSpace(AnimationOutLogin.Text))
                            {
                                StartFieldIllumination(AnimationOutLogin); // подсветка поля
                                AuthorizationError += "\nВведите логин!"; // сообщение с ошибкой
                            }
                            if (String.IsNullOrWhiteSpace(AnimationOutPassword.Password))
                            {
                                StartFieldIllumination(AnimationOutPassword); // подсветка поля
                                AuthorizationError += "\nВведите пароль!"; // сообщение с ошибкой
                            }

                            BeginFadeAnimation(AnimationErrorInput); // затухание сообщения об ошибке
                        }
                    }, (obj) => true));
            }
        }

        //гостевой вход в аккаунт
        private RelayCommand _entranceGuest { get; set; }
        public RelayCommand EntranceGuest
        {
            get
            {
                return _entranceGuest ??
                    (_entranceGuest = new RelayCommand(async (obj) =>
                    {


                        // передаём в JSON состояние, что мы вошли в аккаунт как гость
                        AuthorizationEntrance authorizationEntrance = new AuthorizationEntrance(); // класс авторизации
                        authorizationEntrance.Entrance = true; // пользователь вошёл в аккаунт
                        authorizationEntrance.UserId = null;
                        authorizationEntrance.UserRole = "Гость";

                        try
                        {
                            var jsonAuthorization = JsonConvert.SerializeObject(authorizationEntrance); //  перзапись данных в формате json
                                                                                                        // записываем обновленные данные в JSON
                            File.WriteAllText(path, jsonAuthorization);

                            // успешная авторизация в аккаунт
                            WorkingWithData.SuccessfulLoginAccount();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Ошибка записи в json файла /n" + ex.Message);
                        }

                    }, (obj) => true));
            }
        }

        // проверяем, авторизовался пользователь или нет
        public bool IsCheckAccountUser()
        {
            bool IsCheck = false; // по умолчанию не авторизовался

            // чтение JSON
            string JSON = File.ReadAllText(path);
            // получаем данные из JSON
            AuthorizationEntrance? account = JsonConvert.DeserializeObject<AuthorizationEntrance>(JSON);

            if (account.Entrance != null)
            {
                IsCheck = account.Entrance;
            }

            return IsCheck;
        }

        // получаем Id пользователя, который авторизован
        public async Task<int> WeGetIdUser()
        {
            int userId;

            // чтение JSON
            string JSON = File.ReadAllText(path);
            // получаем данные из JSON
            AuthorizationEntrance? account = JsonConvert.DeserializeObject<AuthorizationEntrance>(JSON);

            if (int.TryParse(account.UserId.ToString(), out int id))
            {
                userId = id;
            }
            else
            {
                userId = 0;
            }
            
            return userId;
        }

        // получаем Role пользователя, который авторизован
        public async Task<string> WeGetRoleUser()
        {
            string role = null;

            // чтение JSON
            string JSON = File.ReadAllText(path);
            // получаем данные из JSON
            AuthorizationEntrance? account = JsonConvert.DeserializeObject<AuthorizationEntrance>(JSON);

            if (account.UserRole != null)
            {
                role = account.UserRole;
            }

            return role;
        }

        #endregion

        // свойства 
        #region Features

        public TextBlock AnimationErrorInput { get; set; } //  анимация текста ошибки на странице

        public Button btnAuthorization { get; set; } // кнопка авторизвация
        public Button btnRegistration { get; set; } // кнопка регистарция
        public TextBox AnimationOutLogin { get; set; } // поле для ввода текста "логин". Вывод подсветки поля
        public PasswordBox AnimationOutPassword { get; set; } // поле для ввода текста "пароль". Вывод подсветки поля
        public Storyboard FieldIllumination { get; set; } // анимация объектов

        // получаем информацию об ошибках и состояниях входа
        public async Task InitializeAsync(Button btnAuthorization, Button btnRegistration, TextBox AnimationOutLogin,
            PasswordBox AnimationOutPassword, TextBlock AnimationErrorInput, Storyboard FieldIllumination)
        {
            if(btnAuthorization != null)
            {
                this.btnAuthorization = btnAuthorization;
            }
            if (btnRegistration != null)
            {
                this.btnRegistration = btnRegistration;
            }
            if(AnimationOutLogin != null)
            {
                this.AnimationOutLogin = AnimationOutLogin;
            }
            if (AnimationOutPassword != null)
            {
                this.AnimationOutPassword = AnimationOutPassword;
            }
            if (AnimationErrorInput != null)
            {
                this.AnimationErrorInput = AnimationErrorInput;
            }
            if (FieldIllumination != null)
            {
                this.FieldIllumination = FieldIllumination;
            }
        }

        // сообщение об ошибке
        private string _authorizationError { get; set; }
        public string AuthorizationError
        {
            get { return _authorizationError; }
            set { _authorizationError = value; OnPropertyChanged(nameof(AuthorizationError)); }
        }

        #endregion

        #region Animation

        // выводим сообщения об ошибке с анимацией затухания
        private async void BeginFadeAnimation(TextBlock textBlock)
        {
            textBlock.IsEnabled = true;
            textBlock.Opacity = 1.0;

            Storyboard storyboard = new Storyboard();
            DoubleAnimation fadeAnimation = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                Duration = TimeSpan.FromSeconds(2),
            };
            Storyboard.SetTargetProperty(fadeAnimation, new System.Windows.PropertyPath(TextBlock.OpacityProperty));
            storyboard.Children.Add(fadeAnimation);
            storyboard.Completed += (s, e) => textBlock.IsEnabled = false;
            storyboard.Begin(textBlock);
        }

        // запускаем анимации для TextBox (подсвечивание объекта)
        private void StartFieldIllumination(TextBox textBox)
        {
            FieldIllumination.Begin(textBox);
        }
        private void StartFieldIllumination(PasswordBox passwordBox)
        {
            FieldIllumination.Begin(passwordBox);
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
