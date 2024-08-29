using Food_Delivery.Data;
using Food_Delivery.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Food_Delivery.ViewModel.Administrator
{
    class CategoryViewModel : INotifyPropertyChanged
    {
        public CategoryViewModel()
        {
            GetListCategory(); // вывод данных в таблицу
        }

        #region workWithTable

        // коллекция отображения данных в таблице
        private ObservableCollection<Category> _listCategory { get; set; } = new ObservableCollection<Category>();
        public ObservableCollection<Category> ListCategory
        {
            get { return _listCategory; }
            set { _listCategory = value; OnPropertyChanged(nameof(ListCategory)); }
        }

        // получаем данные из БД c последующим выводом в таблицу
        private async void GetListCategory()
        {
            ListCategory.Clear(); // очищаем коллекцию перед заполнением
            using(FoodDeliveryContext foodDeliveryContext = new FoodDeliveryContext())
            {
                List<Category> categories = await foodDeliveryContext.Categories.ToListAsync();
                foreach (Category category in categories)
                {
                    ListCategory.Add(category);
                }
            }
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
