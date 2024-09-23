using Food_Delivery.Data;
using Food_Delivery.Helper;
using Food_Delivery.Model;
using Food_Delivery.Model.DPO;
using Food_Delivery.View.Administrator.MenuSectionPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Animation;

namespace Food_Delivery.ViewModel.Administrator
{
    public class OrdersViewModel : INotifyPropertyChanged
    {
        public OrdersViewModel()
        {
            GetListOrders(); // получаем список заказов клиентов

            // подписываемся на событие закрытия страницы для редактирования или добавления данных
            WorkingWithData._closingCorkWithDataOrdersPage += ClosingCorkWithDataOrdersPage;

            // после получения фокуса данного приложения запукаем закрытый Popup
            WorkingWithData._launchPopupAfterReceivingFocusOrders += LaunchPopupAfterReceivingFocusOrders;

            DarkBackground = Visibility.Collapsed; // фон для Popup скрыт
        }

        // подготовка страницы
        #region PreparingPage

        // коллекция отображения данных в таблице
        private ObservableCollection<OrderDPO> _listOrders { get; set; } = new ObservableCollection<OrderDPO>();
        public ObservableCollection<OrderDPO> ListOrders
        {
            get { return _listOrders; }
            set { _listOrders = value; OnPropertyChanged(nameof(ListOrders)); }
        }

        // отображаем список заказов в таблице
        private async Task GetListOrders()
        {
            ListOrders.Clear(); // очищаем коллекцию перед заполнением

            using (FoodDeliveryContext foodDeliveryContext = new FoodDeliveryContext())
            {
                List<Order> orders = await foodDeliveryContext.Orders.ToListAsync();

                // храним список заказов после замены id
                List<OrderDPO> orderDPOs = new List<OrderDPO>(); 


                // заполняем таблицу
                foreach (Order item in orders)
                {
                    OrderDPO orderDPO = new OrderDPO();
                    // заманяем id 
                    orderDPO = await orderDPO.CopyFromOrder(item);

                    orderDPOs.Add(orderDPO);
                }

                // делаем сортировку по статусу заказа (новый -> обработка -> принят к доставке и тд)
                ListOrders = new ObservableCollection<OrderDPO>
                    (await Task.Run(() => orderDPOs
                    .OrderByDescending(o => o.statusName == "Новый заказ")
                    .ThenByDescending(o => o.statusName == "В обработке")
                    .ThenByDescending(o => o.statusName == "Готов")
                    .ThenByDescending(o => o.statusName == "Доставляется")
                    .ThenBy(o => o.statusName == "Доставлен")
                    .ThenBy(o => o.statusName == "Отменен")
                    .ThenBy(o => o.statusName == "Отклонен")
                    .ToList()));
            }
        }

        #endregion

        #region WorkingWithData

        // хранение состояния работы над данными
        private bool IsAddData {  get; set; } // true - добавление данных; false - редактирование данных

        // страница для добавления и редактирования данных
        PageWorkingWithDataOrders pageWorkingWithDataOrders { get; set; }

        // запускаем страницу для добавления данных
        private RelayCommand _btn_OpenPageToAddData { get; set; }
        public RelayCommand Btn_OpenPageToAddData
        {
            get
            {
                return _btn_OpenPageToAddData ??
                    (_btn_OpenPageToAddData = new RelayCommand(async (obj) =>
                    {
                        IsAddData = true; // изменяем режим работы Popup на режим добавления данных
                        WorkingWithData.ExitHamburgerMenu(); // закрываем, если открыто "гамбургер меню"
                        ClearMemoryAfterFrame(pageWorkingWithDataOrders); // очистка памяти перед запуском страницы
                        pageWorkingWithDataOrders = new PageWorkingWithDataOrders(IsAddData, SelectedOrder);
                        PageFrame = pageWorkingWithDataOrders; // запускаем страницу для добавления данных

                    }, (obj) => true));
            }
        }

