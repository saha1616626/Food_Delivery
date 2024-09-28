using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Food_Delivery.Model.DPO
{
    // класс для отображения группированного списка товаров
    public class ProductViewDPO : INotifyPropertyChanged
    {
        private string _name;
        public string name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    RaisePropertyChanged(nameof(name));
                }
            }
        }

        public List<DishesDPO> Disheses { get; set; }

        public ProductViewDPO()
        {
            Disheses = new List<DishesDPO>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
