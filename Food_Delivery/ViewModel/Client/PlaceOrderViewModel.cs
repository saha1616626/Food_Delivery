using Food_Delivery.Data;
using Food_Delivery.Helper;
using Food_Delivery.Model.DPO;
using Food_Delivery.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MaterialDesignThemes.Wpf;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows;

namespace Food_Delivery.ViewModel.Client
{
    public class PlaceOrderViewModel : INotifyPropertyChanged
    {
        AuthorizationViewModel authorizationViewModel = new AuthorizationViewModel();
        private readonly string pathShoppingCart = @"E:\3comm\Documents\Предметы\Курс 3.2\Курсовая\Приложение\Программа\Food_Delivery\Food_Delivery\Data\ShoppingCart\shoppingCart.json";

        public PlaceOrderViewModel()
        {
            GetListProduct(); // отображаем список товаров
        }

        // подготовка страницы
        #region PreparingPage

        public async Task ChangingName()
        {
            SelectedStartTimeDelivery = new DateTime((int)DateTime.Now.Year, (int)DateTime.Now.Month, (int)DateTime.Now.Day, 9, 0, 0, 0); // устанавливаем начальное время
            SelectedDate = (new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)); // установка начальной даты заказа
            IsOptionCardSelected = true; // по умолчанию выбрана карта
            IsFieldVisibilityTypePayment = true; // делаем недоступное поле для ввода суммы сдачи, так как выбрана карта
        }

        #endregion

        // отображение списка товаров
        #region DisplayingListProducts

        // коллекция отображения списка товаров
        private ObservableCollection<CompositionCartDPO> _listCompositionCart { get; set; } = new ObservableCollection<CompositionCartDPO>();
        public ObservableCollection<CompositionCartDPO> ListCompositionCart
        {
            get { return _listCompositionCart; }
            set { _listCompositionCart = value; OnPropertyChanged(nameof(ListCompositionCart)); }
        }

        // отображаем список товаров
        private async Task GetListProduct()
        {
            ListCompositionCart.Clear(); // очищаем список

            using (FoodDeliveryContext foodDeliveryContext = new FoodDeliveryContext())
            {
                CostPrice = 0; // стоимость заказа
                // проверяем, гость или авторизованный пользователь
                string role = await authorizationViewModel.WeGetRoleUser();

                if (role != null)
                {
                    if (role == "Гость")
                    {
                        // чтение файла корзины
                        string jsonDataCart = File.ReadAllText(pathShoppingCart);
                        // получение товаров
                        List<CompositionCart>? cart = JsonConvert.DeserializeObject<List<CompositionCart>>(jsonDataCart);

                        if (cart.Any())
                        {
                            // корзина не пуста. Заполняем список
                            foreach (CompositionCart cartItem in cart)
                            {
                                CompositionCartDPO compositionCartDPO = new CompositionCartDPO();
                                compositionCartDPO = await compositionCartDPO.CopyFromCompositionCart(cartItem);
                                ListCompositionCart.Add(compositionCartDPO);
                                CostPrice += compositionCartDPO.quantity * compositionCartDPO.dishes.price;
                            }

                        }
                    }
                    else if (role == "Пользователь")
                    {

                    }

                    FinalPrice = CostPrice.ToString();
                }
            }
        }

        #endregion

        // работа над заказом
        #region WorkingWithPlaceOrder

        // выход со страницы
        private RelayCommand _btn_ClosePageMakingOrder { get; set; }
        public RelayCommand Btn_ClosePageMakingOrder
        {
            get
            {
                return _btn_ClosePageMakingOrder ??
                    (_btn_ClosePageMakingOrder = new RelayCommand(async (obj) =>
                    {
                        // событие закрытия страницы
                        WorkingWithData.ClosingCheckoutPage();
                    }, (obj) => true));
            }
        }

        // создание заказа
        private RelayCommand _btn_MakingOrders { get; set; }
        public RelayCommand Btn_MakingOrders
        {
            get
            {
                return _btn_MakingOrders ??
                    (_btn_MakingOrders = new RelayCommand(async (obj) =>
                    {
                        // событие закрытия страницы
                        WorkingWithData.ClosingCheckoutPage();
                    }, (obj) => true));
            }
        }

        #endregion

        // свойства 
        #region Features

