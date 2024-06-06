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
        public int id {  get; set; }

        public string dateTime { get; set; } = null!;

        public DateTime? desiredDeliveryTime { get; set; }

        public int accountId { get; set; }

        public int shoppingCartId { get; set; }

        public int orderStatusId { get; set; }

        public string? name { get; set; }

        public string? surname { get; set; }

        public string? patronymic { get; set; }

        public string? city { get; set; }

        public string? street { get; set; }

        public string? house { get; set; }

        public string? apartment { get; set; }

        public string? numberPhone { get; set; }

        public string? email { get; set; }

        public double? coastPrice { get; set; }

        public string? typePayment { get; set; }

        public double? changeFromAmount { get; set; }

        // устанваливаем внешний ключ на таблицу Account
        public virtual Account Account { get; set; } = null!;

        // устанваливаем внешний ключ на таблицу ShoppingCart

        public virtual ShoppingCart ShoppingCart { get; set; } = null!;

        // устанваливаем внешний ключ на таблицу OrderStatus
        public virtual OrderStatus OrderStatus { get; set; } = null!;

        public Order() { }

        public Order(int id, string dateTime, DateTime? desiredDeliveryTime, int accountId, int shoppingCartId, int orderStatusId, string? name, string? surname, string? patronymic, string? city, string? street, string? house, string? apartment, string? numberPhone, string? email, double? coastPrice, string? typePayment, double? changeFromAmount, Account account, ShoppingCart shoppingCart, OrderStatus orderStatus)
        {
            this.id = id;
            this.dateTime = dateTime;
            this.desiredDeliveryTime = desiredDeliveryTime;
            this.accountId = accountId;
            this.shoppingCartId = shoppingCartId;
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
            this.coastPrice = coastPrice;
            this.typePayment = typePayment;
            this.changeFromAmount = changeFromAmount;
            Account = account;
            ShoppingCart = shoppingCart;
            OrderStatus = orderStatus;
        }
    }
}
