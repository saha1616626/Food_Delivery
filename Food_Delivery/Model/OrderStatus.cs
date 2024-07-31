using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Food_Delivery.Model
{
    public class OrderStatus
    {
        public int id {  get; set; }

        public string name { get; set; }

        // связываем Order и OrderStatus (установка внешнего ключа)
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

        public OrderStatus() { }

        public OrderStatus(int id, string name)
        {
            this.id = id;
            this.name = name;
        }
    }
}
