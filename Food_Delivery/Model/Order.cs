using Food_Delivery.Model.DPO;
using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Food_Delivery.Model
{
    public class Order
    {
        public int id { get; set; }
        public DateTime? dateTime { get; set; }
        public DateTime? startDesiredDeliveryTime { get; set; }
        public DateTime? endDesiredDeliveryTime { get; set; }
        public int? accountId { get; set; }
        public int? orderStatusId { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string? patronymic { get; set; }
        public string city { get; set; }
        public string street { get; set; }
        public string house { get; set; }
        public string? apartment { get; set; }
        public string? numberPhone { get; set; }
        public string? email { get; set; }
        public int costPrice { get; set; }
        public string typePayment { get; set; }
        public int? prepareChangeMoney { get; set; }

        // устанваливаем внешний ключ на таблицу Account
        public virtual Account Account { get; set; } = null!;

        // устанваливаем внешний ключ на таблицу OrderStatus
        public virtual OrderStatus OrderStatus { get; set; } = null!;

        // связываем CompositionOrder и Order
        public virtual ICollection<CompositionOrder> CompositionOrders { get; set; } = new List<CompositionOrder>();


        public Order() { }

        public Order(int id, DateTime? dateTime, DateTime? startDesiredDeliveryTime, DateTime? endDesiredDeliveryTime,
            int? accountId, int? orderStatusId, string name, string surname,
            string? patronymic, string city, string street, string house, string? apartment, string? numberPhone,
            string? email, int coastPrice, string typePayment, int? changeFromAmount, Account account,
            ShoppingCart shoppingCart, OrderStatus orderStatus)
        {
            this.id = id;
            this.dateTime = dateTime;
            this.startDesiredDeliveryTime = startDesiredDeliveryTime;
            this.endDesiredDeliveryTime = endDesiredDeliveryTime;
            this.accountId = accountId;
            this.orderStatusId = orderStatusId;
            this.name = name;
            this.surname = surname;
            this.patronymic = patronymic;
            this.city = city;
            this.street = street;
            this.house = house;
            this.apartment = apartment;
            this.numberPhone = numberPhone;
            this.email = email;
            this.costPrice = coastPrice;
            this.typePayment = typePayment;
            this.prepareChangeMoney = changeFromAmount;
            Account = account;
            OrderStatus = orderStatus;
        }

        // получаем заказ из OrderDPO
        public async Task<Order> CopyFromOrderDPO(OrderDPO orderDPO)
        {
            Order order = new Order();
            order.id = orderDPO.id;
            if (orderDPO.dateTime != null)
            {
                order.dateTime = orderDPO.dateTime;
            }
            if (orderDPO.startDesiredDeliveryTime != null)
            {
                order.startDesiredDeliveryTime = orderDPO.startDesiredDeliveryTime;
            }
            if (orderDPO.endDesiredDeliveryTime != null)
            {
                order.endDesiredDeliveryTime = orderDPO.endDesiredDeliveryTime;
            }
            if (orderDPO.accountId != null)
            {
                order.accountId = orderDPO.accountId;
            }
            if (orderDPO.orderStatusId != null)
            {
                order.orderStatusId = orderDPO.orderStatusId;
            }
            if (orderDPO.name != null)
            {
                order.name = orderDPO.name;
            }
            if (orderDPO.surname != null)
            {
                order.surname = orderDPO.surname;
            }
            if(orderDPO.patronymic != null)
            {
                order.patronymic = orderDPO.patronymic;
            }
            if (orderDPO.city != null)
            {
                order.city = orderDPO.city;
            }
            if (orderDPO.street != null)
            {
                order.street = orderDPO.street;
            }
            if (orderDPO.house != null)
            {
                order.house = orderDPO.house;
            }
            if (orderDPO.apartment != null)
            {
                order.apartment = orderDPO.apartment;
            }
            if (orderDPO.numberPhone != null)
            {
                order.numberPhone = orderDPO.numberPhone;
            }
            if (orderDPO.email != null)
            {
                order.email = orderDPO.email;
            }
            order.costPrice = orderDPO.costPrice;
            if (orderDPO.typePayment != null)
            {
                order.typePayment = orderDPO.typePayment;
            }
            if (orderDPO.prepareChangeMoney != null)
            {
                order.prepareChangeMoney = orderDPO.prepareChangeMoney;
            }

            return order;
        }
    }
}
