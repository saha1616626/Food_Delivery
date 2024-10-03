﻿using Food_Delivery.Data;
using Food_Delivery.Helper;
using Food_Delivery.Model;
using Food_Delivery.View.Client.MainPages;
using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Animation;
using static System.Net.Mime.MediaTypeNames;

namespace Food_Delivery.ViewModel.Client
{
    public class MainMenuServiceViewModel : INotifyPropertyChanged
    {
        AuthorizationViewModel authorizationViewModel = new AuthorizationViewModel();
        private readonly string pathShoppingCart = @"E:\3comm\Documents\Предметы\Курс 3.2\Курсовая\Приложение\Программа\Food_Delivery\Food_Delivery\Data\ShoppingCart\shoppingCart.json";
        private readonly string pathAddressClient = @"E:\3comm\Documents\Предметы\Курс 3.2\Курсовая\Приложение\Программа\Food_Delivery\Food_Delivery\Data\Address\AddressUnauthorizedUser.json";

        public MainMenuServiceViewModel()
        {
            DarkBackground = Visibility.Collapsed; // скрываем фон корзины

            // при запуске меню отображаем страницу с товарами
            StartingHomePage();

            // подтягиваем данные адреса клиента, если он имеется
            DisplayingUserAddress();

            // подписываемся на событие - отображаем фон при запуске корзины
            WorkingWithData._backgroundForShopping += BackgroundForShopping;

            // подписываемся на событие - запуск страницы оформления заказа
            WorkingWithData._launchPageMakingOrder += LaunchPageMakingOrder;

            // подписываемся на событие - закрываем страницу оформления заказа
            WorkingWithData._closingCheckoutPage += ClosingCheckoutPage;


            // после получения фокуса данного приложения запукаем закрытый Popup
            WorkingWithData._launchPopupAfterReceivingFocusMainMenuService += LaunchPopupAfterReceivingFocusMainMenuService;
        }

        // запуск страниц
        #region launchPage

        PageProduct pageProduct { get; set; } // страница с товарами
        PlaceOrderPage placeOrderPage { get; set; } // страница с оформлением заказа

        // запускаем страницу со списком товаров
        private async Task StartingHomePage()
        {
            ClearMemoryAfterFrame(placeOrderPage);
            ClearMemoryAfterFrame(pageProduct);
            pageProduct = new PageProduct();
            FrameMainMenu = pageProduct;
        }

        // запускаем страницу оформления заказа
        private async void LaunchPageMakingOrder(object sender, EventAggregator e)
        {
            ClearMemoryAfterFrame(placeOrderPage);
            ClearMemoryAfterFrame(pageProduct);
            placeOrderPage = new PlaceOrderPage();
            FramePlaceOrder = placeOrderPage;
        }

        // закрываем страницу оформления заказа, переходим на страницу с товарами и открываем корзину
        private async void ClosingCheckoutPage(object sender, EventAggregator e)
        {
            ClearMemoryAfterFrame(placeOrderPage);
            ClearMemoryAfterFrame(pageProduct);
            pageProduct = new PageProduct();
            FrameMainMenu = pageProduct;

            // запускаем корзину
            //WorkingWithData.OpenShoppingCart();

            DarkBackground = Visibility.Collapsed; // скрывем фон
        }
        
        /// <summary>
        /// переход на страницу авторизации
        /// </summary>
        private RelayCommand _btn_Authorization { get; set; }
        public RelayCommand Btn_Authorization
        {
            get
            {
                return _btn_Authorization ??
                    (_btn_Authorization = new RelayCommand(async (obj) =>
                    {
                        WorkingWithData.ExitPageFromAccount();
                    }, (obj) => true));
            }
        }

        #endregion

        // работа над корзиной
        #region shoppingCart

        // отображаем фон при запуске корзины
        public bool IsBackgroundDisplay = false; // переключение режима работы фона
        private async void BackgroundForShopping(object sender, EventAggregator e)
        {
            IsBackgroundDisplay = !IsBackgroundDisplay;

            DarkBackground = IsBackgroundDisplay ? Visibility.Visible : Visibility.Collapsed;

            // если фон скрыт, то обновляем список товаров
        }

        #endregion

        // Popup адреса доставки или выхода из аккаунта
        #region Popup

        readonly string pathDataPopup = @"E:\3comm\Documents\Предметы\Курс 3.2\Курсовая\Приложение\Программа\Food_Delivery\Food_Delivery\Data\СheckPopup.json";

        // переменная отвечающая за выбор Popup: адрес или выходод из аккаунта
        private bool IsAdressUser { get; set; }

        // запускаем Popup работы с адрессом клиента
        private RelayCommand _btn_AdressUser { get; set; }
        public RelayCommand Btn_AdressUser
        {
            get
            {
                return _btn_AdressUser ??
                    (_btn_AdressUser = new RelayCommand(async (obj) =>
                    {
                        StartPoupUserAdress = true; // запускаем Popup
                        DarkBackground = Visibility.Visible; // темный фон
                        IsAdressUser = true; // оповещаем систему, что мы начали редактирование адреса

                        // подтягиваем данные адреса клиента, если он имеется
                        await DisplayingUserAddress();

                        NotificationOfThePopupLaunchJson(); // оповещаем JSON, чтомы запустили Popup
                    }, (obj) => true));
            }
        }

