using Food_Delivery.Helper;
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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Food_Delivery.View.Administrator.MenuSectionPages
{
    /// <summary>
    /// Interaction logic for PageCategory.xaml
    /// </summary>
    public partial class PageCategory : Page
    {
        private readonly CategoryViewModel _categoryViewModel; // объект класса
        public PageCategory()
        {
            InitializeComponent();

            // ассинхронно передаём фрейм и другие атрибуты в CategoryViewModel
            _categoryViewModel = (CategoryViewModel)this.Resources["CategoryViewModel"];
            _categoryViewModel.InitializeAsync(AddAndEditDataPopup, DarkBackground);
        }

        #region Popup

        // скрыть фон при скрытие popup
        private void MyPopup_Closed(object sender, EventArgs e)
        {
            DarkBackground.Visibility = Visibility.Collapsed;
        }

        #endregion

        // закрываем "гамбургер" меню если открыто
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WorkingWithData.ExitHamburgerMenu();
        }
    }
}