        // запускаем страницу для редактирования данных
        private RelayCommand _btn_OpenPageToEditData { get; set; }
        public RelayCommand Btn_OpenPageToEditData
        {
            get
            {
                return _btn_OpenPageToEditData ??
                    (_btn_OpenPageToEditData = new RelayCommand(async (obj) =>
                    {
                        IsAddData = false; // изменяем режим работы Popup на режим редактирования данных
                        WorkingWithData.ExitHamburgerMenu(); // закрываем, если открыто "гамбургер меню"
                        ClearMemoryAfterFrame(pageWorkingWithDataOrders); // очистка памяти перед запуском страницы
                        pageWorkingWithDataOrders = new PageWorkingWithDataOrders(IsAddData, SelectedOrder);
                        PageFrame = pageWorkingWithDataOrders; // запускаем страницу для редактирования данных

                    }, (obj) => true));
            }
        }

        // закрываем страницу для редактирования или добавления данных
        public void ClosingCorkWithDataOrdersPage(object sender, EventAggregator e)
        {
            ClearMemoryAfterFrame(pageWorkingWithDataOrders); // очистка фрейма
            GetListOrders(); // обновляем список
        }

        #endregion

        #region Popup

        readonly string pathDataPopup = @"E:\3comm\Documents\Предметы\Курс 3.2\Курсовая\Приложение\Программа\Food_Delivery\Food_Delivery\Data\СheckPopup.json";