        // Вывода адреса клиента
        public async Task DisplayingUserAddress()
        {
            string address = "";

            // проверяем, гость или авторизованный пользователь. 
            string role = await authorizationViewModel.WeGetRoleUser();
            if (role != null)
            {
                // получение адреса клиента
                string jsonAdress = File.ReadAllText(pathAddressClient);
                AddressUnauthorizedUser? addressUnauthorizedUser = JsonConvert.DeserializeObject<AddressUnauthorizedUser>(jsonAdress);
                if (addressUnauthorizedUser != null)
                {
                    OutClientCity = addressUnauthorizedUser.city;
                    OutClientStreet = addressUnauthorizedUser.street;
                    OutClientHouse = addressUnauthorizedUser.house;
                    if (addressUnauthorizedUser.apartment != null)
                    {
                        OutClientApartment = addressUnauthorizedUser.apartment;
                    }

                    address = $"г.{OutClientCity}, {OutClientStreet}, {OutClientHouse}, {OutClientApartment}";
                }
            }
            else if (role == "Пользователь")
            {

            }

            // Выводим адрес в меню
            if (address.Length >= 24) // если больше 24 символов, то обрезаем строчку
            {
                SelectedAdress = address.Substring(0, 24) + "...";
            }
        }

        // закрываем Popup
        private async Task ClosePopupWorkingWithData()
        {
            // Закрываем Popup
            StartPoupUserAdress = false;
            DarkBackground = Visibility.Collapsed; // скрываем фон
        }

        // закрываем Popup
        private RelayCommand _closePopup { get; set; }
        public RelayCommand ClosePopup
        {
            get
            {
                return _closePopup ??
                    (_closePopup = new RelayCommand(async (obj) =>
                    {
                        using (FoodDeliveryContext foodDeliveryContext = new FoodDeliveryContext())
                        {
                            ClosePopupWorkingWithData();
                        }
                    }, (obj) => true));
            }
        }

