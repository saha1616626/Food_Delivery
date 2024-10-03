using Food_Delivery.Data;
using Food_Delivery.Helper;
using Food_Delivery.Model;
using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Food_Delivery.ViewModel.Client
{
    public class ClientPersonalDataViewModel : INotifyPropertyChanged
    {
        AuthorizationViewModel authorizationViewModel = new AuthorizationViewModel();
        public ClientPersonalDataViewModel()
        {
            // начальная настройка страницы
            InitialPageSetup();
        }

        // начальная настройка страницы
        private async Task InitialPageSetup()
        {
            // получаем id пользователя
            int userId = await authorizationViewModel.WeGetIdUser();
            if (userId != 0)
            {
                using (FoodDeliveryContext foodDeliveryContext = new FoodDeliveryContext())
                {
                    // сохраняем данные в БД
                    Account account = await foodDeliveryContext.Accounts.FirstOrDefaultAsync(a => a.id == userId);
                    if (account != null)
                    {
                        OutClientCity = account.city;
                        OutClientStreet = account.street;
                        OutClientHouse = account.house;
                        if (account.apartment != null)
                        {
                            OutClientApartment = account.apartment;
                        }
                        OutClientName = account.name;
                        OutClientSurname = account.surname;
                        if(account.patronymic != null)
                        {
                            OutClientPatronymic = account.patronymic;
                        }
                        OutClientNumberPhone = account.numberPhone;
                        if (account.email != null)
                        {
                            OutClientPatronymic = account.email;
                        }

                    }
                }
            }
        }

        // работа над изменениями данных
        #region WorkingWithData

        // изменение информации о пользователе
        private RelayCommand _addUserInfoData { get; set; }
        public RelayCommand AddUserInfoData
        {
            get
            {
                return _addUserInfoData ??
                    (_addUserInfoData = new RelayCommand(async (obj) =>
                    {
                        /// !string.IsNullOrWhiteSpace(OutClientName) && !string.IsNullOrWhiteSpace(OutClientSurname)
                        /// && !string.IsNullOrWhiteSpace(OutClientNumberPhone)

                        // проверка наличия обязательных данных
                        if (true)
                        {
                            // проверяем корректность введенных данных
                            bool isCheckingNumbers = false; // переменная корректности введённого номера телефона
                            bool isCheckingEmail = false; // переменная корректности введённого Email

                            ErrorInput = ""; // очищаем поле ошибки

                            // проверяем цело число в поле "Номер телефона"
                            isCheckingNumbers = (double.TryParse(OutClientNumberPhone.Trim(), out double number));
                            if (!isCheckingNumbers) // если все числа корректны
                            {
                                StartFieldIllumination(AnimationOutNumberPhone); // подсветка поля
                                ErrorInput += "\nФормат номера телефона нарушен! Только цифры!"; // сообщение с ошибкой
                            }
                            else
                            {
                                if (OutClientNumberPhone.Trim().Length == 11) // проверяем на кол-во цифр в номере телефона
                                {
                                    if (OutClientNumberPhone.Trim().StartsWith("7")) // проверка на 7 в начале
                                    {

                                    }
                                    else
                                    {
                                        isCheckingNumbers = false;
                                        StartFieldIllumination(AnimationOutNumberPhone); // подсветка поля
                                        ErrorInput += "\nНомер телефона должен начинаться с \"7\"!"; // сообщение с ошибкой
                                    }
                                }
                                else
                                {
                                    isCheckingNumbers = false;
                                    StartFieldIllumination(AnimationOutNumberPhone); // подсветка поля
                                    ErrorInput += "\nФормат номера телефона нарушен! 11 цифр!"; // сообщение с ошибкой
                                }
                            }

                            // проверяем Email, если введен
                            if (OutClientEmail != null && OutClientEmail.Trim() != "")
                            {
                                if (!OutClientEmail.Contains("@"))
                                {
                                    StartFieldIllumination(AnimationOutEmail); // подсветка поля
                                    ErrorInput += "\nФормат Email нарушен! Нет знака \"@\"!"; // сообщение с ошибкой
                                }
                            }
                            else
                            {
                                isCheckingEmail = true;
                            }

                            BeginFadeAnimation(AnimationErrorInput); // затухание сообщения об ошибке

                            // проверка формата данных
                            if (isCheckingNumbers && isCheckingEmail)
                            {
                                using (FoodDeliveryContext foodDeliveryContext = new FoodDeliveryContext())
                                {
                                    // получаем id пользователя
                                    int userId = await authorizationViewModel.WeGetIdUser();
                                    if (userId != 0)
                                    {
                                        // сохраняем данные в БД
                                        Account account = await foodDeliveryContext.Accounts.FirstOrDefaultAsync(a => a.id == userId);
                                        if (account != null)
                                        {
                                            account.name = OutClientName.Trim();
                                            account.surname = OutClientSurname.Trim();
                                            if (!string.IsNullOrWhiteSpace(OutClientPatronymic))
                                            {
                                                account.patronymic = OutClientPatronymic.Trim();
                                            }
                                            else
                                            {
                                                account.patronymic = "";
                                            }
                                            account.numberPhone = OutClientNumberPhone.Trim();
                                            if (!string.IsNullOrWhiteSpace(OutClientEmail))
                                            {
                                                account.email = OutClientEmail.Trim();
                                            }
                                            else
                                            {
                                                account.email = "";
                                            }

                                            ErrorInput = "       Личные данные сохранены!";
                                            BeginFadeAnimation(AnimationErrorInput); // затухание сообщения об ошибке
                                            // обновляем данные в БД
                                            await foodDeliveryContext.SaveChangesAsync();
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            ErrorInput = ""; // очищаем сообщение об ошибке

                            if (string.IsNullOrWhiteSpace(OutClientName))
                            {
                                StartFieldIllumination(AnimationOutName); // подсветка поля
                                ErrorInput = "Заполните обязательные поля!"; // сообщение с ошибкой
                            }

                            if (string.IsNullOrWhiteSpace(OutClientSurname))
                            {
                                StartFieldIllumination(AnimationOutSurname); // подсветка поля
                                ErrorInput = "Заполните обязательные поля!"; // сообщение с ошибкой
                            }

                            if (string.IsNullOrWhiteSpace(OutClientNumberPhone))
                            {
                                StartFieldIllumination(AnimationOutNumberPhone); // подсветка поля
                                ErrorInput = "Заполните обязательные поля!"; // сообщение с ошибкой
                            }


                        }
                    }, (obj) => true));
            }
        }

        // изменение информации об адресе
        private RelayCommand _addAddressData { get; set; }
        public RelayCommand AddAddressData
        {
            get
            {
                return _addAddressData ??
                    (_addAddressData = new RelayCommand(async (obj) =>
                    {
                        // проверка наличия обязательных данных
                        if (!string.IsNullOrWhiteSpace(OutClientCity) && !string.IsNullOrWhiteSpace(OutClientStreet) &&
                       !string.IsNullOrWhiteSpace(OutClientHouse))
                        {
                            bool isCheckingHouse = false; // переменная корректности введённого дома
                            bool isCheckingApartment = false; // переменная корректности введённой квартиры

                            ErrorInput = ""; // очищаем сообщение об ошибке

                            // проверяем целое число в поле "Дом"
                            isCheckingHouse = int.TryParse(OutClientHouse.Trim(), out int house);
                            if (!isCheckingHouse) // число не получено
                            {
                                StartFieldIllumination(AnimationOutHouse); // подсветка поля
                                ErrorInput = "Введите целое число!"; // сообщение с обибкой
                            }

                            // проверяем цело число в поле "Квартира"
                            if (OutClientApartment != null && OutClientApartment.Trim() != "")
                            {
                                isCheckingApartment = int.TryParse(OutClientApartment.Trim(), out int apartament);
                                if (!isCheckingApartment) // число не получено
                                {
                                    StartFieldIllumination(AnimationOutApartment); // подсветка поля
                                    ErrorInput = "Введите целое число!"; // сообщение с обибкой
                                }
                            }
                            else
                            {
                                isCheckingApartment = true; // если квартира не выбрана, то проверка пройдена, так как атрибут не обязатльный
                            }

                            BeginFadeAnimation(AnimationErrorInput); // затухание сообщения об ошибке

                            // проверка формата данных
                            if (isCheckingApartment && isCheckingHouse)
                            {
                                using (FoodDeliveryContext foodDeliveryContext = new FoodDeliveryContext())
                                {
                                    // получаем id пользователя
                                    int userId = await authorizationViewModel.WeGetIdUser();
                                    if (userId != 0)
                                    {
                                        // сохраняем данные в БД
                                        Account account = await foodDeliveryContext.Accounts.FirstOrDefaultAsync(a => a.id == userId);
                                        if (account != null)
                                        {
                                            account.city = OutClientCity.Trim();
                                            account.street = OutClientStreet.Trim();
                                            account.house = OutClientHouse.Trim();
                                            if (!string.IsNullOrWhiteSpace(OutClientApartment))
                                            {
                                                account.apartment = OutClientApartment.Trim();
                                            }
                                            else
                                            {
                                                account.apartment = "";
                                            }
                                            ErrorInput = "              Адрес сохранён!";
                                            BeginFadeAnimation(AnimationErrorInput); // затухание сообщения об ошибке
                                            // обновляем данные в БД
                                            await foodDeliveryContext.SaveChangesAsync();
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            ErrorInput = ""; // очищаем сообщение об ошибке

                            if (string.IsNullOrWhiteSpace(OutClientCity))
                            {
                                StartFieldIllumination(AnimationOutCity); // подсветка поля
                                ErrorInput = "Заполните обязательные поля!"; // сообщение с ошибкой
                            }

                            if (string.IsNullOrWhiteSpace(OutClientStreet))
                            {
                                StartFieldIllumination(AnimationOutStreet); // подсветка поля
                                ErrorInput = "Заполните обязательные поля!"; // сообщение с ошибкой
                            }

                            if (string.IsNullOrWhiteSpace(OutClientHouse))
                            {
                                StartFieldIllumination(AnimationOutHouse); // подсветка поля
                                ErrorInput = "Заполните обязательные поля!"; // сообщение с ошибкой
                            }

                            BeginFadeAnimation(AnimationErrorInput); // затухание сообщения об ошибке
                        }
                    }, (obj) => true));
            }
        }

        #endregion

        // свойства 
        #region Features

        public TextBox AnimationOutName { get; set; } // поле для ввода текста "имя клиента". Вывод подсветки поля
        public TextBox AnimationOutSurname { get; set; } // поле для ввода текста "фамилия клиента". Вывод подсветки поля
        public TextBox AnimationOutPatronymic { get; set; } // поле для ввода текста "отчество клиента". Вывод подсветки поля
        public TextBox AnimationOutCity { get; set; } // поле для ввода текста "город клиента". Вывод подсветки поля
        public TextBox AnimationOutStreet { get; set; } // поле для ввода текста "улица клиента". Вывод подсветки поля
        public TextBox AnimationOutHouse { get; set; } // поле для ввода текста "дом клиента". Вывод подсветки поля
        public TextBox AnimationOutApartment { get; set; } // поле для ввода текста "квартира клиента". Вывод подсветки поля
        public TextBox AnimationOutNumberPhone { get; set; } // поле для ввода текста "номер телефона клиента". Вывод подсветки поля
        public TextBox AnimationOutEmail { get; set; } // поле для ввода текста "email клиента". Вывод подсветки поля
        public TextBlock AnimationErrorInput { get; set; } // объект текстового поля. Анимация затухания текста после вывода сообщения.
        public Storyboard FieldIllumination { get; set; } // анимация объектов

        public async Task InitializeAsync(Storyboard FieldIllumination, TextBox AnimationOutName,
            TextBox AnimationOutSurname, TextBox AnimationOutPatronymic, TextBox AnimationOutCity, TextBox AnimationOutStreet,
            TextBox AnimationOuttHouse, TextBox AnimationOutApartment, TextBox AnimationOutNumberPhone, TextBox AnimationOutEmail,
            TextBlock AnimationErrorInput)
        {
            if (FieldIllumination != null)
            {
                this.FieldIllumination = FieldIllumination;
            }
            if (AnimationOutName != null)
            {
                this.AnimationOutName = AnimationOutName;
            }
            if (AnimationOutSurname != null)
            {
                this.AnimationOutSurname = AnimationOutSurname;
            }
            if (AnimationOutPatronymic != null)
            {
                this.AnimationOutPatronymic = AnimationOutPatronymic;
            }
            if (AnimationOutCity != null)
            {
                this.AnimationOutCity = AnimationOutCity;
            }
            if (AnimationOutStreet != null)
            {
                this.AnimationOutStreet = AnimationOutStreet;
            }
            if (AnimationOuttHouse != null)
            {
                this.AnimationOutHouse = AnimationOuttHouse;
            }
            if (AnimationOutApartment != null)
            {
                this.AnimationOutApartment = AnimationOutApartment;
            }
            if (AnimationOutNumberPhone != null)
            {
                this.AnimationOutNumberPhone = AnimationOutNumberPhone;
            }
            if (AnimationOutEmail != null)
            {
                this.AnimationOutEmail = AnimationOutEmail;
            }
            if (AnimationErrorInput != null)
            {
                this.AnimationErrorInput = AnimationErrorInput;
            }
        }

        // свойство для вывода текстовой ошибки при добавлении или редактировании данных
        private string _errorInput { get; set; }
        public string ErrorInput
        {
            get { return _errorInput; }
            set { _errorInput = value; OnPropertyChanged(nameof(ErrorInput)); }
        }

        // email клиента
        private string _outClientEmail { get; set; }
        public string OutClientEmail
        {
            get { return _outClientEmail; }
            set { _outClientEmail = value; OnPropertyChanged(nameof(OutClientEmail)); }
        }

        // номер телефона клиента
        private string _outClientNumberPhone { get; set; }
        public string OutClientNumberPhone
        {
            get { return _outClientNumberPhone; }
            set { _outClientNumberPhone = value; OnPropertyChanged(nameof(OutClientNumberPhone)); }
        }

        // квартира клиента
        private string _outClientApartment { get; set; }
        public string OutClientApartment
        {
            get { return _outClientApartment; }
            set { _outClientApartment = value; OnPropertyChanged(nameof(OutClientApartment)); }
        }

        // дом клиента
        private string _outClientHouse { get; set; }
        public string OutClientHouse
        {
            get { return _outClientHouse; }
            set { _outClientHouse = value; OnPropertyChanged(nameof(OutClientHouse)); }
        }

        // улица клиента
        private string _outClientStreet { get; set; }
        public string OutClientStreet
        {
            get { return _outClientStreet; }
            set { _outClientStreet = value; OnPropertyChanged(nameof(OutClientStreet)); }
        }

        // город клиента
        private string _outClientCity { get; set; }
        public string OutClientCity
        {
            get { return _outClientCity; }
            set { _outClientCity = value; OnPropertyChanged(nameof(OutClientCity)); }
        }

        // отчество клиента
        private string _outClientPatronymic { get; set; }
        public string OutClientPatronymic
        {
            get { return _outClientPatronymic; }
            set { _outClientPatronymic = value; OnPropertyChanged(nameof(OutClientPatronymic)); }
        }

        // фамилия клиента
        private string _outClientSurname { get; set; }
        public string OutClientSurname
        {
            get { return _outClientSurname; }
            set { _outClientSurname = value; OnPropertyChanged(nameof(OutClientSurname)); }
        }

        // имя клиента
        private string _outClientName { get; set; }
        public string OutClientName
        {
            get { return _outClientName; }
            set { _outClientName = value; OnPropertyChanged(nameof(OutClientName)); }
        }

        #endregion

        // анимации
        #region Animation

        // анимация затухания текста
        private async Task BeginFadeAnimation(TextBlock textBlock)
        {
            textBlock.IsEnabled = true;
            textBlock.Opacity = 1.0;

            Storyboard storyboard = new Storyboard();
            DoubleAnimation fadeAnimation = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                Duration = TimeSpan.FromSeconds(1)
            };
            Storyboard.SetTargetProperty(fadeAnimation, new PropertyPath(TextBlock.OpacityProperty));
            storyboard.Children.Add(fadeAnimation);
            storyboard.Completed += (s, e) => textBlock.IsEnabled = false;
            storyboard.Begin(textBlock);
        }

        // запуск анимации подсвечивания объекта
        private void StartFieldIllumination(TextBox textBox)
        {
            FieldIllumination.Begin(textBox);
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