        public TextBox AnimationOutName { get; set; } // поле для ввода текста "имя клиента". Вывод подсветки поля
        public TextBox AnimationOutSurname { get; set; } // поле для ввода текста "фамилия клиента". Вывод подсветки поля
        public TextBox AnimationOutPatronymic { get; set; } // поле для ввода текста "отчество клиента". Вывод подсветки поля
        public TextBox AnimationOutCity { get; set; } // поле для ввода текста "город клиента". Вывод подсветки поля
        public TextBox AnimationOutStreet { get; set; } // поле для ввода текста "улица клиента". Вывод подсветки поля
        public TextBox AnimationOutHouse { get; set; } // поле для ввода текста "дом клиента". Вывод подсветки поля
        public TextBox AnimationOutApartment { get; set; } // поле для ввода текста "квартира клиента". Вывод подсветки поля
        public TextBox AnimationOutNumberPhone { get; set; } // поле для ввода текста "номер телефона клиента". Вывод подсветки поля
        public TextBox AnimationOutEmail { get; set; } // поле для ввода текста "email клиента". Вывод подсветки поля
        public DatePicker AnimationDeliveryDate { get; set; } // поле для ввода даты доставки. Вывод подсветки поля
        public TimePicker AnimationStartDesiredDeliveryTime { get; set; } // поле для ввода начала интервала доставки. Вывод подсветки поля
        public TimePicker AnimationEndDesiredDeliveryTime { get; set; } // поле для ввода конца интервала доставки. Вывод подсветки поля
        public TextBox AnimationAmountChange { get; set; } // поле для ввода текста "сумма сдачи". Вывод подсветки поля
        public TextBlock AnimationErrorInput { get; set; } // объект текстового поля. Анимация затухания текста после вывода сообщения.
        public Storyboard FieldIllumination { get; set; } // анимация объектов

        // ассинхронно получаем информацию из PageWorkingWithDataOrders 
        public async Task InitializeAsync( Storyboard FieldIllumination, TextBox AnimationOutName,
            TextBox AnimationOutSurname, TextBox AnimationOutPatronymic, TextBox AnimationOutCity, TextBox AnimationOutStreet,
            TextBox AnimationOuttHouse, TextBox AnimationOutApartment, TextBox AnimationOutNumberPhone, TextBox AnimationOutEmail,
            DatePicker AnimationDeliveryDate, TimePicker AnimationStartDesiredDeliveryTime, TimePicker AnimationEndDesiredDeliveryTime,
            TextBox AnimationAmountChange, TextBlock AnimationErrorInput)
        {
            if (FieldIllumination != null)
            {
                this.FieldIllumination = FieldIllumination;
            }
            if (AnimationOutName != null)
            {
                this.AnimationOutName = AnimationOutName;
            }
            if (AnimationOutSurname != null)
            {
                this.AnimationOutSurname = AnimationOutSurname;
            }
            if (AnimationOutPatronymic != null)
            {
                this.AnimationOutPatronymic = AnimationOutPatronymic;
            }
            if (AnimationOutCity != null)
            {
                this.AnimationOutCity = AnimationOutCity;
            }
            if (AnimationOutStreet != null)
            {
                this.AnimationOutStreet = AnimationOutStreet;
            }
            if (AnimationOuttHouse != null)
            {
                this.AnimationOutHouse = AnimationOuttHouse;
            }
            if (AnimationOutApartment != null)
            {
                this.AnimationOutApartment = AnimationOutApartment;
            }
            if (AnimationOutNumberPhone != null)
            {
                this.AnimationOutNumberPhone = AnimationOutNumberPhone;
            }
            if (AnimationOutEmail != null)
            {
                this.AnimationOutEmail = AnimationOutEmail;
            }
            if (AnimationDeliveryDate != null)
            {
                this.AnimationDeliveryDate = AnimationDeliveryDate;
            }
            if (AnimationStartDesiredDeliveryTime != null)
            {
                this.AnimationStartDesiredDeliveryTime = AnimationStartDesiredDeliveryTime;
            }
            if (AnimationEndDesiredDeliveryTime != null)
            {
                this.AnimationEndDesiredDeliveryTime = AnimationEndDesiredDeliveryTime;
            }
            if (AnimationAmountChange != null)
            {
                this.AnimationAmountChange = AnimationAmountChange;
            }
            if (AnimationErrorInput != null)
            {
                this.AnimationErrorInput = AnimationErrorInput;
            }
        }

        // финальная цена
        private string _finalPrice { get; set; }
        public string FinalPrice
        {
            get { return _finalPrice; }
            set { _finalPrice = value; OnPropertyChanged(nameof(FinalPrice)); }
        }

        public int CostPrice { get; set; } // стоимость заказа

        // выбор оплата картой
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

        // выбор оплата наличными
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

        // поле подготовить сдачу от суммы
        private string _outAmountChange { get; set; }
        public string OutAmountChange
        {
            get { return _outAmountChange; }
            set { _outAmountChange = value; OnPropertyChanged(nameof(OutAmountChange)); }
        }

        // дата доставки 
        private DateTime _selectedDate { get; set; }
        public DateTime SelectedDate
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

