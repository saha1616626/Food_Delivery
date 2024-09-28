using Food_Delivery.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Food_Delivery.Model.DPO
{
    public class DishesDPO : INotifyPropertyChanged
    {
        public int id { get; set; }

        private string _name { get; set; }
        public string name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged(nameof(name)); }
        }

        private string _description { get; set; }
        public string description
        {
            get { return _description; }
            set { _description = value; OnPropertyChanged(nameof(description)); }
        }

        private int _categoryId { get; set; }
        public int categoryId
        {
            get { return _categoryId; }
            set { _categoryId = value; OnPropertyChanged(nameof(categoryId)); }
        }

        private string _categoryName { get; set; }
        public string categoryName
        {
            get { return _categoryName; }
            set { _categoryName = value; OnPropertyChanged(nameof(categoryName)); }
        }

        private int? _calories { get; set; }
        public int? calories
        {
            get { return _calories; }
            set { _calories = value; OnPropertyChanged(nameof(calories)); }
        }

        private int? _squirrels { get; set; }
        public int? squirrels
        {
            get { return _squirrels; }
            set { _squirrels = value; OnPropertyChanged(nameof(squirrels)); }
        }

        private int? _fats { get; set; }
        public int? fats
        {
            get { return _fats; }
            set { _fats = value; OnPropertyChanged(nameof(fats)); }
        }

        private int? _carbohydrates { get; set; }
        public int? carbohydrates
        {
            get { return _carbohydrates; }
            set { _carbohydrates = value; OnPropertyChanged(nameof(_carbohydrates)); }
        }

        private int? _weight { get; set; }
        public int? weight
        {
            get { return _weight; }
            set { _weight = value; OnPropertyChanged(nameof(weight)); }
        }

        private int? _quantity { get; set; }
        public int? quantity
        {
            get { return _quantity; }
            set { _quantity = value; OnPropertyChanged(nameof(quantity)); }
        }

        private int _price { get; set; }
        public int price
        {
            get { return _price; }
            set { _price = value; OnPropertyChanged(nameof(price)); }
        }

        private BitmapImage _image { get; set; }
        public BitmapImage image
        {
            get { return _image; }
            set { _image = value; OnPropertyChanged(nameof(image)); }
        }

        private bool _stopList { get; set; }
        public bool stopList
        {
            get { return _stopList; }
            set { _stopList = value; OnPropertyChanged(nameof(stopList)); }
        }

        // свойство отображения кнопок + и - на товаре
        private bool _isAddedToCart { get; set; }
        public bool IsAddedToCart
        {
            get { return _isAddedToCart; }
            set { _isAddedToCart = value; OnPropertyChanged(nameof(IsAddedToCart)); }
        }

        // кол-во товара в корзине
        private int _numberIitemsCart;
        public int numberIitemsCart
        {
            get { return _numberIitemsCart; }
            set
            {
                _numberIitemsCart = value;
                OnPropertyChanged(nameof(numberIitemsCart));
            }
        }

        // получаем блюда из Dishes с заменой id
        public async Task<DishesDPO> CopyFromDishes(Dishes dishes)
        {
            DishesDPO dishesDPO = new DishesDPO();

            dishesDPO.id = dishes.id;

            if (dishes.name != null)
            {
                dishesDPO.name = dishes.name;
            }

            if (dishes.description != null)
            {
                dishesDPO.description = dishes.description;
            }

            // поиск категорий
            using (FoodDeliveryContext foodDeliveryContext = new FoodDeliveryContext())
            {
                List<Category> categories = await foodDeliveryContext.Categories.ToListAsync();
                // ищем категорию присущую данному блюду
                Category category = categories.FirstOrDefault(c => c.id == dishes.categoryId);
                if (category != null)
                {
                    dishesDPO.categoryName = category.name;
                }
            }

            if (dishes.calories != null)
            {
                dishesDPO.calories = dishes.calories;
            }

            if (dishes.squirrels != null)
            {
                dishesDPO.squirrels = dishes.squirrels;
            }

            if (dishes.fats != null)
            {
                dishesDPO.fats = dishes.fats;
            }

            if (dishes.carbohydrates != null)
            {
                dishesDPO.carbohydrates = dishes.carbohydrates;
            }

            if (dishes.weight != null)
            {
                dishesDPO.weight = dishes.weight;
            }

            if (dishes.quantity != null)
            {
                dishesDPO.quantity = dishes.quantity;
            }

            if (dishes.price != null)
            {
                dishesDPO.price = dishes.price;
            }

            // преобразуем массив byte в изображение
            if (dishes.image != null)
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit(); // устанавливаем свойства объекта без инициализации
                bitmap.StreamSource = new MemoryStream(dishes.image);
                bitmap.EndInit(); // сообщаем, что объект может выполнить необходимые операции для заверешения инициализации

                dishesDPO.image = bitmap;
            }

            if (dishes.stopList != null)
            {
                dishesDPO.stopList = dishes.stopList;
            }

            return dishesDPO;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
