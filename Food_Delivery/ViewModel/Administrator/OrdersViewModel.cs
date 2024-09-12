using Food_Delivery.Helper;
using Food_Delivery.View.Administrator.MenuSectionPages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Food_Delivery.ViewModel.Administrator
{
    public class OrdersViewModel : INotifyPropertyChanged
    {
        public OrdersViewModel()
        {

            // подписываемся на событие закрытия страницы для редактирования или добавления данных
            WorkingWithData._closingCorkWithDataOrdersPage += ClosingCorkWithDataOrdersPage;
        }

        #region WorkingWithData

        // хранение состояния работы над данными
        private bool IsAddData {  get; set; } // true - добавление данных; false - редактирование данных

        // страница для добавления и редактирования данных
        PageWorkingWithDataOrders pageWorkingWithDataOrders { get; set; }

        // запускаем Popup для добавления данных
        private RelayCommand _btn_OpenPopupToAddData { get; set; }
        public RelayCommand Btn_OpenPopupToAddData
        {
            get
            {
                return _btn_OpenPopupToAddData ??
                    (_btn_OpenPopupToAddData = new RelayCommand(async (obj) =>
                    {
                        IsAddData = true; // изменяем режим работы Popup на режим добавления данных
                        WorkingWithData.ExitHamburgerMenu(); // закрываем, если открыто "гамбургер меню"
                        ClearMemoryAfterFrame(pageWorkingWithDataOrders); // очистка памяти перед запуском страницы
                        pageWorkingWithDataOrders = new PageWorkingWithDataOrders(IsAddData);
                        PageFrame = pageWorkingWithDataOrders; // запускаем страницу для добавления данных

                    }, (obj) => true));
            }
        }

        // закрываем страницу для редактирования или добавления данных
        public void ClosingCorkWithDataOrdersPage(object sender, EventAggregator e)
        {
            ClearMemoryAfterFrame(pageWorkingWithDataOrders); // очистка фрейма
        }

        #endregion

        // свойства
        #region Features



        // Page для запуска страницы
        private Page _pageFrame {  get; set; }
        public Page PageFrame
        {
            get { return _pageFrame; }
            set { _pageFrame = value; OnPropertyChanged(nameof(PageFrame)); }
        }

        // очистка фрейма (памяти)
        public void ClearMemoryAfterFrame(FrameworkElement element)
        {
            if (element != null)
            {
                // очистка всех привязанных элементов
                BindingOperations.ClearAllBindings(element);
                // очистка визуальных элементов
                element.Resources.Clear();
                // Очистка ссылки на предыдущий экземпляр
                element = null;
            }

            if (PageFrame != null)
            {
                // очистка фрейма
                PageFrame.Content = null;
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