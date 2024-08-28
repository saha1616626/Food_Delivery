using Food_Delivery.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Food_Delivery.ViewModel.Administrator
{
    class HamburgerMenuViewModel : INotifyPropertyChanged
    {
        public HamburgerMenuViewModel()
        {
            // подписываемся на событие закрытия "гамбургер" меню
            WorkingWithData._exitHamburgerMenu += ToggleSideMenu;
        }

        #region pageTransitionEvent

        // вызов события запуска страницы "категории"

        private RelayCommand _openCategoryPageButton { get; set; }
        public RelayCommand OpenCategoryPageButton
        {
            get
            {
                return _openCategoryPageButton ??
                    (_openCategoryPageButton = new RelayCommand((obj) =>
                    {
                        // запускаем страницу "категории" из класса MainMenuViewModel
                        WorkingWithData.OpenCategoryPage();

                    }, (obj) => true));
            }
        }

        // вызов события запуска страницы "блюда"
        private RelayCommand _openDishesPageButton { get; set; }
        public RelayCommand OpenDishesPageButton
        {
            get
            {
                return _openDishesPageButton ??
                    (_openDishesPageButton = new RelayCommand((obj) =>
                    {
                        // запускаем страницу "блюда" из класса MainMenuViewModel
                        WorkingWithData.OpenDishesPage();
                    }, (obj) => true));
            }
        }

        #endregion

        #region workHamburgerMenu

        // свойства отвечающие за роботу "гамбургер меню"
        private double _sideMenuWidth { get; set; } // ширина меню
        public double SideMenuWidth
        {
            get { return _sideMenuWidth; }
            set { _sideMenuWidth = value; OnPropertyChanged(nameof(SideMenuWidth)); }
        }

        private bool _isSideMenuVisible { get; set; } // видимость меню
        public bool IsSideMenuVisible
        {
            get { return _isSideMenuVisible; }
            set { _isSideMenuVisible = value; OnPropertyChanged(nameof(IsSideMenuVisible)); }
        }

        // запуск меню
        private RelayCommand _hamburgerButton { get; set; }
        public RelayCommand HamburgerButton
        {
            get
            {
                return _hamburgerButton ?? 
                    (_hamburgerButton = new RelayCommand((obj) =>
                    {
                        ToggleSideMenu();
                    }, (obj) => true));
            }
        }

        // работа меню
        private void ToggleSideMenu()
        {
            IsSideMenuVisible = !IsSideMenuVisible; // при каждом вызове меняем видимость
            SideMenuWidth = IsSideMenuVisible ? 200 : 0; // изменяем ширину
        }

        private void ToggleSideMenu(object sender, EventAggregator e)
        {
            IsSideMenuVisible = !IsSideMenuVisible; // при каждом вызове меняем видимость
            SideMenuWidth = IsSideMenuVisible ? 200 : 0; // изменяем ширину
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

