using Microsoft.Identity.Client.NativeInterop;
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
    public class CompositionOrderDPO : INotifyPropertyChanged
    {
        public int id { get; set; }
        public int orderId { get; set; }
        public int? dishesId { get; set; }

        private string _nameDishes { get; set; }
        public string nameDishes
        {
            get { return _nameDishes; }
            set { _nameDishes = value; OnPropertyChanged(nameof(nameDishes)); }
        }
        private string _descriptionDishes { get; set; }
        public string descriptionDishes
        {
            get { return _descriptionDishes; }
            set { _descriptionDishes = value; OnPropertyChanged(nameof(descriptionDishes)); }
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

        private int _quantityInOrder { get; set; } // кол-во в заказе, а не в товаре
        public int QuantityInOrder
        {
            get { return _quantityInOrder; }
            set { _quantityInOrder = value; OnPropertyChanged(nameof(QuantityInOrder)); }
        }

        private int? _quantityInProduct { get; set; } // кол-во штук в товаре, а не кол-во товаров в заказе
        public int? QuantityInProduct
        {
            get { return _quantityInProduct; }
            set { _quantityInProduct = value; OnPropertyChanged(nameof(QuantityInProduct)); }
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

        // видимость кнопки добавить товар
        private bool _isAddDishButton { get; set; }
        public bool IsAddDishButton
        {
            get { return _isAddDishButton; }
            set { _isAddDishButton = value; OnPropertyChanged(nameof(IsAddDishButton)); }
        }

        // видиомость кнопок для изменения кол-во товара в заказе
        private bool _isEditDishButton { get; set; }
        public bool IsEditDishButton
        {
            get { return _isEditDishButton; }
            set { _isEditDishButton = value; OnPropertyChanged(nameof(IsEditDishButton)); }
        }

        // сумма денег по выбранному товару
        private int _amountProduct { get; set; }
        public int AmountProduct
        {
            get { return _amountProduct; }
            set { _amountProduct = value; OnPropertyChanged(nameof(AmountProduct)); }
        }

        // получаем блюда из Dishes
        public async Task<CompositionOrderDPO> CompositionOrder(Dishes dishes)
        {
            CompositionOrderDPO compositionOrderDPO = new CompositionOrderDPO();
            compositionOrderDPO.id = 0;
            compositionOrderDPO.orderId = 0;
            compositionOrderDPO.dishesId = dishes.id;
            compositionOrderDPO.nameDishes = dishes.name;
            compositionOrderDPO.descriptionDishes = dishes.description;
            if(dishes.calories != null)
            {
                compositionOrderDPO.calories = dishes.calories;
            }
            if (dishes.squirrels != null)
            {
                compositionOrderDPO.squirrels = dishes.squirrels;
            }
            if (dishes.fats != null)
            {
                compositionOrderDPO.fats = dishes.fats;
            }
            if (dishes.carbohydrates != null)
            {
                compositionOrderDPO.carbohydrates = dishes.carbohydrates;
            }
            if (dishes.weight != null)
            {
                compositionOrderDPO.weight = dishes.weight;
            }
            compositionOrderDPO.QuantityInOrder = 0;
            compositionOrderDPO.QuantityInProduct = dishes.quantity;
            compositionOrderDPO.price = dishes.price;

            // преобразуем массив byte в изображение
            if (dishes.image != null)
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit(); // устанавливаем свойства объекта без инициализации
                bitmap.StreamSource = new MemoryStream(dishes.image);
                bitmap.EndInit(); // сообщаем, что объект может выполнить необходимые операции для заверешения инициализации

                compositionOrderDPO.image = bitmap;
            }

            compositionOrderDPO.IsAddDishButton = true; // видимость кнопк добавить товар в список
            compositionOrderDPO.IsEditDishButton = false; // видиомость кнопок для изменения кол - во товара в заказе
            compositionOrderDPO.AmountProduct = 0; // сумма денег по выбранному товару

            return compositionOrderDPO;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