        /// <summary>
        /// Сохраняем данные адреса.
        /// Должны быть заполнены все поля, кроме "кавртира"
        /// </summary>
        private RelayCommand _btn_AddAdress { get; set; }
        public RelayCommand Btn_AddAdress
        {
            get
            {
                return _btn_AddAdress ??
                    (_btn_AddAdress = new RelayCommand(async (obj) =>
                    {
                        // проверка наличия обязательных данных
                        if (!string.IsNullOrWhiteSpace(OutClientCity) && !string.IsNullOrWhiteSpace(OutClientStreet) &&
                       !string.IsNullOrWhiteSpace(OutClientHouse))
                        {
                            bool isCheckingHouse = false; // переменная корректности введённого дома
                            bool isCheckingApartment = false; // переменная корректности введённой квартиры

                            ErrorInputPopup = ""; // очищаем сообщение об ошибке

                            // проверяем целое число в поле "Дом"
                            isCheckingHouse = int.TryParse(OutClientHouse.Trim(), out int house);
                            if (!isCheckingHouse) // число не получено
                            {
                                StartFieldIllumination(AnimationOutHouse); // подсветка поля
                                ErrorInputPopup = "Введите целое число!"; // сообщение с обибкой
                            }

                            // проверяем цело число в поле "Квартира"
                            if (OutClientApartment != null && OutClientApartment.Trim() != "")
                            {
                                isCheckingApartment = int.TryParse(OutClientApartment.Trim(), out int apartament);
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

                            BeginFadeAnimation(AnimationErrorInputPopup); // затухание сообщения об ошибке

                            // проверка формата данных
                            if (isCheckingApartment && isCheckingHouse)
                            {
                                // проверяем, гость или авторизованный пользователь. 
                                string role = await authorizationViewModel.WeGetRoleUser();

                                if (role != null)
                                {
                                    // Сохраняем новые данные в JSON
                                    AddressUnauthorizedUser addressUnauthorizedUser = new AddressUnauthorizedUser();
                                    addressUnauthorizedUser.city = OutClientCity.Trim();
                                    addressUnauthorizedUser.street = OutClientStreet.Trim();
                                    addressUnauthorizedUser.house = OutClientHouse.Trim();
                                    if (!string.IsNullOrWhiteSpace(OutClientApartment))
                                    {
                                        addressUnauthorizedUser.apartment = OutClientApartment.Trim();
                                    }

                                    // добавляем данные в JSON
                                    string updatedJsonData = JsonConvert.SerializeObject(addressUnauthorizedUser, Formatting.Indented); // обновленный JSON
                                    File.WriteAllText(pathAddressClient, updatedJsonData);
                                    ClosePopupWorkingWithData(); // закрываем Popup
                                }
                                else if (role == "Пользователь")
                                {

                                }

                                // обновляем данные адреса клиента
                                DisplayingUserAddress();
                            }

                        }
                        else
                        {
                            ErrorInputPopup = ""; // очищаем сообщение об ошибке

                            if (string.IsNullOrWhiteSpace(OutClientCity))
                            {
                                StartFieldIllumination(AnimationOutCity); // подсветка поля
                                ErrorInputPopup = "Заполните обязательные поля!"; // сообщение с ошибкой
                            }

                            if (string.IsNullOrWhiteSpace(OutClientStreet))
                            {
                                StartFieldIllumination(AnimationOutStreet); // подсветка поля
                                ErrorInputPopup = "Заполните обязательные поля!"; // сообщение с ошибкой
                            }

                            if (string.IsNullOrWhiteSpace(OutClientHouse))
                            {
                                StartFieldIllumination(AnimationOutHouse); // подсветка поля
                                ErrorInputPopup = "Заполните обязательные поля!"; // сообщение с ошибкой
                            }

                            BeginFadeAnimation(AnimationErrorInputPopup); // затухание сообщения об ошибке
                        }

                    }, (obj) => true));
            }
        }

        // записываем в JSON, что мы запустили Popup данной страницы
        private void NotificationOfThePopupLaunchJson()
        {
            // передаём в JSON, что мы запустили Popup
            var jsonData = new { popup = "MainMenuService" };
            // Преобразуем объект в JSON-строку
            string jsonText = JsonConvert.SerializeObject(jsonData);
            File.WriteAllText(pathDataPopup, jsonText);
        }

        // запускаем закрытый Popup после выхода из приложения
        private void LaunchPopupAfterReceivingFocusMainMenuService(object sender, EventAggregator eventAggregator)
        {
            if (IsAdressUser) // если был запущен popup адресса пользователя
            {
                StartPoupUserAdress = true; // отображаем Popup
                DarkBackground = Visibility.Visible; // показать фон
            }
            else //  если был запущен popup выхода из аккаунта
            {

                DarkBackground = Visibility.Visible; // показать фон
            }
        }

        #endregion

        // основные свойства
        #region Features

        public TextBox AnimationOutCity { get; set; } // поле для ввода текста "город клиента". Вывод подсветки поля
        public TextBox AnimationOutStreet { get; set; } // поле для ввода текста "улица клиента". Вывод подсветки поля
        public TextBox AnimationOutHouse { get; set; } // поле для ввода текста "дом клиента". Вывод подсветки поля
        public TextBox AnimationOutApartment { get; set; } // поле для ввода текста "квартира клиента". Вывод подсветки поля

        public Storyboard FieldIllumination { get; set; } // анимация объектов
        public TextBlock AnimationErrorInputPopup { get; set; } // объект текстового поля. Анимация затухания текста после вывода сообщения.

        public async Task InitializeAsync(Storyboard FieldIllumination, TextBox AnimationOutCity, TextBox AnimationOutStreet,
            TextBox AnimationOuttHouse, TextBox AnimationOutApartment, TextBlock AnimationErrorInputPopup)
        {
            if (FieldIllumination != null)
            {
                this.FieldIllumination = FieldIllumination;
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
            if(AnimationErrorInputPopup != null)
            {
                this.AnimationErrorInputPopup = AnimationErrorInputPopup;
            }
        }

        // Адрес клиента
        private string _selectedAdress { get; set; }
        public string SelectedAdress
        {
            get { return _selectedAdress; }
            set { _selectedAdress = value; OnPropertyChanged(nameof(SelectedAdress)); }
        }

        // ошибка в Popup
        private string _errorInputPopup { get; set; }
        public string ErrorInputPopup
        {
            get { return _errorInputPopup; }
            set { _errorInputPopup = value; OnPropertyChanged(nameof(ErrorInputPopup)); }
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

        //  запуск Popup добавление адреса
        private bool _startPoupUserAdress { get; set; }
        public bool StartPoupUserAdress
        {
            get { return _startPoupUserAdress; }
            set
            {
                _startPoupUserAdress = value;
                OnPropertyChanged(nameof(StartPoupUserAdress));
            }
        }

        // фон для корзины
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

        // Page для запуска страницы товары
        private Page _frameMainMenu { get; set; }
        public Page FrameMainMenu
        {
            get { return _frameMainMenu; }
            set { _frameMainMenu = value; OnPropertyChanged(nameof(FrameMainMenu)); }
        }

        // Page для запуска страницы оформление заказов
        private Page _framePlaceOrder { get; set; }
        public Page FramePlaceOrder
        {
            get { return _framePlaceOrder; }
            set { _framePlaceOrder = value; OnPropertyChanged(nameof(FramePlaceOrder)); }
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

            if (FrameMainMenu != null)
            {
                // очистка фрейма
                FrameMainMenu.Content = null;
            }

            if(FramePlaceOrder != null)
            {
                FramePlaceOrder.Content = null;
            }
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
