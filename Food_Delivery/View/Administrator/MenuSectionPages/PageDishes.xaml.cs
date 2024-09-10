using Food_Delivery.Helper;
using Food_Delivery.ViewModel.Administrator;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.IO;
using Newtonsoft.Json;
using System.Windows.Media.Animation;
using Food_Delivery.Model;

namespace Food_Delivery.View.Administrator.MenuSectionPages
{
    /// <summary>
    /// Interaction logic for PageDishes.xaml
    /// </summary>
    public partial class PageDishes : Page
    {

        private readonly DishesViewModel _dishesViewModel; // объект класса
        public PageDishes()
        {
            InitializeComponent();

            // ассинхронно передаём фрейм и другие атрибуты в DishesViewModel
            _dishesViewModel = (DishesViewModel)this.Resources["DishesViewModel"];
            _dishesViewModel.InitializeAsync(AddAndEditDataPopup, DarkBackground, (Storyboard)FindResource("FieldIllumination"),
                NameDishes, ErrorInputPopup, CbCategory, PriceDishes, QuantityDishes, CaloriesDishes, SquirrelsDishes, FatsDishes,
                CarbohydratesDishes, WeightDishes, DeleteDataPopup, ErrorInput);
        }


        #region Popup

        // скрыть фон при скрытие popup
        private void MyPopup_Closed(object sender, EventArgs e)
        {
            DarkBackground.Visibility = Visibility.Collapsed;
        }

        // после того, как Popap был закрыт, мы оповещаем систему, что не надо запускать Popup после потери фокуса на приожении
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

        // поиск категории
        private void DishesSearch(object sender, TextChangedEventArgs e)
        {
            // получаем текст из поля при поиске данных
            var textInfo = sender as System.Windows.Controls.TextBox;
            if (textInfo != null)
            {
                _dishesViewModel.HandlerTextBoxChanged(textInfo.Text);
            }
        }

        // закрываем "гамбургер" меню, если открыто, при нажатии на окно, но не на меню
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WorkingWithData.ExitHamburgerMenu();
        }

    }
}
