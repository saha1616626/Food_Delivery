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

        public int accountId { get; set; }

        public decimal costPrice { get; set; }

        public bool isCartPaid { get; set; }

        // устанавливаем внешний ключ на таблицу Account
        public virtual Account Account { get; set; } = null!;

        // связываем CompositionCart и ShoppingCart (установка внешнего ключа)
        public virtual ICollection<CompositionCart> CompositionCarts { get; set; } = new List<CompositionCart>();

        public ShoppingCart() { }

        public ShoppingCart(int id, int accountId, decimal costPrice, bool isCartPaid, Account account)
        {
            this.id = id;
            this.accountId = accountId;
            this.costPrice = costPrice;
            this.isCartPaid = isCartPaid;
            Account = account;
        }
    }
}
