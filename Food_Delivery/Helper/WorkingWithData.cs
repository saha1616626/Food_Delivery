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
        #region hamburgerMenuAdmin

        // закрываем гамбургер меню
        public static event EventHandler<EventAggregator> _exitHamburgerMenu; // подписываемся в 
        public static void ExitHamburgerMenu() // вызываем событие в 
        {
            _exitHamburgerMenu?.Invoke(null, new EventAggregator());
        }

        #endregion

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

        #endregion

        // открываем нужный Popup при получении фокуса страницы
        #region PopupWindowClosedWhenFocusLost 

        public static event EventHandler<EventAggregator> _launchPopupAfterReceivingFocusDish; // подписываемся в DishesViewModel
        public static void LaunchPopupAfterReceivingFocusDish()
        {
            _launchPopupAfterReceivingFocusDish?.Invoke(null, new EventAggregator()); // вызываем событие в MainWindow
        }

        #endregion
    }
}
