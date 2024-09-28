using Food_Delivery.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;
using System.Windows.Controls;
using Food_Delivery.Model;
using Food_Delivery.View.Administrator.MenuSectionPages;
using Food_Delivery.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using System.IO;

namespace Food_Delivery.ViewModel.Administrator
{
    // главный класс для смены фреймов
    public class MainMenuViewModel : INotifyPropertyChanged
    {
        public MainMenuViewModel()
        {
            DarkBackground = Visibility.Collapsed; // фон для Popup скрыт

            // подготовка страницы
            AccountSettings();

            // подписываемся на событие запуска страницы "категории"
            WorkingWithData._openCategoryPage += OpenCategoryPage;
            // подписываемся на событие запуска страницы "блюда"
            WorkingWithData._openDishesPage += OpenDishesPage;
            // подписываемся на событие запуска страницы "заказы"
            WorkingWithData._openOrdersPage += OpenOrdersPage;
            // подписываемся на событие запуска страницы "пользователи"
            WorkingWithData._openUsersPage += OpenUsersPage;

            // после получения фокуса данного приложения запукаем закрытый Popup
            WorkingWithData._launchPopupAfterReceivingFocusMainMenu += LaunchingPopupWhenGettingFocus;
        }

        #region launchPage

        // класс "категории"
        PageCategory pageCategory {  get; set; }

        // запуск страницы "категории"
        private void OpenCategoryPage(object sender, EventAggregator e)
        {
            ClearMemoryAfterFrame(pageCategory); // очистка памяти
            pageCategory = new PageCategory();
            LaunchFrame.NavigationService.Navigate(pageCategory);
            // закрываем "гамбургер" меню
            WorkingWithData.ExitHamburgerMenu();
        }

        // класс "блюа"
        PageDishes pageDishes { get; set; }

        // запуск страницы "блюда"
        private void OpenDishesPage(object sender, EventAggregator e)
        {
            ClearMemoryAfterFrame(pageDishes); // очистка памяти
            pageDishes = new PageDishes();
            LaunchFrame.NavigationService.Navigate(pageDishes);
            // закрываем "гамбургер" меню
            WorkingWithData.ExitHamburgerMenu();
        }

        // класс "заказы"
        PageOrders pageOrders { get; set; }

        // запуск страницы "заказы"
        private void OpenOrdersPage(object sender, EventAggregator e)
        {
            ClearMemoryAfterFrame(pageOrders); // очистка памяти
            pageOrders = new PageOrders();
            LaunchFrame.NavigationService.Navigate(pageOrders);
            // закрываем "гамбургер" меню
            WorkingWithData.ExitHamburgerMenu();
        }

        // класс "пользователи"
        PageUsers pageUsers { get; set; }

        // запуск страницы "пользователи"
        private void OpenUsersPage(object sender, EventAggregator e)
        {
            ClearMemoryAfterFrame(pageUsers); // очистка памяти
            pageUsers = new PageUsers();
            LaunchFrame.NavigationService.Navigate(pageUsers);
            // закрываем "гамбургер" меню
            WorkingWithData.ExitHamburgerMenu();
        }

        #endregion

        #region Features

        // название профиля
        private async Task AccountSettings()
        {
            // получаем id авторизованного пользователя
            AuthorizationViewModel authorizationViewModel = new AuthorizationViewModel();
            int userId = await authorizationViewModel.WeGetIdUser();
            string role = await authorizationViewModel.WeGetRoleUser();

            if (userId != 0 && userId != null)
            {
                using (FoodDeliveryContext foodDeliveryContext = new FoodDeliveryContext())
                {
                    Account accounts = await foodDeliveryContext.Accounts.FirstOrDefaultAsync(a => a.id == userId);
                    if (accounts != null)
                    {
                        AuthorizedUser = $"{role} \"{accounts.login}\"";
                    }
                }
            }
            
        }


        // ассинхронно передаём фрейм из MainMenuPage
        public async Task InitializeAsync(Frame openPage)
        {
            if(openPage != null)
            {
                LaunchFrame = openPage;
            }
        }

        // свойство для запуска фреймов страниц
        private Frame _launchFrame;
        public Frame LaunchFrame
        {
            get { return _launchFrame; }
            set
            {
                _launchFrame = value;
                LaunchFrame.NavigationService.Navigate(pageOrders = new PageOrders());
            }
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

            if (LaunchFrame != null)
            {
                // очистка фрейма
                LaunchFrame.Content = null;
            }
        }

        #endregion


        // свойства 
        #region Features

        // подпись заголовка профиля
        private string _authorizedUser { get; set; }
        public string AuthorizedUser
        {
            get { return _authorizedUser; }
            set { _authorizedUser = value; OnPropertyChanged(nameof(AuthorizedUser)); }
        }

        #endregion

        // свойства и методы Popup
        #region FeaturesUsers

        // запускаем Popup для удаления данных
        private RelayCommand _btn_LogOutOfYourAccount { get; set; }
        public RelayCommand Btn_LogOutOfYourAccount
        {
            get
            {
                return _btn_LogOutOfYourAccount ??
                    (_btn_LogOutOfYourAccount = new RelayCommand((obj) =>
                    {
                        StartPoupOfOutAccount = true; // отображаем Popup
                        DarkBackground = Visibility.Visible; // показать фон
                        WorkingWithData.ExitHamburgerMenu(); // закрываем, если открыто "гамбургер меню"

                        NotificationOfThePopupLaunchJson(); // оповещаем JSON, чтомы запустили Popup

                    }, (obj) => true));
            }
        }

        // выход из аккаунта
        private RelayCommand _btn_LogOutYourAccount { get; set; }
        public RelayCommand Btn_LogOutYourAccount
        {
            get
            {
                return _btn_LogOutYourAccount ??
                    (_btn_LogOutYourAccount = new RelayCommand((obj) =>
                    {
                        AuthorizationViewModel authorizationViewModel = new AuthorizationViewModel();
                        authorizationViewModel.LogOutYourAccount();
                    }, (obj) => true));
            }
        }

        // путь к json работа окна Popup
        readonly string pathDataPopup = @"E:\3comm\Documents\Предметы\Курс 3.2\Курсовая\Приложение\Программа\Food_Delivery\Food_Delivery\Data\СheckPopup.json";

        // записываем в JSON, что мы запустили Popup данной страницы
        private void NotificationOfThePopupLaunchJson()
        {
            // передаём в JSON, что мы запустили Popup
            var jsonData = new { popup = "MainMenu" };
            // Преобразуем объект в JSON-строку
            string jsonText = JsonConvert.SerializeObject(jsonData);
            File.WriteAllText(pathDataPopup, jsonText);
        }

        // запускаем Popup для выхода из аккаунта
        private void LaunchingPopupWhenGettingFocus(object sender, EventAggregator eventAggregator)
        {
            StartPoupOfOutAccount = true;
            DarkBackground = Visibility.Visible; // показать фон
            WorkingWithData.ExitHamburgerMenu(); // закрываем, если открыто "гамбургер меню"
        }

        //  отображение Popup для выхода из аккаунта
        private bool _startPoupOfOutAccount { get; set; }
        public bool StartPoupOfOutAccount
        {
            get { return _startPoupOfOutAccount; }
            set
            {
                _startPoupOfOutAccount = value;
                OnPropertyChanged(nameof(StartPoupOfOutAccount));
            }
        }

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
            StartPoupOfOutAccount = false;
            DarkBackground = Visibility.Collapsed; // скрываем фон
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
