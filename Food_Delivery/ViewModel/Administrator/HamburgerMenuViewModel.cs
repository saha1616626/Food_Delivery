﻿using Food_Delivery.Helper;
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
            WorkingWithData._exitHamburgerMenu += TurnOffSideMenu;

            IsMenuButtonVisibility = true;
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

        // вызов события запуска страницы "заказы"
        private RelayCommand _openOrdersPageButton { get; set; }
        public RelayCommand OpenOrdersPageButton
        {
            get
            {
                return _openOrdersPageButton ??
                    (_openOrdersPageButton = new RelayCommand((obj) =>
                    {
                        // запускаем страницу "заказы" из класса MainMenuViewModel
                        WorkingWithData.OpenOrdersPage();
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

        private bool _isMenuButtonVisibility { get; set; } // видимость кнопки запуска меню
        public bool IsMenuButtonVisibility
        {
            get { return _isMenuButtonVisibility; }
            set { _isMenuButtonVisibility = value; OnPropertyChanged(nameof(IsMenuButtonVisibility)); }
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
            IsMenuButtonVisibility = IsSideMenuVisible ? false : true; // скрываем кнопку или показываем
        }

        // закрываем меню
        private void TurnOffSideMenu(object sender, EventAggregator e)
        {
            IsSideMenuVisible = false; // невидимое меню
            SideMenuWidth = 0; // изменяем ширину
            IsMenuButtonVisibility = IsSideMenuVisible ? false : true; // скрываем кнопку или показываем
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

