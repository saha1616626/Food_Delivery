using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Food_Delivery.Model.DPO
{
    public class ComposittionCartDPO : INotifyPropertyChanged
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
        private string _dishesName { get; set; }
        public string dishesName
        {
            get { return _dishesName; }
            set
            {
                _dishesName = value;
                OnPropertyChanged(nameof(dishesName));
            }
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
        //public async Task<CompositionCartDPO> CopyFromCompositionCart(CompositionCart)
        //{
        //    //ComposittionCartDPO 
        //} 

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
