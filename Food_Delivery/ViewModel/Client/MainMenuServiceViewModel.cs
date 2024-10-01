using Food_Delivery.Helper;
using Food_Delivery.View.Client.MainPages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Food_Delivery.ViewModel.Client
{
    public class MainMenuServiceViewModel : INotifyPropertyChanged
    {
        public MainMenuServiceViewModel()
        {
            DarkBackground = Visibility.Collapsed; // скрываем фон корзины

            // при запуске меню отображаем страницу с товарами
            StartingHomePage();

            // подписываемся на событие - отображаем фон при запуске корзины
            WorkingWithData._backgroundForShopping += BackgroundForShopping;

            // подписываемся на событие - запуск страницы оформления заказа
            WorkingWithData._launchPageMakingOrder += LaunchPageMakingOrder;

            // подписываемся на событие - закрываем страницу оформления заказа
            WorkingWithData._closingCheckoutPage += ClosingCheckoutPage;
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
            WorkingWithData.OpenShoppingCart();
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

        // основные свойства
        #region Features

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

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
