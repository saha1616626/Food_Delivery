using Food_Delivery.Helper;
using Food_Delivery.View.Administrator.MenuSectionPages;
using Food_Delivery.ViewModel.Administrator;
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
    /// Interaction logic for MainMenuPage.xaml
    /// </summary>
    public partial class MainMenuPage : Page
    {
        private readonly MainMenuViewModel _mainMenuViewModel; // объект класса
        public MainMenuPage()
        {
            InitializeComponent();

            _mainMenuViewModel = (MainMenuViewModel)this.Resources["MainMenuViewModel"];
            // ассинхронно передаём фрейм в MainMenuViewModel
            _mainMenuViewModel.InitializeAsync(mainAdminMenu);

        }

        // закрываем "гамбургер" меню, если открыто, при нажатии на окно, но не на меню
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WorkingWithData.ExitHamburgerMenu();
        }

    }
}
