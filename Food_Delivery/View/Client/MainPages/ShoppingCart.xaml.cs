using Food_Delivery.Model.DPO;
using Food_Delivery.ViewModel.Client;
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

namespace Food_Delivery.View.Client.MainPages
{
    /// <summary>
    /// Interaction logic for ShoppingCart.xaml
    /// </summary>
    public partial class ShoppingCart : UserControl
    {
        private readonly ShoppingCartViewModel _shoppingCartViewModel; // объект класса
        public ShoppingCart()
        {
            InitializeComponent();
            _shoppingCartViewModel = (ShoppingCartViewModel)this.Resources["ShoppingCartViewModel"];
        }

        // работа с товарами
        #region Product

        // удаляем товар из корзины
        private async void DeleteItemShoppingCartButton(object sender, RoutedEventArgs e)
        {
            // получаем элемент после нажатия на кнопку
            CompositionCartDPO compositionCartDPO = (e.Source as FrameworkElement)?.DataContext as CompositionCartDPO;
            if (compositionCartDPO != null)
            {
                // удаляем товар из корзины
                await _shoppingCartViewModel.DeleteItemToShoppingCart(compositionCartDPO);

            }
        }

        // изменение товара в корзине в большую сторону
        private async void AddItemShoppingCartButton(object sender, RoutedEventArgs e)
        {
            // получаем элемент после нажатия на кнопку
            CompositionCartDPO compositionCartDPO = (e.Source as FrameworkElement)?.DataContext as CompositionCartDPO;
            if (compositionCartDPO != null)
            {
                // изменение товара в корзине
                await _shoppingCartViewModel.AddItemShoppingCart(compositionCartDPO);

            }
        }

        // изменение товара в корзине в меньшую сторону
        private async void RemoveItemShoppingCartButton(object sender, RoutedEventArgs e)
        {
            // получаем элемент после нажатия на кнопку
            CompositionCartDPO compositionCartDPO = (e.Source as FrameworkElement)?.DataContext as CompositionCartDPO;
            if (compositionCartDPO != null)
            {
                // изменение товара в корзине
                await _shoppingCartViewModel.RemoveItemShoppingCart(compositionCartDPO);

            }
        }

        #endregion
    }
}
