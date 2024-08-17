using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
