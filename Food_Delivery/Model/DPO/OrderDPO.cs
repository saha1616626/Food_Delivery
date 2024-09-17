using Food_Delivery.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Food_Delivery.Model.DPO
{
    public class OrderDPO : INotifyPropertyChanged
    {
        public int id { get; set; }
        private DateTime? _dateTime { get; set; }
        public DateTime? dateTime
        {
            get { return _dateTime; }
            set { _dateTime = value; OnPropertyChanged(nameof(dateTime)); }
        }
        private DateTime? _startDesiredDeliveryTime { get; set; }
        public DateTime? startDesiredDeliveryTime
        {
            get { return _startDesiredDeliveryTime; }
            set { _startDesiredDeliveryTime = value; OnPropertyChanged(nameof(startDesiredDeliveryTime)); }
        }
        private DateTime? _endDesiredDeliveryTime { get; set; }
        public DateTime? endDesiredDeliveryTime
        {
            get { return _endDesiredDeliveryTime; }
            set { _endDesiredDeliveryTime = value; OnPropertyChanged(nameof(endDesiredDeliveryTime)); }
        }
        private int? _accountId { get; set; }
        public int? accountId
        {
            get { return _accountId; }
            set { _accountId = value; OnPropertyChanged(nameof(accountId)); }
        }
        private int? _orderStatusId { get; set; }
        public int? orderStatusId
        {
            get { return _orderStatusId; }
            set { _orderStatusId = value; OnPropertyChanged(nameof(orderStatusId)); }
        }
        private string _statusName { get; set; }
        public string statusName
        {
            get { return _statusName; }
            set { _statusName = value; OnPropertyChanged(nameof(statusName)); }
        }
        private string _name { get; set; }
        public string name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged(nameof(name)); }
        }
        private string _surname { get; set; }
        public string surname
        {
            get { return _surname; }
            set { _surname = value; OnPropertyChanged(nameof(surname)); }
        }
        private string? _patronymic { get; set; }
        public string? patronymic
        {
            get { return _patronymic; }
            set { _patronymic = value; OnPropertyChanged(nameof(patronymic)); }
        }
        private string _city { get; set; }
        public string city
        {
            get { return _city; }
            set { _city = value; OnPropertyChanged(nameof(city)); }
        }
        private string _street { get; set; }
        public string street
        {
            get { return _street; }
            set { _street = value; OnPropertyChanged(nameof(street)); }
        }
        private string _house { get; set; }
        public string house
        {
            get { return _house; }
            set { _house = value; OnPropertyChanged(nameof(house)); }
        }
        private string? _apartment { get; set; }
        public string? apartment
        {
            get { return _apartment; }
            set { _apartment = value; OnPropertyChanged(nameof(apartment)); }
        }
        private string? _numberPhone { get; set; }
        public string? numberPhone
        {
            get { return _numberPhone; }
            set { _numberPhone = value; OnPropertyChanged(nameof(numberPhone)); }
        }
        private string? _email { get; set; }
        public string? email
        {
            get { return _email; }
            set { _email = value; OnPropertyChanged(nameof(email)); }
        }
        private int _costPrice { get; set; }
        public int costPrice
        {
            get { return _costPrice; }
            set { _costPrice = value; OnPropertyChanged(nameof(costPrice)); }
        }
        private string _typePayment { get; set; }
        public string typePayment
        {
            get { return _typePayment; }
            set { _typePayment = value; OnPropertyChanged(nameof(typePayment)); }
        }
        private int? _prepareChangeMoney { get; set; }
        public int? prepareChangeMoney
        {
            get { return _prepareChangeMoney; }
            set { _prepareChangeMoney = value; OnPropertyChanged(nameof(prepareChangeMoney)); }
        }

        // массив товаров в заказе
        private List<CompositionOrder> _compositionOrder { get; set; }
        public List<CompositionOrder> compositionOrder
        {
            get { return _compositionOrder; }
            set { _compositionOrder = value; OnPropertyChanged(nameof(compositionOrder)); }
        }

        public OrderDPO(int id, DateTime? dateTime, DateTime? startDesiredDeliveryTime,
            DateTime? endDesiredDeliveryTime, int? accountId, int? orderStatusId,
            string statusName, string name, string surname, string patronymic, string city, string street,
            string house, string apartment, string numberPhone, string email, int costPrice, string typePayment,
            int prepareChangeMoney, List<CompositionOrder> compositionOrder)
        {
            this.id = id;
            this.dateTime = dateTime;
            this.startDesiredDeliveryTime = startDesiredDeliveryTime;
            this.endDesiredDeliveryTime = endDesiredDeliveryTime;
            this.accountId = accountId;
            this.orderStatusId = orderStatusId;
            this.statusName = statusName;
            this.name = name;
            this.surname = surname;
            this.patronymic = patronymic;
            this.city = city;
            this.street = street;
            this.house = house;
            this.apartment = apartment;
            this.numberPhone = numberPhone;
            this.email = email;
            this.costPrice = costPrice;
            this.typePayment = typePayment;
            this.prepareChangeMoney = prepareChangeMoney;
            this.compositionOrder = compositionOrder;
        }

        public OrderDPO() { }

        // получаем заказ из Order с заменой id и детализацией всех блюд в заказе
        public async Task<OrderDPO> CopyFromCompositionCart(Order order)
        {
            OrderDPO orderDPO = new OrderDPO();

            orderDPO.id = order.id;
            if (order.dateTime != null)
            {
                orderDPO.dateTime = order.dateTime;
            }
            if (order.startDesiredDeliveryTime != null)
            {
                orderDPO.startDesiredDeliveryTime = order.startDesiredDeliveryTime;
            }
            if (order.endDesiredDeliveryTime != null)
            {
                orderDPO.endDesiredDeliveryTime = order.endDesiredDeliveryTime;
            }
            if (order.accountId != null)
            {
                orderDPO.accountId = order.accountId;
            }
            if (order.name != null)
            {
                orderDPO.name = order.name;
            }
            if (order.surname != null)
            {
                orderDPO.surname = order.surname;
            }
            if (order.patronymic != null)
            {
                orderDPO.patronymic = order.patronymic;
            }
            if (order.city != null)
            {
                orderDPO.city = order.city;
            }
            if (order.street != null)
            {
                orderDPO.street = order.street;
            }
            if (order.house != null)
            {
                orderDPO.house = order.house;
            }
            if (order.apartment != null)
            {
                orderDPO.apartment = order.apartment;
            }
            if (order.numberPhone != null)
            {
                orderDPO.numberPhone = order.numberPhone;
            }
            if (order.email != null)
            {
                orderDPO.email = order.email;
            }

            orderDPO.costPrice = order.costPrice;
            orderDPO.typePayment = order.typePayment;

            if(order.prepareChangeMoney != null)
            {
                orderDPO.prepareChangeMoney = order.prepareChangeMoney;
            }

            // получаем список товаров заказа
            using (FoodDeliveryContext foodDeliveryContext = new FoodDeliveryContext())
            {
                List<OrderStatus> orderStatuses = await foodDeliveryContext.OrderStatus.ToListAsync(); // получаем список статусов заказов
                List<CompositionOrder> compositionOrders = await foodDeliveryContext.CompositionOrders.ToListAsync(); // получаем список блюд
                List<CompositionOrder> dishesOrder = await Task.Run(() => compositionOrders.FindAll(d => d.orderId == order.id));
                if(dishesOrder.Count > 0)
                {
                    orderDPO.compositionOrder = dishesOrder;
                }

                if (order.orderStatusId != null)
                {
                    orderDPO.orderStatusId = order.orderStatusId;

                    OrderStatus status = new OrderStatus();
                    status = await Task.Run(() => orderStatuses.FirstOrDefault(s => s.id == order.id));
                    if(status != null)
                    {
                        orderDPO.statusName = status.name;
                    }
                }
            }

            return orderDPO;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
