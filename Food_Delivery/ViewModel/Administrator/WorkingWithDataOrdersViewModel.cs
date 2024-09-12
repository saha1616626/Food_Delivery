using Food_Delivery.Data;
using Food_Delivery.Helper;
using Food_Delivery.Model;
using Food_Delivery.View.Administrator.MenuSectionPages;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Printing;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Food_Delivery.ViewModel.Administrator
{
    public class WorkingWithDataOrdersViewModel : INotifyPropertyChanged
    {
        public WorkingWithDataOrdersViewModel()
        {
            // подготовка полей
            SelectedStartTimeDelivery = new DateTime(1, 1, 1, 9, 0, 0, 0); // устанавливаем начальное время
            IsOptionCardSelected = true; // по умолчанию выбрана карта
            IsFieldVisibilityTypePayment = true; // делаем недоступное поле для ввода суммы сдачи, так как выбрана карта
            SelectedOrderStatus = "Новый заказ"; // при создании заказа по умолчанию ставим "Новый заказ"
            DarkBackground = Visibility.Collapsed; // фон для Popup скрыт

            // после получения фокуса данного приложения запукаем закрытый Popup
            WorkingWithData._launchPopupAfterReceivingFocusOrders += LaunchPopupAfterReceivingFocusOrders;
        }

        // подготовка страницы
        #region PreparingPage

        // изменяем названия страницы (редактируем или добавлем новые записи)
        public async Task ChangingName(bool IsAddData)
        {
            if (IsAddData) // добавление данных
            {
                HeadingPage = "Создание заказа";
            }
            else // редактирование данных
            {
                HeadingPage = "Изменение заказа";
            }
        }

        // возврат на страницу "заказы"
        private RelayCommand _btn_ReturnPreviousPage { get; set; }
        public RelayCommand Btn_ReturnPreviousPage
        {
            get
            {
                return _btn_ReturnPreviousPage ??
                    (_btn_ReturnPreviousPage = new RelayCommand(async (obj) =>
                    {
                        // вызываем событие перехода на страницу "заказы"
                        WorkingWithData.ClosingCorkWithDataOrdersPage();
                    }, (obj) => true));
            }
        }

        #endregion

        // путь к json работа окна Popup
        readonly string pathDataPopup = @"E:\3comm\Documents\Предметы\Курс 3.2\Курсовая\Приложение\Программа\Food_Delivery\Food_Delivery\Data\СheckPopup.json";

        // работа с товарами в заказе
        #region Popup

        private bool IsCheckAddAndEditOrDeleteFocus; // true - добавление, редактирование или удаление данных.
                                                // для удержания фокуса на приложении при переходе между окнами

        private RelayCommand _btn_AddDishes { get; set; }
        public RelayCommand Btn_AddDishes
        {
            get
            {
                return _btn_AddDishes ??
                    (_btn_AddDishes = new RelayCommand(async (obj) =>
                    {
                        DarkBackground = Visibility.Visible; // показать фон
                        StartPoupAddDishes = true; // запускаем Poup
                        IsCheckAddAndEditOrDeleteFocus = true; // режим добавления данных

                        NotificationOfThePopupLaunchJson(); // оповещаем JSON, чтомы запустили Popup
                    }, (obj) => true));
            }
        }

        // записываем в JSON, что мы запустили Popup данной страницы
        private void NotificationOfThePopupLaunchJson()
        {
            // передаём в JSON, что мы запустили Popup
            var jsonData = new { popup = "Orders" };
            // Преобразуем объект в JSON-строку
            string jsonText = JsonConvert.SerializeObject(jsonData);
            File.WriteAllText(pathDataPopup, jsonText);
        }

        // запускаем Popup (для редактирования или удаления)
        private void LaunchPopupAfterReceivingFocusOrders(object sender, EventAggregator eventAggregator)
        {
            if (IsCheckAddAndEditOrDeleteFocus) // если это добавление или редактирование
            {
                StartPoupAddDishes = true; // отображаем Popup
            }
            else // если это удаление данных 
            {
                
            }
            DarkBackground = Visibility.Visible; // показать фон
            WorkingWithData.ExitHamburgerMenu(); // закрываем, если открыто "гамбургер меню"
        }
        #endregion

        // свойства
        #region Features

        // фон для Popup
        private Visibility _darkBackground { get; set; }
        public Visibility DarkBackground
        {
            get { return _darkBackground; }
            set
            {
                _darkBackground = value;
                OnPropertyChanged(nameof(DarkBackground));
            }
        }

        //  запуск Popup добавления товаров в заказ
        private bool _startPoupAddDishes {  get; set; }
        public bool StartPoupAddDishes
        {
            get { return _startPoupAddDishes; }
            set {  _startPoupAddDishes = value; 
            OnPropertyChanged(nameof(StartPoupAddDishes)); }
        }

        // видимость кнопки "добавить товар в заказ"
        private bool _isAddDishes { get; set; }
        public bool IsAddDishes
        {
            get { return _isAddDishes; }
            set
            {
                _isAddDishes = value; OnPropertyChanged(nameof(IsAddDishes));
            }
        }

        // выбранный статус заказа
        private string _selectedOrderStatus { get; set; }
        public string SelectedOrderStatus
        {
            get { return _selectedOrderStatus; }
            set
            {
                _selectedOrderStatus = value; OnPropertyChanged(nameof(SelectedOrderStatus));
                // если заказ имеет один из статусов, то мы не даём нажать на кноку "добавить товар"
                if (SelectedOrderStatus == "Готов" || SelectedOrderStatus == "Доставляется" ||
                    SelectedOrderStatus == "Отменен" || SelectedOrderStatus == "Отклонен" || SelectedOrderStatus == "Доставлен")
                {
                    IsAddDishes = false;
                }
                else
                {
                    IsAddDishes = true; // можно добавить товар
                }
            }
        }

        // список статусов заказа
        private List<string> _optionsOrderStatus { get; set; }
        public List<string> OptionsOrderStatus
        {
            get
            {
                return _optionsOrderStatus = new List<string>
            { "Новый заказ", "В обработке", "Готов", "Доставляется", "Доставлен", "Отменен", "Отклонен" };
            }
            set
            {
                _optionsOrderStatus = value;
                OnPropertyChanged(nameof(OptionsOrderStatus));
            }
        }

        // оплата картой
        private bool _isOptionCardSelected { get; set; }
        public bool IsOptionCardSelected
        {
            get { return _isOptionCardSelected; }
            set
            {
                _isOptionCardSelected = value; OnPropertyChanged(nameof(IsOptionCardSelected));
                IsFieldVisibilityTypePayment = false;
            } // делаем поле для ввода сдачи недоступным
        }

        // оплата наличными
        private bool _isOptionCashSelected { get; set; }
        public bool IsOptionCashSelected
        {
            get { return _isOptionCashSelected; }
            set
            {
                _isOptionCashSelected = value; OnPropertyChanged(nameof(IsOptionCashSelected));
                IsFieldVisibilityTypePayment = true;
            } // делаем поле для ввода сдачи доступным
        }

        // видимость поля способа оплаты
        private bool _isFieldVisibilityTypePayment;
        public bool IsFieldVisibilityTypePayment
        {
            get { return _isFieldVisibilityTypePayment; }
            set
            {
                _isFieldVisibilityTypePayment = value;
                OnPropertyChanged(nameof(IsFieldVisibilityTypePayment));
            }
        }

        // сумма заказа
        private string _outCostPrice;
        public string OutCostPrice
        {
            get { return _outCostPrice; }
            set
            {
                _outCostPrice = value;
                OnPropertyChanged(nameof(OutCostPrice));
            }
        }

        // название страницы
        private string _headingPage { get; set; }
        public string HeadingPage
        {
            get { return _headingPage; }
            set { _headingPage = value; OnPropertyChanged(nameof(HeadingPage)); }
        }

        // дата доставки 
        private DatePicker _selectedDate { get; set; }
        public DatePicker SelectedDate
        {
            get { return _selectedDate; }
            set { _selectedDate = value; OnPropertyChanged(nameof(SelectedDate)); }
        }

        // начальное время доставки
        private DateTime _selectedStartTimeDelivery { get; set; }
        public DateTime SelectedStartTimeDelivery
        {
            get { return _selectedStartTimeDelivery; }
            set { _selectedStartTimeDelivery = value; OnPropertyChanged(nameof(SelectedStartTimeDelivery)); LimitationInitialDeliveryTime(); }
        }

        // конечное время доставки
        private DateTime _selectedEndTimeDelivery { get; set; }
        public DateTime SelectedEndTimeDelivery
        {
            get { return _selectedEndTimeDelivery; }
            set { _selectedEndTimeDelivery = value; OnPropertyChanged(nameof(SelectedEndTimeDelivery)); }
        }


        #endregion

        // работа выбора диапозона время и даты доставки
        #region DataTimeDelivry

        // настройка начального диапозона времени
        private async Task LimitationInitialDeliveryTime()
        {

            TimeSpan minTime = new TimeSpan(9, 0, 0);
            TimeSpan maxTime = new TimeSpan(20, 0, 0);
            TimeSpan currentTimeSpan = DateTime.Now.TimeOfDay; // получаем время сейчас

            if (SelectedStartTimeDelivery != null) // проверка, было ли выбранно время начального интервала доставки
            {
                TimeSpan selectedTime = SelectedStartTimeDelivery.TimeOfDay; // получаем время выбранное пользователем
                if (selectedTime < minTime) // если выбранное время меньше времени открытия доставки, то ставим
                                            // минимальную дату доставки, а также ближайшее время по конечному интервалу доставки
                {
                    SelectedStartTimeDelivery = new DateTime(1, 1, 1, (int)minTime.TotalHours, (int)minTime.Minutes, (int)minTime.Seconds);
                    SelectedEndTimeDelivery = new DateTime(1, 1, 1, 11, 0, 0, 0);
                }
                else if (selectedTime > maxTime) // если выбранное время больше времени закрытия ссервиса доставки, то ставим
                                                 // максимальную дату доставки, а также ближайшее время по конечному интервалу доставки
                {
                    SelectedStartTimeDelivery = new DateTime(1, 1, 1, (int)maxTime.TotalHours, (int)maxTime.Minutes, (int)maxTime.Seconds);
                    SelectedEndTimeDelivery = new DateTime(1, 1, 1, 22, 0, 0, 0);
                }
                else
                {
                    SelectedEndTimeDelivery = new DateTime(1, 1, 1, (int)selectedTime.TotalHours + 2, (int)selectedTime.Minutes, (int)selectedTime.Seconds);
                }
            }

        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