        // запускаем Popup для удаления данных
        private RelayCommand _btn_OpenPopupToDeleteData { get; set; }
        public RelayCommand Btn_OpenPopapToDeleteData
        {
            get
            {
                return _btn_OpenPopupToDeleteData ??
                    (_btn_OpenPopupToDeleteData = new RelayCommand((obj) =>
                    {
                        StartPoupAddOrders = true; // отображаем Popup
                        DarkBackground = Visibility.Visible; // показать фон
                        WorkingWithData.ExitHamburgerMenu(); // закрываем, если открыто "гамбургер меню"
                        // отображаем название категории перед удалением в Popup
                        if (SelectedOrder.dateTime != null)
                        {
                            DataOfOrderDeleted = "Заказ от: " + SelectedOrder.dateTime;
                        }

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

        // запускаем Popup (для удаления)
        private void LaunchPopupAfterReceivingFocusOrders(object sender, EventAggregator eventAggregator)
        {
            StartPoupAddOrders = true; // отображаем Popup
            DarkBackground = Visibility.Visible; // показать фон
            WorkingWithData.ExitHamburgerMenu(); // закрываем, если открыто "гамбургер меню"
        }

        // закрываем Popup
        private async Task ClosePopupWorkingWithData()
        {
            // Закрываем Popup
            StartPoupAddOrders = false;
            DarkBackground = Visibility.Collapsed; // скрываем фон
        }

        // удаление данных
        private RelayCommand _btn_DeleteData { get; set; }
        public RelayCommand Btn_DeleteData
        {
            get
            {
                return _btn_DeleteData ??
                    (_btn_DeleteData = new RelayCommand(async (obj) =>
                    {
                        using (FoodDeliveryContext foodDeliveryContext = new FoodDeliveryContext())
                        {
                            // ищем нужный заказ и товары заказа для удаления
                            List<CompositionOrder> orderList = await foodDeliveryContext.CompositionOrders.ToListAsync();
                            if(orderList != null)
                            {
                                List<CompositionOrder> resList = await Task.Run(() => orderList.Where(o => o.orderId == SelectedOrder.id).ToList());
                                if(resList != null)
                                {
                                    foreach(CompositionOrder order in resList)
                                    {
                                        foodDeliveryContext.CompositionOrders.Remove(order);
                                    }
                                }
                            }
                            await foodDeliveryContext.SaveChangesAsync(); // cохраняем изменения в базе данных 

                            List<Order> orders = await foodDeliveryContext.Orders.ToListAsync();
                            if (orders != null)
                            {
                                Order order = await Task.Run(() => orders.FirstOrDefault(o => o.id == SelectedOrder.id));
                                if(order != null)
                                {
                                    foodDeliveryContext.Orders.Remove(order);
                                }
                            }

                            await foodDeliveryContext.SaveChangesAsync(); // cохраняем изменения в базе данных 

                            ClosePopupWorkingWithData(); // закрываем Popup
                            GetListOrders(); // обновляем список
                        }
                    }, (obj) => true));
            }
        }

        #endregion

        // свойства
        #region Features

        public TextBlock AnimationErrorInput { get; set; } // текстовое поле для анимации текста ошибки

        // ассинхронно получаем информацию из OrdersPage
        public async Task InitializeAsync(TextBlock AnimationErrorInput)
        {
            if (AnimationErrorInput != null)
            {
                this.AnimationErrorInput = AnimationErrorInput;
            }
        }

        // отображение даты заказа в Popup для удаления данных
        private string _dataOfOrderDeleted { get; set; }
        public string DataOfOrderDeleted
        {
            get { return _dataOfOrderDeleted; }
            set { _dataOfOrderDeleted = value; OnPropertyChanged(nameof(DataOfOrderDeleted)); }
        }

        //  запуск Popup удаления товаров из заказа
        private bool _startPoupAddOrders { get; set; }
        public bool StartPoupAddOrders
        {
            get { return _startPoupAddOrders; }
            set
            {
                _startPoupAddOrders = value;
                OnPropertyChanged(nameof(StartPoupAddOrders));
            }
        }

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

        // свойство для вывода ошибки при поиске данных в таблице
        private string _errorInput { get; set; }
        public string ErrorInput
        {
            get { return _errorInput; }
            set { _errorInput = value; OnPropertyChanged(nameof(ErrorInput)); }
        }

        // выбранное блюдо
        private OrderDPO _selectedOrder { get; set; }
        public OrderDPO SelectedOrder
        {
            get { return _selectedOrder; }
            set { _selectedOrder = value; OnPropertyChanged(nameof(SelectedOrder)); 
                OnPropertyChanged(nameof(IsWorkButtonEnable)); }
        }

        // отображение кнопки "удалить" и "редакировать"
        private bool _isWorkButtonEnable { get; set; }
        public bool IsWorkButtonEnable
        {
            get { return SelectedOrder != null; } // если в таблице выбранн объект, то кнопки работают
            set { _isWorkButtonEnable = value; OnPropertyChanged(nameof(IsWorkButtonEnable)); }
        }

        // Page для запуска страницы
        private Page _pageFrame {  get; set; }
        public Page PageFrame
        {
            get { return _pageFrame; }
            set { _pageFrame = value; OnPropertyChanged(nameof(PageFrame)); }
        }

        // очистка фрейма (памяти)
        public void ClearMemoryAfterFrame(FrameworkElement element)
        {
            if (element != null)
            {
                // очистка всех привязанных элементов
                BindingOperations.ClearAllBindings(element);
                // очистка визуальных элементов
                element.Resources.Clear();
                // Очистка ссылки на предыдущий экземпляр
                element = null;
            }

            if (PageFrame != null)
            {
                // очистка фрейма
                PageFrame.Content = null;
            }
        }

        #endregion

        // поиск данных в таблице
        #region OrdersSearch

        // список для фильтров таблицы
        public ObservableCollection<OrderDPO> ListSearch { get; set; } = new ObservableCollection<OrderDPO>();

        public async Task HandlerTextBoxChanged(string searchByValue)
        {
            searchByValue = searchByValue.Trim(); // убираем пробелы
            if (!string.IsNullOrWhiteSpace(searchByValue))
            {
                await GetListOrders(); // обновляем список
                ListSearch.Clear(); // очищаем список поиска данных

                // объединяем дату заказа, время досатвки, имя и фамилию клиента для поиска
                foreach (OrderDPO item in ListOrders)
                {
                    string unification = item.dateTime.ToString().ToLower() + " " + item.startDesiredDeliveryTime.ToString().ToLower() + " " + 
                        item.endDesiredDeliveryTime.ToString().ToLower() + " " + item.statusName.ToString().ToLower() + " " + item.name.ToString().ToLower() + " " + 
                        item.surname.ToString().ToLower() + " " + item.numberPhone.ToString().ToLower();

                    bool dataExists = unification.Contains(searchByValue.ToLowerInvariant());

                    if (dataExists)
                    {
                        ListSearch.Add(item);
                    }
                }

                ListOrders.Clear(); // очищаем список перед заполнением
                ListOrders = ListSearch; // обновляем список

                if (ListSearch.Count == 0)
                {
                    // оповещениие об отсутствии данных
                    ErrorInput = "Заказ не найдена!"; // собщение об ошибке
                    BeginFadeAnimation(AnimationErrorInput); // анимация затухания ошибки
                }                
            }
            else
            {
                ListOrders.Clear();
                await GetListOrders(); // обновляем список
            }
        }

        #endregion

        // анимации
        #region Animation

        // анимация затухания текста
        private void BeginFadeAnimation(TextBlock textBlock)
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

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}