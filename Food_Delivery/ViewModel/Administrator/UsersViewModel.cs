using Food_Delivery.Data;
using Food_Delivery.Helper;
using Food_Delivery.Model;
using Food_Delivery.Model.DPO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client.NativeInterop;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Animation;

namespace Food_Delivery.ViewModel.Administrator
{
    public class UsersViewModel : INotifyPropertyChanged
    {
        public UsersViewModel()
        {
            GetListAccounts(); // получаем список пользователей

            DarkBackground = Visibility.Collapsed; // фон для Popup скрыт

            // после получения фокуса данного приложения запукаем закрытый Popup
            WorkingWithData._launchPopupAfterReceivingFocusUsers += LaunchingPopupWhenGettingFocus;
        }

        // подготовка страницы
        #region PreparingPage

        // коллекция отображения данных в таблице
        private ObservableCollection<AccountDPO> _listAccounts { get; set; } = new ObservableCollection<AccountDPO>();
        public ObservableCollection<AccountDPO> ListAccounts
        {
            get { return _listAccounts; }
            set { _listAccounts = value; OnPropertyChanged(nameof(ListAccounts)); }
        }

        // отображаем список пользователей в таблице
        private async Task GetListAccounts()
        {
            ListAccounts.Clear(); // очищаем коллекцию перед заполнением

            using (FoodDeliveryContext foodDeliveryContext = new FoodDeliveryContext())
            {
                List<Model.Account> accounts = await foodDeliveryContext.Accounts.ToListAsync();

                // промежуточный список
                List<AccountDPO> accountDPOs = new List<AccountDPO>();

                foreach (var item in accounts)
                {
                    AccountDPO accountDPO = new AccountDPO();
                    accountDPOs.Add(await accountDPO.CopyFromAccount(item));
                }

                // фильтрация по статусу роли
                ListAccounts = new ObservableCollection<AccountDPO>
                     (await Task.Run(() => accountDPOs
                    .OrderByDescending(o => o.nameRole == "Администратор")
                    .ThenByDescending(o => o.nameRole == "Менеджер")
                    .ThenBy(o => o.nameRole == "Пользователь")
                    .ToList()));
            }
        }

        #endregion

        // работа над добавлением, редактированием и удалением пользователя
        #region WorkingWithData

        // путь к json работа окна Popup
        readonly string pathDataPopup = @"E:\3comm\Documents\Предметы\Курс 3.2\Курсовая\Приложение\Программа\Food_Delivery\Food_Delivery\Data\СheckPopup.json";

        // состояние: Popup добавление, редактирование или удаление.
        private bool IsCheckAddAndEditOrDelete; // true - добавление или редактирование данных.

        // свойство определющее назаначение запуска Popup (редактирование или добавление данных)
        private bool IsAddData { get; set; } // true - добавление данных; false - редактирование данных

        // запускаем Popup для добавления данных
        private RelayCommand _btn_OpenPopupToAddData { get; set; }
        public RelayCommand Btn_OpenPopupToAddData
        {
            get
            {
                return _btn_OpenPopupToAddData ??
                    (_btn_OpenPopupToAddData = new RelayCommand(async (obj) =>
                    {
                        IsAddData = true; // изменяем режим работы Popup на режим добавления данных
                        IsCheckAddAndEditOrDelete = true; // режим редактирования или добавления данных (удержания фокуса на Popup)
                        IsEditUser = false; // видимость кнопок для изменения пароля
                        StartPoupOfUsers = true; // отображаем Popup
                        DarkBackground = Visibility.Visible; // показать фон
                        WorkingWithData.ExitHamburgerMenu(); // закрываем, если открыто "гамбургер меню"
                        HeadingPopup = "Добавить пользователя"; // изменяем заголовок Popup
                        ActionConfirmationButton = "Добавить"; // изменение названия кнопки подтверждения действия 
                        // добавляем данные в ComboBox
                        using (FoodDeliveryContext foodDeliveryContext = new FoodDeliveryContext())
                        {
                            List<Role> roles = await foodDeliveryContext.Roles.ToListAsync();

                            // добавляем категории в список
                            OptionsRole = roles;
                        }

                        await ClearingPopup(); // очищаем поля

                        NotificationOfThePopupLaunchJson(); // оповещаем JSON, чтомы запустили Popup

                    }, (obj) => true));
            }
        }

