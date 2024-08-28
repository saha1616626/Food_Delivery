using Food_Delivery.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;
using System.Windows.Controls;
using Food_Delivery.Model;
using Food_Delivery.View.Administrator.MenuSectionPages;

namespace Food_Delivery.ViewModel.Administrator
{
    // главный класс для смены фреймов
    public class MainMenuViewModel
    {
        public MainMenuViewModel()
        {
            // подписываемся на событие запуска страницы "категории"
            WorkingWithData._openCategoryPage += OpenCategoryPage;
            // подписываемся на событие запуска страницы "блюда"
            WorkingWithData._openDishesPage += OpenDishesPage;
        }

        #region launchPage

        // класс "категории"
        PageCategory pageCategory {  get; set; }

        // запуск страницы "категории"
        private void OpenCategoryPage(object sender, EventAggregator e)
        {
            ClearMemoryAfterFrame(pageCategory); // очистка памяти
            pageCategory = new PageCategory();
            LaunchFrame.NavigationService.Navigate(pageCategory);
            // закрываем "гамбургер" меню
            WorkingWithData.ExitHamburgerMenu();
        }

        // класс "блюа"
        PageDishes pageDishes { get; set; }

        // запуск страницы "блюда"
        private void OpenDishesPage(object sender, EventAggregator e)
        {
            ClearMemoryAfterFrame(pageDishes); // очистка памяти
            pageDishes = new PageDishes();
            LaunchFrame.NavigationService.Navigate(pageDishes);
            // закрываем "гамбургер" меню
            WorkingWithData.ExitHamburgerMenu();
        }

        #endregion

        #region Features

        // ассинхронно передаём фрейм из MainMenuPage
        public async Task InitializeAsync(Frame openPage)
        {
            if(openPage != null)
            {
                LaunchFrame = openPage;
            }
        }

        // свойство для запуска фреймов страниц
        private Frame _launchFrame;
        public Frame LaunchFrame
        {
            get { return _launchFrame; }
            set
            {
                _launchFrame = value;
            }
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

            if (LaunchFrame != null)
            {
                // очистка фрейма
                LaunchFrame.Content = null;
            }
        }

        #endregion

    }
}
