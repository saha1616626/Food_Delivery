using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Food_Delivery.Model
{
    public class ShoppingCart
    {
        public int id { get; set; }

        public int? accountId { get; set; }

        public int costPrice { get; set; }

        // устанавливаем внешний ключ на таблицу Account
        public virtual Account Account { get; set; } = null!;

        // связываем CompositionCart и ShoppingCart (установка внешнего ключа)
        public virtual ICollection<CompositionCart> CompositionCarts { get; set; } = new List<CompositionCart>();

        // связываем Order и ShoppingCart (установка внешнего ключа)
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

        public ShoppingCart() { }

        public ShoppingCart(int id, int? accountId, int costPrice, Account account)
        {
            this.id = id;
            this.accountId = accountId;
            this.costPrice = costPrice;
            Account = account;
        }
    }
}