        // email клиента
        private string _outClientEmail { get; set; }
        public string OutClientEmail
        {
            get { return _outClientEmail; }
            set { _outClientEmail = value; OnPropertyChanged(nameof(OutClientEmail)); }
        }

        // номер телефона клиента
        private string _outClientNumberPhone { get; set; }
        public string OutClientNumberPhone
        {
            get { return _outClientNumberPhone; }
            set { _outClientNumberPhone = value; OnPropertyChanged(nameof(OutClientNumberPhone)); }
        }

        // квартира клиента
        private string _outClientApartment { get; set; }
        public string OutClientApartment
        {
            get { return _outClientApartment; }
            set { _outClientApartment = value; OnPropertyChanged(nameof(OutClientApartment)); }
        }

        // дом клиента
        private string _outClientHouse { get; set; }
        public string OutClientHouse
        {
            get { return _outClientHouse; }
            set { _outClientHouse = value; OnPropertyChanged(nameof(OutClientHouse)); }
        }

        // улица клиента
        private string _outClientStreet { get; set; }
        public string OutClientStreet
        {
            get { return _outClientStreet; }
            set { _outClientStreet = value; OnPropertyChanged(nameof(OutClientStreet)); }
        }

        // город клиента
        private string _outClientCity { get; set; }
        public string OutClientCity
        {
            get { return _outClientCity; }
            set { _outClientCity = value; OnPropertyChanged(nameof(OutClientCity)); }
        }

        // отчество клиента
        private string _outClientPatronymic { get; set; }
        public string OutClientPatronymic
        {
            get { return _outClientPatronymic; }
            set { _outClientPatronymic = value; OnPropertyChanged(nameof(OutClientPatronymic)); }
        }

        // фамилия клиента
        private string _outClientSurname { get; set; }
        public string OutClientSurname
        {
            get { return _outClientSurname; }
            set { _outClientSurname = value; OnPropertyChanged(nameof(OutClientSurname)); }
        }

        // имя клиента
        private string _outClientName { get; set; }
        public string OutClientName
        {
            get { return _outClientName; }
            set { _outClientName = value; OnPropertyChanged(nameof(OutClientName)); }
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
                    SelectedStartTimeDelivery = new DateTime((int)DateTime.Now.Year, (int)DateTime.Now.Month, (int)DateTime.Now.Day, (int)minTime.TotalHours, (int)minTime.Minutes, (int)minTime.Seconds);
                    SelectedEndTimeDelivery = new DateTime((int)DateTime.Now.Year, (int)DateTime.Now.Month, (int)DateTime.Now.Day, 11, 0, 0);
                }
                else if (selectedTime > maxTime) // если выбранное время больше времени закрытия ссервиса доставки, то ставим
                                                 // максимальную дату доставки, а также ближайшее время по конечному интервалу доставки
                {
                    SelectedStartTimeDelivery = new DateTime((int)DateTime.Now.Year, (int)DateTime.Now.Month, (int)DateTime.Now.Day, (int)maxTime.TotalHours, (int)maxTime.Minutes, (int)maxTime.Seconds);
                    SelectedEndTimeDelivery = new DateTime((int)DateTime.Now.Year, (int)DateTime.Now.Month, (int)DateTime.Now.Day, 22, 0, 0);
                }
                else
                {
                    SelectedEndTimeDelivery = new DateTime((int)DateTime.Now.Year, (int)DateTime.Now.Month, (int)DateTime.Now.Day, (int)selectedTime.TotalHours + 2, (int)selectedTime.Minutes, (int)selectedTime.Seconds);
                }
            }

        }

        #endregion

        // анимации
        #region Animation

        // анимация затухания текста
        private async Task BeginFadeAnimation(TextBlock textBlock)
        {
            textBlock.IsEnabled = true;
            textBlock.Opacity = 1.0;

            Storyboard storyboard = new Storyboard();
            DoubleAnimation fadeAnimation = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                Duration = TimeSpan.FromSeconds(1)
            };
            Storyboard.SetTargetProperty(fadeAnimation, new PropertyPath(TextBlock.OpacityProperty));
            storyboard.Children.Add(fadeAnimation);
            storyboard.Completed += (s, e) => textBlock.IsEnabled = false;
            storyboard.Begin(textBlock);
        }

        // запуск анимации подсвечивания объекта
        private void StartFieldIllumination(TextBox textBox)
        {
            FieldIllumination.Begin(textBox);
        }
        private void StartFieldIllumination(ComboBox comboBox)
        {
            FieldIllumination.Begin(comboBox);
        }
        private void StartFieldIllumination(DatePicker datePicker)
        {
            FieldIllumination.Begin(datePicker);
        }
        private void StartFieldIllumination(TimePicker timePicker)
        {
            FieldIllumination.Begin(timePicker);
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
