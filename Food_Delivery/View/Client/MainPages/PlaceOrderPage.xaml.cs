using Food_Delivery.Model.DPO;
using Food_Delivery.ViewModel.Administrator;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Food_Delivery.View.Client.MainPages
{
    /// <summary>
    /// Interaction logic for PlaceOrderPage.xaml
    /// </summary>
    public partial class PlaceOrderPage : Page
    {

        private readonly PlaceOrderViewModel _placeOrderViewModel; // объект класса
        public PlaceOrderPage()
        {
            InitializeComponent();

            _placeOrderViewModel = (PlaceOrderViewModel)this.Resources["PlaceOrderViewModel"];
            _placeOrderViewModel.ChangingName(); // начальная настройка страницы
            _placeOrderViewModel.InitializeAsync((Storyboard)FindResource("FieldIllumination"), ClientName,
                ClientSurname, ClientPatronymic, ClientCity, ClientStreet, ClientHouse, ClientApartment, ClientNumberPhone, ClientEmail,
                DeliveryDate, StartDesiredDeliveryTime, EndDesiredDeliveryTime, AmountChange, ErrorInput);

            SetDatePickerLimits(); // работа над датой заказа
        }

        // работа с датой и временем
        #region DateTime

        // устанавливаем дату для заказа, а также ограничиваем список для выбора. Нельзя сделать заказ после 20:00
        private void SetDatePickerLimits()
        {

                DateTime dateTime = DateTime.Now;
                TimeSpan nowTime = dateTime.TimeOfDay;
                if (nowTime > new TimeSpan(20, 0, 0))
                {
                    DeliveryDate.SelectedDate = (new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day + 1)); // установка начальной даты заказа
                    DeliveryDate.BlackoutDates.Add(new CalendarDateRange(DateTime.MinValue, DateTime.Today));
                }
                else
                {
                    DeliveryDate.SelectedDate = (new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)); // установка начальной даты заказа
                    DeliveryDate.BlackoutDates.Add(new CalendarDateRange(DateTime.MinValue, DateTime.Today.AddDays(-1)));
                }
        }

        // нельзя в дату и время вручную внести данные
        private void DeliveryDateAndTime(object sender, TextCompositionEventArgs e)
        {
            e.Handled = true; // Запрещаем ввод с клавиатуры
        }

        #endregion  

    }
}
