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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using System.IO;

namespace Food_Delivery.View.Administrator.MenuSectionPages
{
    /// <summary>
    /// Interaction logic for PageWorkingWithDataOrders.xaml
    /// </summary>
    public partial class PageWorkingWithDataOrders : Page
    {
        private readonly WorkingWithDataOrdersViewModel _workingWithDataOrdersViewModel; // объект класса

        public PageWorkingWithDataOrders(bool IsAddData)
        {
            InitializeComponent();

            _workingWithDataOrdersViewModel = (WorkingWithDataOrdersViewModel)this.Resources["WorkingWithDataOrdersViewModel"]; 
            _workingWithDataOrdersViewModel.ChangingName(IsAddData); // передаём состояния работы страницы (добавление или редактирование данных
        }

        #region Popup

        // после скрытия Poup изменяем фон
        private void PopupAddDishesOrder_Closed(object sender, EventArgs e)
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

        // нельзя в дату и время вручную внести данные
        private void DeliveryDateAndTime(object sender, TextCompositionEventArgs e)
        {
            e.Handled = true;
        }

        // закрываем "гамбургер" меню, если открыто, при нажатии на окно, но не на меню
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WorkingWithData.ExitHamburgerMenu();
        }
    }
}
