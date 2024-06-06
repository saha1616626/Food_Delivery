using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Food_Delivery.Model
{
    public class Dishes
    {
        public int id { get; set; }

        public string name { get; set; } = null!;

        public string description { get; set; } = null!;

        public int categoryId { get; set; }

        public int? calories { get; set; } = null!;

        public decimal? weight { get; set; } = null!;

        public int? quantity { get; set; } = null!;

        public decimal? price { get; set; } = null!;

        public Blob image { get; set; }

        public bool stopList { get; set; }

        // устанавливаем внешний ключ на таблицу Category
        public virtual Category Category { get; set; } = null!;

        // связывем CompositionCart и Dishes
        public virtual ICollection<CompositionCart> Carts { get; set; } = new List<CompositionCart>();

        public Dishes() { }

        public Dishes(int id, string name, string description, int categoryId, int calories, decimal weight, int quantity, decimal price, Blob image, bool stopList, Category category)
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.categoryId = categoryId;
            this.calories = calories;
            this.weight = weight;
            this.quantity = quantity;
            this.price = price;
            this.image = image;
            this.stopList = stopList;
            Category = category;
        }
    }
}
