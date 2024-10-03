using Food_Delivery.Helper;
using Food_Delivery.ViewModel.Administrator;
using Food_Delivery.ViewModel.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Food_Delivery.View.Client.MainPages
{
    /// <summary>
    /// Interaction logic for MainMenuServicePage.xaml
    /// </summary>
    public partial class MainMenuServicePage : Page
    {
        private readonly MainMenuServiceViewModel _mainMenuServiceViewModel; // объект класса
        public MainMenuServicePage()
        {
            InitializeComponent();

            _mainMenuServiceViewModel = (MainMenuServiceViewModel)this.Resources["MainMenuServiceViewModel"];
            _mainMenuServiceViewModel.InitializeAsync((Storyboard)FindResource("FieldIllumination"), 
            ClientCity, ClientStreet, ClientHouse, ClientApartment, ErrorInputPopup);
        }

        #region Popup

        // скрыть фон при скрытие popup
        private void MyPopup_Closed(object sender, EventArgs e)
        {
            DarkBackground.Visibility = Visibility.Collapsed;
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

        // закрываем корзину, если открыто, при нажатии на окно, но не на меню
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // если корзина была открыта, то мы ее скрываем вместе с фоном и обновляем список
            if (_mainMenuServiceViewModel.IsBackgroundDisplay)
            {
                WorkingWithData.ExitShoppingCart(); // закрываем корзину
                DarkBackground.Visibility = Visibility.Hidden;
                _mainMenuServiceViewModel.IsBackgroundDisplay = false; // скрываем фон

                // обновляем список товаров
                WorkingWithData.UpdatingListProducts();
            }
        }
    }
}