        // запускаем Popup для редактирования данных
        private RelayCommand _btn_OpenPopupToEditData { get; set; }
        public RelayCommand Btn_OpenPopupToEditData
        {
            get
            {
                return _btn_OpenPopupToEditData ??
                    (_btn_OpenPopupToEditData = new RelayCommand(async (obj) =>
                    {
                        IsAddData = false; // изменяем режим работы Popup на режим редактирования данных
                        IsCheckAddAndEditOrDelete = true; // режим редактирования или добавления данных (удержания фокуса на Popup)
                        IsEditUser = true; // видимость кнопок для изменения пароля
                        StartPoupOfUsers = true; // отображаем Popup
                        DarkBackground = Visibility.Visible; // показать фон
                        WorkingWithData.ExitHamburgerMenu(); // закрываем, если открыто "гамбургер меню"
                        HeadingPopup = "Редактировать данные пользователя"; // изменяем заголовок Popup
                        ActionConfirmationButton = "Изменить"; // изменение названия кнопки подтверждения действия 
                        await ClearingPopup(); // очищаем поля

                        // добавляем данные в ComboBox
                        using (FoodDeliveryContext foodDeliveryContext = new FoodDeliveryContext())
                        {
                            OptionsRole = await foodDeliveryContext.Roles.ToListAsync();

                            Role role = await Task.Run(()=> OptionsRole.FirstOrDefault(r => r.id == SelectedAccount.roleId));
                            if (role != null)
                            {
                                SelectedRole = role;
                            }
                        }

                        // вносим данные из выбранного пользователя
                        if (SelectedAccount != null)
                        {
                            if (!string.IsNullOrWhiteSpace(SelectedAccount.name))
                            {
                                AnimationOutName.Text = SelectedAccount.name;
                            }
                            if (!string.IsNullOrWhiteSpace(SelectedAccount.surname))
                            {
                                AnimationOutSurname.Text = SelectedAccount.surname;
                            }
                            if (!string.IsNullOrWhiteSpace(SelectedAccount.patronymic))
                            {
                                AnimationOutPatronymic.Text = SelectedAccount.patronymic;
                            }
                            if (!string.IsNullOrWhiteSpace(SelectedAccount.numberphone))
                            {
                                AnimationOutNumberPhone.Text = SelectedAccount.numberphone;
                            }
                            if (!string.IsNullOrWhiteSpace(SelectedAccount.email))
                            {
                                AnimationOutEmail.Text = SelectedAccount.email;
                            }
                            if (!string.IsNullOrWhiteSpace(SelectedAccount.city))
                            {
                                AnimationOutCity.Text = SelectedAccount.city;
                            }
                            if (!string.IsNullOrWhiteSpace(SelectedAccount.street))
                            {
                                AnimationOutStreet.Text = SelectedAccount.street;
                            }
                            if (!string.IsNullOrWhiteSpace(SelectedAccount.house))
                            {
                                AnimationOutHouse.Text = SelectedAccount.house;
                            }
                            if (!string.IsNullOrWhiteSpace(SelectedAccount.apartament))
                            {
                                AnimationOutApartment.Text = SelectedAccount.apartament;
                            }

                            if (!string.IsNullOrWhiteSpace(SelectedAccount.login))
                            {
                                AnimationOutLogin.Text = SelectedAccount.login;
                            }
                        }
                        
                        NotificationOfThePopupLaunchJson(); // оповещаем JSON, чтомы запустили Popup

                    }, (obj) => true));
            }
        }

        // очищаем Popup после закрытия
        private async Task ClearingPopup()
        {
            AnimationOutName.Text = "";
            AnimationOutSurname.Text = "";
            AnimationOutPatronymic.Text = "";
            AnimationOutCity.Text = "";
            AnimationOutStreet.Text = "";
            AnimationOutHouse.Text = "";
            AnimationOutApartment.Text = "";
            AnimationOutNumberPhone.Text = "";
            AnimationOutEmail.Text = "";
            SelectedRole = null;
            AnimationOutLogin.Text = "";
            AnimationOutPassword.Password = "";
            AnimationOutNewPassword.Password = "";
        }

