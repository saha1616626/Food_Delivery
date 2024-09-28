using Food_Delivery.Helper;
using Food_Delivery.Model.DPO;
using Food_Delivery.ViewModel.Administrator;
using Food_Delivery.ViewModel.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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

namespace Food_Delivery.View.Client.MainPages
{
    /// <summary>
    /// Interaction logic for PageProduct.xaml
    /// </summary>
    public partial class PageProduct : Page
    {
        private readonly ProductViewModel _productViewModel; // объект класса
        public PageProduct()
        {
            InitializeComponent();
            _productViewModel = (ProductViewModel)this.Resources["ProductViewModel"];
        }

        // работа с товарами
        #region Product

        // добавляем товар в корзину
        private async void AddToCartButton(object sender, RoutedEventArgs e)
        {
            // получаем элемент после нажатия на кнопку добавить в корзину
            DishesDPO dishesDPO = (e.Source as FrameworkElement)?.DataContext as DishesDPO;
            if (dishesDPO != null)
            {
                // добавление данных в корзину
                _productViewModel.AddItemToShoppingCart(dishesDPO);

            }

        }

        // изменение товар в корзине в меньшую сторону
        private async void DeleteItemsToCartButton(object sender, RoutedEventArgs e)
        {
            // получаем элемент после нажатия на кнопку добавить в корзину
            DishesDPO dishesDPO = (e.Source as FrameworkElement)?.DataContext as DishesDPO;
            if (dishesDPO != null)
            {
                // изменение товар в корзине в меньшую сторону
                _productViewModel.RemoveItemToShoppingCart(dishesDPO);

            }
        }

        #endregion

        #region Popup

        // скрыть фон при скрытие popup
        private void MyPopup_Closed(object sender, EventArgs e)
        {
            //DarkBackground.Visibility = Visibility.Collapsed;
        }

        // после того, как Popup был закрыт, мы оповещаем систему, что не надо запускать Popup после потери фокуса на приложении
        private void Window_LossOfFocus(object sender, MouseButtonEventArgs e)
        {
            // путь к json работа окна Popup
            string pathDataPopup = @"E:\3comm\Documents\Предметы\Курс 3.2\Курсовая\Приложение\Программа\Food_Delivery\Food_Delivery\Data\СheckPopup.json";

            var jsonData = new { popup = "" };
            // Преобразуем объект в JSON-строку
            string jsonText = JsonConvert.SerializeObject(jsonData);
            File.WriteAllText(pathDataPopup, jsonText);
        }

        #endregion

        // закрываем "гамбургер" меню, если открыто, при нажатии на окно, но не на меню
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WorkingWithData.ExitHamburgerMenu();
        }
    }
}
