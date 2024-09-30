using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Food_Delivery.Helper
{
    // обработка событий программы
    public class WorkingWithData
    {
        // гамбургер меню администратора
        #region hamburgerMenuAdmin

        // закрываем гамбургер меню
        public static event EventHandler<EventAggregator> _exitHamburgerMenu; // подписываемся в 
        public static void ExitHamburgerMenu() // вызываем событие в 
        {
            _exitHamburgerMenu?.Invoke(null, new EventAggregator());
        }

        #endregion

        // меню администратора 
        #region mainMenuAdmin

        // переход на страницу "категориии"
        public static event EventHandler<EventAggregator> _openCategoryPage; // подписываемся в MainMenuViewModel
        public static void OpenCategoryPage()
        {
            _openCategoryPage?.Invoke(null, new EventAggregator()); // вызываем событие в HamburgerMenuViewModel
        }

        // переход на страницу "блюда"
        public static event EventHandler<EventAggregator> _openDishesPage; // подписываемся в MainMenuViewModel
        public static void OpenDishesPage()
        {
            _openDishesPage?.Invoke(null, new EventAggregator()); // вызываем событие в HamburgerMenuViewModel
        }

        // переход на страницу "заказы"
        public static event EventHandler<EventAggregator> _openOrdersPage; // подписываемся в MainMenuViewModel
        public static void OpenOrdersPage()
        {
            _openOrdersPage?.Invoke(null, new EventAggregator()); // вызываем событие в HamburgerMenuViewModel
        }

        // переход на страницу "пользователи"
        public static event EventHandler<EventAggregator> _openUsersPage; // подписываемся в MainMenuViewModel
        public static void OpenUsersPage()
        {
            _openUsersPage?.Invoke(null, new EventAggregator()); // вызываем событие в HamburgerMenuViewModel
        }

        #endregion

        // открываем нужный Popup при получении фокуса страницы
        #region PopupWindowClosedWhenFocusLost 

        // запуск Popup категорий
        public static event EventHandler<EventAggregator> _launchPopupAfterReceivingFocusCategory; // подписываемся в CategoryViewModel
        public static void LaunchPopupAfterReceivingFocusCategory()
        {
            _launchPopupAfterReceivingFocusCategory?.Invoke(null, new EventAggregator()); // вызываем событие в MainWindow
        }

        // запуск Popup блюд
        public static event EventHandler<EventAggregator> _launchPopupAfterReceivingFocusDish; // подписываемся в DishesViewModel
        public static void LaunchPopupAfterReceivingFocusDish()
        {
            _launchPopupAfterReceivingFocusDish?.Invoke(null, new EventAggregator()); // вызываем событие в MainWindow
        }

        // запуск Poup заказы
        public static event EventHandler<EventAggregator> _launchPopupAfterReceivingFocusOrders; // подписываемся в WorkingWithDataOrdersViewModel
        public static void LaunchPopupAfterReceivingFocusOrders()
        {
            _launchPopupAfterReceivingFocusOrders?.Invoke(null, new EventAggregator()); // вызываем событие в MainWindow
        }

        // запуск Poup пользователи
        public static event EventHandler<EventAggregator> _launchPopupAfterReceivingFocusUsers; // подписываемся в UsersViewModel
        public static void LaunchPopupAfterReceivingFocusUsers()
        {
            _launchPopupAfterReceivingFocusUsers?.Invoke(null, new EventAggregator()); // вызываем событие в MainWindow
        }

        // запуск Poup меню администратора
        public static event EventHandler<EventAggregator> _launchPopupAfterReceivingFocusMainMenu; // подписываемся в MainMenuViewModel
        public static void LaunchPopupAfterReceivingFocusMainMenu()
        {
            _launchPopupAfterReceivingFocusMainMenu?.Invoke(null, new EventAggregator()); // вызываем событие в MainWindow
        }

        #endregion

        // работа над заказами
        #region Orders

        // закрываем страницу для редактирования и добавления данных и возвращаемся к общему списку заказов
        public static event EventHandler<EventAggregator> _closingCorkWithDataOrdersPage; // подписываемся в OrdersViewModel
        public static void ClosingCorkWithDataOrdersPage()
        {
            _closingCorkWithDataOrdersPage?.Invoke(null, new EventAggregator()); // вызываем событие в WorkingWithDataOrdersViewModel
        }

        #endregion

        // работа над авторизацией
        #region Authorization

        // успешная авторизация в аккаунт
        public static event EventHandler<EventAggregator> SuccessfulLogin; // подписываемся в MainWindow
        public static void SuccessfulLoginAccount() // вызываем в AuthorizationViewModel
        {
            SuccessfulLogin?.Invoke(null, new EventAggregator());
        }

        // выход из аккаунта
        public static event EventHandler<EventAggregator> ExitFromAccount; // подписываемся в MainWindow
        public static void ExitPageFromAccount() // вызываем в AuthorizationViewModel
        {
            ExitFromAccount?.Invoke(null, new EventAggregator());
        }

        #endregion

        // работа над корзиной и товарами
        #region ShoppinCartAndProductPage

        // обновляем список товаров
        public static event EventHandler<EventAggregator> _updatingListProducts; // подписываемся в ProductViewModel
        public static void UpdatingListProducts() // вызываем событие в MainMenuServicePage
        {
            _updatingListProducts?.Invoke(null, new EventAggregator()); 
        }

        // отображаем фон при запуске корзины
        public static event EventHandler<EventAggregator> _backgroundForShopping; // подписываемся в MainMenuServiceViewModel
        public static void BackgroundForShopping()
        {
            _backgroundForShopping?.Invoke(null, new EventAggregator()); // вызываем событие в ShoppingCartViewModel
        }

        // закрываем корзину
        public static event EventHandler<EventAggregator> _exitShoppingCart; // подписываемся в MainMenuServicePage
        public static void ExitShoppingCart() // вызываем событие в ShoppingCartViewModel
        {
            _exitShoppingCart?.Invoke(null, new EventAggregator());
        }

        #endregion
    }
}
