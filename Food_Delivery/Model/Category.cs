using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Food_Delivery.Model
{
    public class Category
    {
        public int id { get; set; }

        public string name { get; set; } = null!;

        public string description { get; set; } = null!;

        // связываем Dishes и Category (установка внешнего ключа)
        public virtual ICollection<Dishes> Dishes { get; set; } = new List<Dishes>();

        public Category() { }

        public Category(int id, string name, string description)
        {
            this.id = id;
            this.name = name;
            this.description = description;
        }
    }
}
