using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Food_Delivery.Model
{
    public class CompositionOrder
    {
        public int id {  get; set; }
        public int orderId { get; set; }
        public int? dishesId { get; set; }
        public string nameDishes { get; set; }
        public string? descriptionDishes { get; set; }
        public int? calories {  get; set; }
        public int? squirrels { get; set; }
        public int? fats { get; set; }
        public int? carbohydrates { get; set; }
        public int? weight { get; set; }
        public int? quantity { get; set; }
        public int price { get; set; }
        public byte[] image { get; set; }

        // устанавливаем внешний ключ на таблицу Order
        public virtual Order Order { get; set; } = null!;

        // устанавливаем внешний ключ на таблицу Dishes
        public virtual Dishes Dishes { get; set; }


        public CompositionOrder() { }   

        public CompositionOrder(int id, int orderId, int? dishesId, string nameDishes, string? descriptionDishes, int? calories, int? squirrels, int? fats, int? carbohydrates, int? weight, int? quantity, int price, byte[] image, Order order, Dishes dishes)
        {
            this.id = id;
            this.orderId = orderId;
            this.dishesId = dishesId;
            this.nameDishes = nameDishes;
            this.descriptionDishes = descriptionDishes;
            this.calories = calories;
            this.squirrels = squirrels;
            this.fats = fats;
            this.carbohydrates = carbohydrates;
            this.weight = weight;
            this.quantity = quantity;
            this.price = price;
            this.image = image;
            Order = order;
            Dishes = dishes;
        }
    }
}
