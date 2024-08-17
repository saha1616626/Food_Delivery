using Food_Delivery.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Food_Delivery.View.Administrator.Menu
{
    /// <summary>
    /// Interaction logic for HamburgerMenu.xaml
    /// </summary>
    public partial class HamburgerMenu : UserControl
    {
        public HamburgerMenu()
        {
            InitializeComponent();

            WorkingWithData._exitHamburgerMenu += ToggleSideMenu;
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleSideMenu();
        }

        private void ToggleSideMenu()
        {
            if (SideMenu.Width == 0)
            {
                SideMenu.Width = 200;
                SideMenu.Visibility = Visibility.Visible;
            }
            else
            {
                SideMenu.Width = 0;
                SideMenu.Visibility = Visibility.Collapsed;
            }
        }

        private void ToggleSideMenu(object sender, EventAggregator e)
        {
            if (SideMenu.Width == 0)
            {
                SideMenu.Width = 200;
                SideMenu.Visibility = Visibility.Visible;
            }
            else
            {
                SideMenu.Width = 0;
                SideMenu.Visibility = Visibility.Collapsed;
            }
        }
    }
}
