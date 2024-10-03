using Food_Delivery.Data;
using Food_Delivery.Model.DPO;
using Food_Delivery.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Food_Delivery.ViewModel.Client
{
    public class CustomerOrdersViewModel : INotifyPropertyChanged
    {
        AuthorizationViewModel authorizationViewModel = new AuthorizationViewModel();
        public CustomerOrdersViewModel()
        {
            GetListOrders(); // получаем список заказов клиентов
        }

        // подготовка страницы
        #region PreparingPage

        // коллекция отображения данных в таблице
        private ObservableCollection<OrderDPO> _listOrders { get; set; } = new ObservableCollection<OrderDPO>();
        public ObservableCollection<OrderDPO> ListOrders
        {
            get { return _listOrders; }
            set { _listOrders = value; OnPropertyChanged(nameof(ListOrders)); }
        }

        // отображаем список заказов в таблице
        private async Task GetListOrders()
        {
            ListOrders.Clear(); // очищаем коллекцию перед заполнением

            using (FoodDeliveryContext foodDeliveryContext = new FoodDeliveryContext())
            {
                List<Order> orders = await foodDeliveryContext.Orders.ToListAsync();

                // храним список заказов после замены id
                List<OrderDPO> orderDPOs = new List<OrderDPO>();

                // получаем id пользователя
                int userId = await authorizationViewModel.WeGetIdUser();
                if (userId != 0)
                {
                    // заполняем таблицу
                    foreach (Order item in await foodDeliveryContext.Orders.Where(o => o.accountId == userId).ToListAsync())
                    {
                        OrderDPO orderDPO = new OrderDPO();
                        // заманяем id 
                        orderDPO = await orderDPO.CopyFromOrder(item);
                        if(orderDPO.statusName == "Новый заказ" || orderDPO.statusName == "Готов")
                        {
                            orderDPO.statusName = "В обработке";
                        }
                        orderDPOs.Add(orderDPO);
                    }
                }
                

                // делаем сортировку по статусу заказа (новый -> обработка -> принят к доставке и тд)
                ListOrders = new ObservableCollection<OrderDPO>
                    (await Task.Run(() => orderDPOs
                    .OrderByDescending(o => o.statusName == "Новый заказ")
                    .ThenByDescending(o => o.statusName == "В обработке")
                    .ThenByDescending(o => o.statusName == "Готов")
                    .ThenByDescending(o => o.statusName == "Доставляется")
                    .ThenBy(o => o.statusName == "Доставлен")
                    .ThenBy(o => o.statusName == "Отменен")
                    .ThenBy(o => o.statusName == "Отклонен")
                    .ToList()));
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
