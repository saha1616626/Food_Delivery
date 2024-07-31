using Food_Delivery.Model.DPO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Food_Delivery.Model
{
    public class Dishes
    {
        public int id { get; set; }

        public string name { get; set; }

        public string description { get; set; }

        public int categoryId { get; set; }

        public int? calories { get; set; }
        public int? squirrels { get; set; }
        public int? fats { get; set; }
        public int? carbohydrates { get; set; }
        public int? weight { get; set; }

        public int? quantity { get; set; }

        public int? price { get; set; }

        public byte[] image { get; set; }

        public bool stopList { get; set; }

        // устанавливаем внешний ключ на таблицу Category
        public virtual Category Category { get; set; } = null!;

        // связывем CompositionCart и Dishes
        public virtual ICollection<CompositionCart> CompositionCarts { get; set; } = new List<CompositionCart>();

        public Dishes() { }
        
        public Dishes(int id, string name, string description, int categoryId, int calories, int squirrels, int fats, int carbohydrates, int weight, int quantity, int price, byte[] image, bool stopList, Category category)
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.categoryId = categoryId;
            this.calories = calories;
            this.squirrels = squirrels;
            this.fats = fats;
            this.carbohydrates = carbohydrates;
            this.weight = weight;
            this.quantity = quantity;
            this.price = price;
            this.image = image;
            this.stopList = stopList;
            Category = category;
        }

        // получаем блюдо из DishesDPO
        public async Task<Dishes> CopyFromDishesDPO(DishesDPO dishesDPO)
        {
            Dishes dishes = new Dishes();

            dishes.id = dishesDPO.id;
            if(dishesDPO.name != null)
            {
                dishes.name = dishesDPO.name;
            }
            if(dishesDPO.description != null)
            {
                dishes.description = dishesDPO.description;
            }
            if(dishesDPO.categoryId != null)
            {
                dishes.categoryId = dishesDPO.categoryId;
            }
            if(dishesDPO.calories != null)
            {
                dishes.calories = dishesDPO.calories;
            }
            if(dishesDPO.squirrels != null)
            {
                dishes.squirrels = dishesDPO.squirrels;
            }
            if(dishesDPO.fats != null)
            {
                dishes.fats = dishesDPO.fats;
            }
            if(dishesDPO.carbohydrates != null)
            {
                dishes.carbohydrates = dishesDPO.carbohydrates;
            }
            if(dishesDPO.weight != null)
            {
                dishes.weight = dishesDPO.weight;
            }
            if(dishesDPO.quantity != null)
            {
                dishes.quantity = dishesDPO.quantity;
            }
            if(dishesDPO.price != null)
            {
                dishes.price = dishesDPO.price;
            }
            if(dishesDPO.image != null)
            {
                // преобразовываем изображение в массив байтов
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    PngBitmapEncoder encoder = new PngBitmapEncoder(); // кодируем в формат PNG
                    encoder.Frames.Add(BitmapFrame.Create(dishesDPO.image)); // преобразовываем полученное изображение в нужный формат
                    encoder.Save(memoryStream);
                    dishes.image = memoryStream.ToArray();

                }
            }

            if(dishesDPO.stopList != null)
            {
                dishes.stopList = dishesDPO.stopList;
            }

            return dishes;
        }
    }
}
