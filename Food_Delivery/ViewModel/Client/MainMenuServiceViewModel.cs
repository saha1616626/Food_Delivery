using Food_Delivery.Helper;
using Food_Delivery.View.Client.MainPages;
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

namespace Food_Delivery.ViewModel.Client
{
    public class MainMenuServiceViewModel : INotifyPropertyChanged
    {
        public MainMenuServiceViewModel()
        {
            DarkBackground = Visibility.Collapsed; // скрываем фон корзины

            // при запуске меню отображаем страницу с товарами
            StartingHomePage();

            // подписываемся на событие - отображаем фон при запуске корзины
            WorkingWithData._backgroundForShopping += BackgroundForShopping;
        }

        // запуск страниц
        #region launchPage

        // запускаем страницу со списком товаров
        PageProduct pageProduct {  get; set; }
        private async Task StartingHomePage()
        {
            ClearMemoryAfterFrame(pageProduct);
            pageProduct = new PageProduct();
            FrameMainMenu = pageProduct;
        }

        #endregion

        // работа над корзиной
        #region shoppingCart

        // отображаем фон при запуске корзины
        public bool IsBackgroundDisplay = false; // переключение режима работы фона
        private async void BackgroundForShopping(object sender, EventAggregator e)
        {
            IsBackgroundDisplay = !IsBackgroundDisplay;

            DarkBackground = IsBackgroundDisplay ? Visibility.Visible : Visibility.Collapsed;

            // если фон скрыт, то обновляем список товаров
        }

        #endregion

        // основные свойства
        #region Features

        // фон для корзины
        private Visibility _darkBackground { get; set; }
        public Visibility DarkBackground
        {
            get { return _darkBackground; }
            set
            {
                _darkBackground = value;
                OnPropertyChanged(nameof(DarkBackground));
            }
        }

        // Page для запуска страницы
        private Page _frameMainMenu { get; set; }
        public Page FrameMainMenu
        {
            get { return _frameMainMenu; }
            set { _frameMainMenu = value; OnPropertyChanged(nameof(FrameMainMenu)); }
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

            if (FrameMainMenu != null)
            {
                // очистка фрейма
                FrameMainMenu.Content = null;
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