        // добавление или редактирование данных
        private RelayCommand _btn_SaveData { get; set; }
        public RelayCommand Btn_SaveData
        {
            get
            {
                return _btn_SaveData ??
                    (_btn_SaveData = new RelayCommand(async(obj) =>
                    {
                        // проверяем правльность ввода всех данных (форматы и тд)
                        bool isCheckingNumbers = false; // переменная корректности введённого номера телефона
                        bool isCheckingEmail = false; // переменная корректности введённого Email
                        bool isCheckingHouse = false; // переменная корректности введённого дома
                        bool isCheckingApartment = false; // переменная корректности введённой квартиры
                        bool isCheckingRole = false; // переменная выбора роли

                        ErrorInputPopup = ""; // очищаем поле ошибки

                        // проверяем дом, если введен
                        if (AnimationOutHouse != null && AnimationOutHouse.Text.Trim() != "")
                        {
                            // проверяем целое число в поле "Дом"
                            isCheckingHouse = int.TryParse(AnimationOutHouse.Text.Trim(), out int house);
                            if (!isCheckingHouse) // число не получено
                            {
                                StartFieldIllumination(AnimationOutHouse); // подсветка поля
                                ErrorInputPopup = "Введите целое число!"; // сообщение с обибкой
                            }
                        }
                        else
                        {
                            isCheckingHouse = true;
                        }
                          
                        // проверяем цело число в поле "Квартира"
                        if (AnimationOutApartment != null && AnimationOutApartment.Text.Trim() != "")
                        {
                            isCheckingApartment = int.TryParse(AnimationOutApartment.Text.Trim(), out int apartament);
                            if (!isCheckingApartment) // число не получено
                            {
                                StartFieldIllumination(AnimationOutApartment); // подсветка поля
                                ErrorInputPopup = "Введите целое число!"; // сообщение с обибкой
                            }
                        }
                        else
                        {
                            isCheckingApartment = true; // если квартира не выбрана, то проверка пройдена, так как атрибут не обязатльный
                        }

                        // проверяем цело число в поле "Номер телефона"
                        if (AnimationOutNumberPhone != null && AnimationOutNumberPhone.Text.Trim() != "")
                        {
                            isCheckingNumbers = (double.TryParse(AnimationOutNumberPhone.Text.Trim(), out double number));
                            if (!isCheckingNumbers) // если все числа корректны
                            {
                                StartFieldIllumination(AnimationOutNumberPhone); // подсветка поля
                                ErrorInputPopup += "\nФормат номера телефона нарушен! Только цифры!"; // сообщение с ошибкой
                            }
                            else
                            {
                                if (AnimationOutNumberPhone.Text.Trim().Length == 11) // проверяем на кол-во цифр в номере телефона
                                {
                                    if (AnimationOutNumberPhone.Text.Trim().StartsWith("7")) // проверка на 7 в начале
                                    {

                                    }
                                    else
                                    {
                                        isCheckingNumbers = false;
                                        StartFieldIllumination(AnimationOutNumberPhone); // подсветка поля
                                        ErrorInputPopup += "\nНомер телефона должен начинаться с \"7\"!"; // сообщение с ошибкой
                                    }
                                }
                                else
                                {
                                    isCheckingNumbers = false;
                                    StartFieldIllumination(AnimationOutNumberPhone); // подсветка поля
                                    ErrorInputPopup += "\nФормат номера телефона нарушен! 11 цифр!"; // сообщение с ошибкой
                                }
                            }
                        }
                        else
                        {
                            isCheckingNumbers = true;
                        }

                        // проверяем Email, если введен
                        if (AnimationOutEmail != null && AnimationOutEmail.Text.Trim() != "")
                        {
                            if (!AnimationOutEmail.Text.Contains("@"))
                            {
                                StartFieldIllumination(AnimationOutEmail); // подсветка поля
                                ErrorInputPopup += "\nФормат Email нарушен! Нет знака \"@\"!"; // сообщение с ошибкой
                            }
                            else
                            {
                                isCheckingEmail = true;
                            }
                        }
                        else
                        {
                            isCheckingEmail = true;
                        }

                        // проверяем роль на выбор
                        if(SelectedRole == null)
                        {
                            StartFieldIllumination(AnimationRole); // подсветка поля
                            ErrorInputPopup += "\nВыберите роль!"; // сообщение с ошибкой
                        }
                        else
                        {
                            isCheckingRole = true;
                        }

                        BeginFadeAnimation(AnimationErrorInputPopup); // затухание сообщения об ошибке

                        // проверка формата данных
                        if (isCheckingNumbers && isCheckingEmail && isCheckingHouse && isCheckingApartment && isCheckingRole)
                        {
                            // проверка на добавление или редактирование
                            if (IsAddData) // добавление данных
                            {
                                // проверка наличия обязательных данных
                                if (!String.IsNullOrWhiteSpace(AnimationOutLogin.Text) &&
                                !String.IsNullOrWhiteSpace(AnimationOutPassword.Password.ToString()))
                                {
                                    // проверяем уникальность логина пользователя
                                    using (FoodDeliveryContext foodDeliveryContext = new FoodDeliveryContext())
                                    {
                                        List<Model.Account> accounts = await foodDeliveryContext.Accounts.ToListAsync();

                                        // находим совпадающий логин в БД
                                        if (accounts.Any(a => a.login == AnimationOutLogin.Text.Trim()))
                                        {
                                            ErrorInputPopup = "\nЛогин существует!"; // сообщение с ошибкой
                                        }
                                        else
                                        {
                                            // логин уникальный, теперь проверяем пароль на корректность ввода
                                            // проверка пароля на соответсвие правилам
                                            if (Regex.IsMatch(AnimationOutPassword.Password.Trim(), @"^[a-zA-Z0-9]{8,}$"))
                                            {
                                                // пароль подходит. Добавляем данные в БД
                                                Model.Account account = new Model.Account();
                                                account.registrationDate = DateTime.Now;
                                                if (!string.IsNullOrWhiteSpace(AnimationOutName.Text))
                                                {
                                                    account.name = AnimationOutName.Text.Trim();
                                                }
                                                if (!string.IsNullOrWhiteSpace(AnimationOutSurname.Text))
                                                {
                                                    account.surname = AnimationOutSurname.Text.Trim();
                                                }
                                                if (!string.IsNullOrWhiteSpace(AnimationOutPatronymic.Text))
                                                {
                                                    account.patronymic = AnimationOutPatronymic.Text.Trim();
                                                }
                                                if (!string.IsNullOrWhiteSpace(AnimationOutNumberPhone.Text))
                                                {
                                                    account.numberPhone = AnimationOutNumberPhone.Text.Trim();
                                                }
                                                if (!string.IsNullOrWhiteSpace(AnimationOutEmail.Text))
                                                {
                                                    account.email = AnimationOutEmail.Text.Trim();
                                                }
                                                if (!string.IsNullOrWhiteSpace(AnimationOutCity.Text))
                                                {
                                                    account.city = AnimationOutCity.Text.Trim();
                                                }
                                                if (!string.IsNullOrWhiteSpace(AnimationOutStreet.Text))
                                                {
                                                    account.street = AnimationOutStreet.Text.Trim();
                                                }
                                                if (!string.IsNullOrWhiteSpace(AnimationOutHouse.Text))
                                                {
                                                    account.house = AnimationOutHouse.Text.Trim();
                                                }
                                                if (!string.IsNullOrWhiteSpace(AnimationOutApartment.Text))
                                                {
                                                    account.apartment = AnimationOutApartment.Text.Trim();
                                                }
                                                if (SelectedRole != null)
                                                {
                                                    account.roleId = SelectedRole.id;
                                                }
                                                if (!string.IsNullOrWhiteSpace(AnimationOutLogin.Text))
                                                {
                                                    account.login = AnimationOutLogin.Text.Trim();
                                                }

                                                // шифруем пароль
                                                account.password = PasswordHasher.HashPassword(AnimationOutPassword.Password.Trim());
                                                await foodDeliveryContext.Accounts.AddAsync(account); // добавляем данные в список БД
                                                await foodDeliveryContext.SaveChangesAsync(); // cохраняем изменения в базе данных

                                                ClosePopupWorkingWithData(); // закрываем Popup
                                                GetListAccounts(); // обновляем список
                                                ClearingPopup(); // очищаем поля
                                            }
                                            else
                                            {
                                                StartFieldIllumination(AnimationOutPassword); // подсветка поля
                                                ErrorInputPopup = "Пароль должен содержать:\r"; // сообщение с ошибкой

                                                if (AnimationOutPassword.Password.Trim().Count() < 8)
                                                {
                                                    ErrorInputPopup += "- Не менее 8 символов!\r";
                                                }

                                                if (!Regex.IsMatch(AnimationOutPassword.Password.Trim(), @"^[a-zA-Z0-9]$"))
                                                {
                                                    ErrorInputPopup += "- Только латинские буквы и арабские цифры!\r";
                                                }

                                                BeginFadeAnimation(AnimationErrorInputPopup); // исчезание инфорамации об ошибке
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    ErrorInputPopup = ""; // очищаем сообщение об ошибке

                                    if (String.IsNullOrWhiteSpace(AnimationOutLogin.Text))
                                    {
                                        StartFieldIllumination(AnimationOutLogin); // подсветка поля
                                        ErrorInputPopup += "\nВведите логин!"; // сообщение с ошибкой
                                    }
                                    if (String.IsNullOrWhiteSpace(AnimationOutPassword.Password))
                                    {
                                        StartFieldIllumination(AnimationOutPassword); // подсветка поля
                                        ErrorInputPopup += "\nВведите пароль!"; // сообщение с ошибкой
                                    }

                                    BeginFadeAnimation(AnimationErrorInputPopup); // затухание сообщения об ошибке
                                }
                            }
                            else // редактирование данных
                            {
                                using (FoodDeliveryContext foodDeliveryContext = new FoodDeliveryContext())
                                {
                                    List<Model.Account> accounts = await foodDeliveryContext.Accounts.ToListAsync();

                                    // получаем выбранный аккаунт из бд
                                    Model.Account selectesAccount = await Task.Run(()=> accounts.FirstOrDefault(a => a.id == SelectedAccount.id));
                                    if(selectesAccount != null)
                                    {
                                        if (!string.IsNullOrWhiteSpace(AnimationOutName.Text))
                                        {
                                            selectesAccount.name = AnimationOutName.Text.Trim();
                                        }
                                        else
                                        {
                                            selectesAccount.name = "";
                                        }
                                        if (!string.IsNullOrWhiteSpace(AnimationOutSurname.Text))
                                        {
                                            selectesAccount.surname = AnimationOutSurname.Text.Trim();
                                        }
                                        else
                                        {
                                            selectesAccount.surname = "";
                                        }
                                        if (!string.IsNullOrWhiteSpace(AnimationOutPatronymic.Text))
                                        {
                                            selectesAccount.patronymic = AnimationOutPatronymic.Text.Trim();
                                        }
                                        else
                                        {
                                            selectesAccount.patronymic = "";
                                        }
                                        if (!string.IsNullOrWhiteSpace(AnimationOutNumberPhone.Text))
                                        {
                                            selectesAccount.numberPhone = AnimationOutNumberPhone.Text.Trim();
                                        }
                                        if (!string.IsNullOrWhiteSpace(AnimationOutEmail.Text))
                                        {
                                            selectesAccount.email = AnimationOutEmail.Text.Trim();
                                        }
                                        else
                                        {
                                            selectesAccount.email = "";
                                        }
                                        if (!string.IsNullOrWhiteSpace(AnimationOutCity.Text))
                                        {
                                            selectesAccount.city = AnimationOutCity.Text.Trim();
                                        }
                                        else
                                        {
                                            selectesAccount.city = "";
                                        }
                                        if (!string.IsNullOrWhiteSpace(AnimationOutStreet.Text))
                                        {
                                            selectesAccount.street = AnimationOutStreet.Text.Trim();
                                        }
                                        else
                                        {
                                            selectesAccount.street = "";
                                        }
                                        if (!string.IsNullOrWhiteSpace(AnimationOutHouse.Text))
                                        {
                                            selectesAccount.house = AnimationOutHouse.Text.Trim();
                                        }
                                        else
                                        {
                                            selectesAccount.house = "";
                                        }
                                        if (!string.IsNullOrWhiteSpace(AnimationOutApartment.Text))
                                        {
                                            selectesAccount.apartment = AnimationOutApartment.Text.Trim();
                                        }
                                        else
                                        {
                                            selectesAccount.apartment = "";
                                        }
                                        if (SelectedRole != null)
                                        {
                                            selectesAccount.roleId = SelectedRole.id;
                                        }

                                        // сохраняем обновленные данные
                                        await foodDeliveryContext.SaveChangesAsync();

                                        ClosePopupWorkingWithData(); // закрываем Popup
                                        GetListAccounts(); // обновляем список
                                        ClearingPopup(); // очищаем поля   
                                    }
                                }
                            }

                            BeginFadeAnimation(AnimationErrorInputPopup); // затухание сообщения об ошибке
                        }
                        
                    }, (obj) => true));
            }
        }

        // изменение пароля
        private RelayCommand _btn_EditPassword { get; set; }
        public RelayCommand Btn_EditPassword
        {
            get
            {
                return _btn_EditPassword ??
                    (_btn_EditPassword = new RelayCommand(async (obj) =>
                    {
                        // проверка наличия обязательных данных
                        if (!String.IsNullOrWhiteSpace(AnimationOutPassword.Password.ToString()) &&
                        !String.IsNullOrWhiteSpace(AnimationOutNewPassword.Password.ToString()))
                        {
                            // проверка старого пароля и текущего пароля, который быд введе в поле "старый пароль"
                            using(FoodDeliveryContext foodDeliveryContext = new FoodDeliveryContext())
                            {
                                // получаем данные от текушего аккаунта
                                List<Model.Account> accounts = await foodDeliveryContext.Accounts.ToListAsync();
                                Model.Account account = await Task.Run(() => accounts.FirstOrDefault(a => a.id == SelectedAccount.id));  
                                if(account != null)
                                {

                                    // проверяем пароли
                                    // шифруем введенный пользователем старыйы пароль и проверяем на совпадение
                                    if (PasswordHasher.VerifyPassword(AnimationOutPassword.Password.ToString().Trim(), account.password))
                                    {
                                        // если пароль совпал, то делаем проверку на коррекность ввода нового пароля
                                        if (Regex.IsMatch(AnimationOutNewPassword.Password.Trim(), @"^[a-zA-Z0-9]{8,}$"))
                                        {
                                            account.password = PasswordHasher.HashPassword(AnimationOutNewPassword.Password.ToString().Trim());
                                            // сохраняем новый пароль
                                            await foodDeliveryContext.SaveChangesAsync();
                                            // оповещаем об успехе операции
                                            ErrorInputPopup = "Пароль успешно обновлен!\r";
                                            BeginFadeAnimation(AnimationErrorInputPopup);
                                            // сбрасываем поля
                                            AnimationOutPassword.Password = ""; AnimationOutNewPassword.Password = "";
                                        }
                                        else
                                        {
                                            StartFieldIllumination(AnimationOutNewPassword); // подсветка поля
                                            ErrorInputPopup = "Пароль должен содержать:\r"; // сообщение с ошибкой

                                            if (AnimationOutPassword.Password.Trim().Count() < 8)
                                            {
                                                ErrorInputPopup += "- Не менее 8 символов!\r";
                                            }

                                            if (!Regex.IsMatch(AnimationOutPassword.Password.Trim(), @"^[a-zA-Z0-9]$"))
                                            {
                                                ErrorInputPopup += "- Только латинские буквы и арабские цифры!\r";
                                            }

                                            BeginFadeAnimation(AnimationErrorInputPopup); // исчезание инфорамации об ошибке
                                            ErrorInputPopup = "";
                                        }
                                    }
                                    else
                                    {
                                        StartFieldIllumination(AnimationOutPassword); // подсветка поля
                                        ErrorInputPopup = "- Пароль не верный!\r";
                                        BeginFadeAnimation(AnimationErrorInputPopup); // исчезание инфорамации об ошибке
                                    }
                                }
                            }
                        }
                        else
                        {
                            ErrorInputPopup = ""; // очищаем сообщение об ошибке

                            if (String.IsNullOrWhiteSpace(AnimationOutPassword.Password))
                            {
                                StartFieldIllumination(AnimationOutPassword); // подсветка поля
                                ErrorInputPopup += "\nВведите старый пароль!"; // сообщение с ошибкой
                            }

                            if (String.IsNullOrWhiteSpace(AnimationOutNewPassword.Password))
                            {
                                StartFieldIllumination(AnimationOutNewPassword); // подсветка поля
                                ErrorInputPopup += "\nВведите новый пароль!"; // сообщение с ошибкой
                            }

                            BeginFadeAnimation(AnimationErrorInputPopup); // затухание сообщения об ошибке
                        }
                    }, (obj) => true));
            }
        }

        // запускаем Popup для удаления данных
        private RelayCommand _btn_OpenPopupToDeleteData { get; set; }
        public RelayCommand Btn_OpenPopupToDeleteData
        {
            get
            {
                return _btn_OpenPopupToDeleteData ??
                    (_btn_OpenPopupToDeleteData = new RelayCommand((obj) =>
                    {
                        StartPoupOfDelete = true; // отображаем Popup
                        IsCheckAddAndEditOrDelete = false; // режим редактирования или добавления данных (удержания фокуса на Popup)
                        DarkBackground = Visibility.Visible; // показать фон
                        WorkingWithData.ExitHamburgerMenu(); // закрываем, если открыто "гамбургер меню"
                        // отображаем логин пользователя перед удалением в Popup
                        if (!SelectedAccount.login.IsNullOrEmpty())
                        {
                            NameOfUserDeleted = SelectedAccount.login;
                        }

                        NotificationOfThePopupLaunchJson(); // оповещаем JSON, чтомы запустили Popup

                    }, (obj) => true));
            }
        }

        // удаление данных
        private RelayCommand _btn_DeleteData { get; set; }
        public RelayCommand Btn_DeleteData
        {
            get
            {
                return _btn_DeleteData ??
                    (_btn_DeleteData = new RelayCommand(async (obj) =>
                    {
                        // проверка на удаление собственного аккаунта
                        AuthorizationViewModel authorizationViewModel = new AuthorizationViewModel();
                        int userId = await authorizationViewModel.WeGetIdUser();

                        if(userId != SelectedAccount.id)
                        {
                            using (FoodDeliveryContext foodDeliveryContext = new FoodDeliveryContext())
                            {
                                // ищем нужную категорию для удаления
                                Model.Account accounts = await foodDeliveryContext.Accounts.FirstOrDefaultAsync(c => c.id == SelectedAccount.id);
                                if (accounts != null)
                                {
                                    foodDeliveryContext.Accounts.Remove(accounts);
                                    await foodDeliveryContext.SaveChangesAsync(); // cохраняем изменения в базе данных                       
                                    ClosePopupWorkingWithData(); // закрываем Popup
                                    GetListAccounts(); // обновляем список
                                }
                            }
                        }
                        else
                        {
                            ClosePopupWorkingWithData(); // закрываем Popup
                            // выводим сообщение об ошибке
                            ErrorInputData = "Нельзя удалить собственный аккаунт!";
                            BeginFadeAnimation(AnimationErrorInputData); // анимация затухания ошибки
                        }
                        
                    }, (obj) => true));
            }
        }

        // запускаем Popup (для редактирования или удаления)
        private void LaunchingPopupWhenGettingFocus(object sender, EventAggregator eventAggregator)
        {
            if (IsCheckAddAndEditOrDelete) // если это добавление или редактирование
            {
                StartPoupOfUsers = true; // отображаем Popup
            }
            else // если это удаление данных 
            {
                StartPoupOfDelete = true; // отображаем Popup
            }
            DarkBackground = Visibility.Visible; // показать фон
            WorkingWithData.ExitHamburgerMenu(); // закрываем, если открыто "гамбургер меню"
        }

        // записываем в JSON, что мы запустили Popup данной страницы
        private void NotificationOfThePopupLaunchJson()
        {
            // передаём в JSON, что мы запустили Popup
            var jsonData = new { popup = "Users" };
            // Преобразуем объект в JSON-строку
            string jsonText = JsonConvert.SerializeObject(jsonData);
            File.WriteAllText(pathDataPopup, jsonText);
        }

        #endregion

        // общие свойства страницы
        #region Features
        public TextBlock AnimationErrorInput { get; set; } //  анимация текста ошибки поиска
        public TextBlock AnimationErrorInputData { get; set; } //  анимация текста ошибки удаления
        public TextBlock AnimationErrorInputPopup { get; set; } //  анимации текста ошибки на Popup

        public TextBox AnimationOutName { get; set; } // поле для ввода текста "имя клиента". Вывод подсветки поля
        public TextBox AnimationOutSurname { get; set; } // поле для ввода текста "фамилия клиента". Вывод подсветки поля
        public TextBox AnimationOutPatronymic { get; set; } // поле для ввода текста "отчество клиента". Вывод подсветки поля
        public TextBox AnimationOutCity { get; set; } // поле для ввода текста "город клиента". Вывод подсветки поля
        public TextBox AnimationOutStreet { get; set; } // поле для ввода текста "улица клиента". Вывод подсветки поля
        public TextBox AnimationOutHouse { get; set; } // поле для ввода текста "дом клиента". Вывод подсветки поля
        public TextBox AnimationOutApartment { get; set; } // поле для ввода текста "квартира клиента". Вывод подсветки поля
        public TextBox AnimationOutNumberPhone { get; set; } // поле для ввода текста "номер телефона клиента". Вывод подсветки поля
        public TextBox AnimationOutEmail { get; set; } // поле для ввода текста "email клиента". Вывод подсветки поля

        public TextBox AnimationOutLogin { get; set; } // поле для ввода текста "логин". Вывод подсветки поля
        public PasswordBox AnimationOutPassword { get; set; } // поле для ввода текста "пароль". Вывод подсветки поля
        public PasswordBox AnimationOutNewPassword { get; set; } // поле для ввода текста "пароль". Вывод подсветки поля
        public ComboBox AnimationRole { get; set; } // поле для выбора роли пользователя. Вывод подсветки поля

        public Storyboard FieldIllumination { get; set; } // анимация объектов

        // ассинхронно получаем информацию из OrdersPage
        public async Task InitializeAsync(TextBlock AnimationErrorInput, TextBlock AnimationErrorInputPopup, TextBox AnimationOutName,
            TextBox AnimationOutSurname, TextBox AnimationOutPatronymic, TextBox AnimationOutCity,
            TextBox AnimationOutStreet, TextBox AnimationOutHouse, TextBox AnimationOutApartment,
            TextBox AnimationOutNumberPhone, TextBox AnimationOutEmail, ComboBox AnimationRole,
            TextBox AnimationOutLogin, PasswordBox AnimationOutPassword, PasswordBox AnimationOutNewPassword,
            Storyboard FieldIllumination, TextBlock AnimationErrorInputData)
        {
            if (AnimationErrorInput != null)
            {
                this.AnimationErrorInput = AnimationErrorInput;
            }
            if (AnimationErrorInputPopup != null)
            {
                this.AnimationErrorInputPopup = AnimationErrorInputPopup;
            }
            if (AnimationOutName != null)
            {
                this.AnimationOutName = AnimationOutName;
            }
            if(AnimationOutSurname != null)
            {
                this.AnimationOutSurname = AnimationOutSurname;
            }
            if(AnimationOutPatronymic != null)
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
            if (AnimationOutHouse != null)
            {
                this.AnimationOutHouse = AnimationOutHouse;
            }
            if (AnimationOutHouse != null)
            {
                this.AnimationOutHouse = AnimationOutHouse;
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
            if (AnimationOutLogin != null)
            {
                this.AnimationOutLogin = AnimationOutLogin;
            }
            if (AnimationRole != null)
            {
                this.AnimationRole = AnimationRole;
            }
            if (AnimationOutPassword != null)
            {
                this.AnimationOutPassword = AnimationOutPassword;
            }
            if (AnimationOutNewPassword != null)
            {
                this.AnimationOutNewPassword = AnimationOutNewPassword;
            }
            if(FieldIllumination != null)
            {
                this.FieldIllumination = FieldIllumination;
            }
            if (AnimationErrorInputData != null)
            {
                this.AnimationErrorInputData = AnimationErrorInputData;
            }
        }

        
        // свойство для вывода ошибки при поиске данных в таблице
        private string _errorInput { get; set; }
        public string ErrorInput
        {
            get { return _errorInput; }
            set { _errorInput = value; OnPropertyChanged(nameof(ErrorInput)); }
        }

        // свойство вывода ошибок
        private string _errorInputData { get; set; }
        public string ErrorInputData
        {
            get { return _errorInputData; }
            set { _errorInputData = value; OnPropertyChanged(nameof(ErrorInputData)); }
        }

        // выбранный пользователь
        private AccountDPO _selectedAccount { get; set; }
        public AccountDPO SelectedAccount
        {
            get { return _selectedAccount; }
            set
            {
                _selectedAccount = value; OnPropertyChanged(nameof(SelectedAccount));
                OnPropertyChanged(nameof(IsWorkButtonEnable));
            }
        }

        // отображение кнопки "удалить" и "редакировать"
        private bool _isWorkButtonEnable { get; set; }
        public bool IsWorkButtonEnable
        {
            get { return SelectedAccount != null; } // если в таблице выбранн объект, то кнопки работают
            set { _isWorkButtonEnable = value; OnPropertyChanged(nameof(IsWorkButtonEnable)); }
        }

        // Page для запуска страницы
        private Page _pageFrame { get; set; }
        public Page PageFrame
        {
            get { return _pageFrame; }
            set { _pageFrame = value; OnPropertyChanged(nameof(PageFrame)); }
        }

        // очистка фрейма (памяти)
        public void ClearMemoryAfterFrame(FrameworkElement element)
        {
            if (element != null)
            {
                // очистка всех привязанных элементов
                BindingOperations.ClearAllBindings(element);
                // очистка визуальных элементов
                element.Resources.Clear();
                // Очистка ссылки на предыдущий экземпляр
                element = null;
            }

            if (PageFrame != null)
            {
                // очистка фрейма
                PageFrame.Content = null;
            }
        }

        #endregion

        // свойства Popup
        #region FeaturesUsers

        // Popup для редактирования или добавления данных
        #region PopupAddOrEditData

        // выбранная роль
        private Role _selectedRole { get; set; }
        public Role SelectedRole
        {
            get { return _selectedRole; }
            set
            {
                _selectedRole = value; OnPropertyChanged(nameof(SelectedRole));
            }
        }

        // ComBox пользователи
        private List<Role> _optionsRole { get; set; }
        public List<Role> OptionsRole
        {
            get { return _optionsRole; }
            set
            {
                _optionsRole = value;
                OnPropertyChanged(nameof(OptionsRole));
            }
        }

        //  отображение Popup для добавления
        private bool _startPoupOfUsers { get; set; }
        public bool StartPoupOfUsers
        {
            get { return _startPoupOfUsers; }
            set
            {
                _startPoupOfUsers = value;
                OnPropertyChanged(nameof(StartPoupOfUsers));
            }
        }

        // заголовок 
        private string _headingPopup { get; set; }
        public string HeadingPopup
        {
            get { return _headingPopup; }
            set { _headingPopup = value; OnPropertyChanged(nameof(HeadingPopup)); }
        }

        // сообщение об ошибке
        private string _errorInputPopup { get; set; }
        public string ErrorInputPopup
        {
            get { return _errorInputPopup; }
            set { _errorInputPopup = value; OnPropertyChanged(nameof(ErrorInputPopup)); }
        }

        // название кнопки для подверждения действия при удалении или редактировании
        private string _actionConfirmationButton { get; set; }
        public string ActionConfirmationButton
        {
            get { return _actionConfirmationButton; }
            set { _actionConfirmationButton = value; OnPropertyChanged(nameof(ActionConfirmationButton)); }
        }

        // видимость полей для изменения пароля
        private bool _isEditUser { get; set; }
        public bool IsEditUser
        {
            get { return _isEditUser; }
            set { _isEditUser = value; OnPropertyChanged(nameof(IsEditUser)); }
        }

        #endregion

        // Popup для удаления данных
        #region PopupDeleteData

        //  отображение Popup для удаления
        private bool _startPoupOfDelete { get; set; }
        public bool StartPoupOfDelete
        {
            get { return _startPoupOfDelete; }
            set
            {
                _startPoupOfDelete = value;
                OnPropertyChanged(nameof(StartPoupOfDelete));
            }
        }

        // вывод пользователся при удалении
        private string _nameOfUserDeleted { get; set; }
        public string NameOfUserDeleted
        {
            get { return _nameOfUserDeleted; }
            set { _nameOfUserDeleted = value; OnPropertyChanged(nameof(NameOfUserDeleted)); }
        }

        #endregion

        // фон для Popup
        private Visibility _darkBackground { get; set; }
        public Visibility DarkBackground
        {
            get { return _darkBackground; }
            set
            {
                _darkBackground = value;
                OnPropertyChanged(nameof(DarkBackground));
            }
        }

        // скрываем Popup
        private RelayCommand _closePopup { get; set; }
        public RelayCommand ClosePopup
        {
            get
            {
                return _closePopup ??
                    (_closePopup = new RelayCommand((obj) =>
                    {
                        ClosePopupWorkingWithData(); // метод закрытия Popup
                    }, (obj) => true));
            }
        }

        // закрываем Popup
        private async Task ClosePopupWorkingWithData()
        {
            // Закрываем Popup
            StartPoupOfUsers = false;
            StartPoupOfDelete = false;
            DarkBackground = Visibility.Collapsed; // скрываем фон
        }

        #endregion

        // поиск данных в таблице
        #region UsersSearch

        // список для фильтров таблицы
        public ObservableCollection<AccountDPO> ListSearch { get; set; } = new ObservableCollection<AccountDPO>();

        public async Task HandlerTextBoxChanged(string searchByValue)
        {
            if (!string.IsNullOrWhiteSpace(searchByValue))
            {
                await GetListAccounts(); // обновляем список
                ListSearch.Clear(); // очищаем список поиска данных

                // объединяем дату заказа, время досатвки, имя и фамилию клиента для поиска
                foreach (AccountDPO item in ListAccounts)
                {
                    string unification = item.registrationDate.ToString().ToLower() + " " + item.login.ToString().ToLower() + " " +
                        item.nameRole.ToString().ToLower();

                    bool dataExists = unification.Contains(searchByValue.ToLowerInvariant());

                    if (dataExists)
                    {
                        ListSearch.Add(item);
                    }
                }

                ListAccounts.Clear(); // очищаем список перед заполнением
                ListAccounts = ListSearch; // обновляем список

                if (ListSearch.Count == 0)
                {
                    // оповещениие об отсутствии данных
                    ErrorInput = "Пользователь не найдена!"; // собщение об ошибке
                    BeginFadeAnimation(AnimationErrorInput); // анимация затухания ошибки
                }
            }
            else
            {
                ListAccounts.Clear();
                await GetListAccounts(); // обновляем список
            }
        }

        #endregion

        // анимации
        #region Animation

        // анимация затухания текста
        private void BeginFadeAnimation(TextBlock textBlock)
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

        private void StartFieldIllumination(TextBox textBox)
        {
            FieldIllumination.Begin(textBox);
        }
        private void StartFieldIllumination(ComboBox comboBox)
        {
            FieldIllumination.Begin(comboBox);
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
