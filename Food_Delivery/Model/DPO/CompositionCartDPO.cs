using Food_Delivery.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Food_Delivery.Model.DPO
{
    public class CompositionCartDPO : INotifyPropertyChanged
    {
        public int id { get; set; }
        private int _shoppingCartId { get; set; }
        public int shoppingCartId
        {
            get { return _shoppingCartId; }
            set
            {
                _shoppingCartId = value;
                OnPropertyChanged(nameof(shoppingCartId));
            }
        }
        private int _dishesId { get; set; }
        public int dishesId
        {
            get { return _dishesId; }
            set
            {
                _dishesId = value;
                OnPropertyChanged(nameof(dishesId));
            }
        }

        private Dishes _dishes { get; set; }
        public Dishes dishes
        {
            get { return _dishes; }
            set { _dishes = value; OnPropertyChanged(nameof(dishes)); }
        }

        private int _quantity { get; set; }
        public int quantity
        {
            get { return _quantity; }
            set
            {
                _quantity = value;
                OnPropertyChanged(nameof(quantity));
            }
        }

        // получаем блюдо из Dishes с заменой id
        public async Task<CompositionCartDPO> CopyFromCompositionCart(CompositionCart compositionCart)
        {
            CompositionCartDPO compositionCartDPO = new CompositionCartDPO();
            
            compositionCartDPO.id = compositionCart.id;
            compositionCartDPO.shoppingCartId = compositionCart.shoppingCartId;
            compositionCartDPO.dishesId = compositionCart.dishesId;

            // получаем блюдо данного товара в корзине
            using (FoodDeliveryContext foodDeliveryContext = new FoodDeliveryContext())
            {
                // ищем нужное блюдо
                Dishes dishes = await foodDeliveryContext.Dishes.FirstOrDefaultAsync(d => d.id == compositionCart.dishesId);
                if(dishes != null)
                {
                    compositionCartDPO.dishes = dishes;
                }
            }

            if(compositionCart.quantity != 0)
            {
                compositionCartDPO.quantity = compositionCart.quantity;
            }

            return compositionCartDPO;

        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
