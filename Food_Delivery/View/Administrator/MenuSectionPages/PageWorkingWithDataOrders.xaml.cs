﻿using Food_Delivery.Helper;
using Food_Delivery.ViewModel.Administrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using System.IO;
using Food_Delivery.Model.DPO;
using System.Collections.ObjectModel;
using Food_Delivery.Model;
using Food_Delivery.Data;
using Microsoft.EntityFrameworkCore;
using System.Windows.Media.Animation;

namespace Food_Delivery.View.Administrator.MenuSectionPages
{
    /// <summary>
    /// Interaction logic for PageWorkingWithDataOrders.xaml
    /// </summary>
    public partial class PageWorkingWithDataOrders : Page
    {
        private readonly WorkingWithDataOrdersViewModel _workingWithDataOrdersViewModel; // объект класса

        public PageWorkingWithDataOrders(bool IsAddData)
        {
            InitializeComponent();

            _workingWithDataOrdersViewModel = (WorkingWithDataOrdersViewModel)this.Resources["WorkingWithDataOrdersViewModel"];
            _workingWithDataOrdersViewModel.ChangingName(IsAddData); // передаём состояния работы страницы (добавление или редактирование данных
            _workingWithDataOrdersViewModel.InitializeAsync(ErrorInputPopup, (Storyboard)FindResource("FieldIllumination"), ClientName,
                ClientSurname, ClientPatronymic, ClientCity, ClientStreet, ClientHouse, ClientApartment, ClientNumberPhone, ClientEmail,
                DeliveryDate, StartDesiredDeliveryTime, EndDesiredDeliveryTime, AmountChange, StatusOrder, CostPrice);
        }

        #region Popup

        // кнопка + на товаре
        public void Btn_AddItems(object sender, RoutedEventArgs e)
        {
            // получаем элемент после нажатия на кнопку "+"
            var currentItem = (e.Source as FrameworkElement)?.DataContext as CompositionOrderDPO;
            if (currentItem != null) // если не пустой объект
            {
                // изменяем кол-во товара
                _workingWithDataOrdersViewModel.EditProductList(currentItem, true);
            }
        }

        // кнопка - на товаре
        public void Btn_DeleteItems(object sender, RoutedEventArgs e)
        {
            // получаем элемент после нажатия на кнопку "-"
            var currentItem = (e.Source as FrameworkElement)?.DataContext as CompositionOrderDPO;
            if (currentItem != null) // если не пустой объект
            {
                // изменяем кол-во товара
                _workingWithDataOrdersViewModel.EditProductList(currentItem, false);
            }
        }

        // кнопка добавить товар в список заказа
        private async void AddDishPopup(object sender, RoutedEventArgs e)
        {
            // получаем объект списка на котором была нажата кнопка "добавить"
            CompositionOrderDPO compositionOrderDPO = (e.Source as FrameworkElement)?.DataContext as CompositionOrderDPO;

            if (compositionOrderDPO != null) // если обект получен, то мы запускаем метод добавления товара в список заказов 
            {
                _workingWithDataOrdersViewModel.AddProductList(compositionOrderDPO);
            }
        }

        // после скрытия Poup изменяем фон
        private void PopupAddDishesOrder_Closed(object sender, EventArgs e)
        {
            DarkBackground.Visibility = Visibility.Collapsed;
        }

        // после того, как Popap был закрыт, мы оповещаем систему, что не надо запускать Popup после потери фокуса на приожении
        private void Window_LossOfFocus(object sender, MouseButtonEventArgs e)
        {
            // путь к json работа окна Popup
            string pathDataPopup = @"E:\3comm\Documents\Предметы\Курс 3.2\Курсовая\Приложение\Программа\Food_Delivery\Food_Delivery\Data\СheckPopup.json";

            var jsonData = new { popup = "" };
            // Преобразуем объект в JSON-строку
            string jsonText = JsonConvert.SerializeObject(jsonData);
            File.WriteAllText(pathDataPopup, jsonText);
        }
        #endregion

        // нельзя в дату и время вручную внести данные
        private void DeliveryDateAndTime(object sender, TextCompositionEventArgs e)
        {
            e.Handled = true;
        }

        // поиск блюда
        private void DishesSearch(object sender, TextChangedEventArgs e)
        {
            // получаем текст из поля при поиске данных
            var textInfo = sender as System.Windows.Controls.TextBox;
            if (textInfo != null)
            {
                _workingWithDataOrdersViewModel.HandlerTextBoxChanged(textInfo.Text);
            }
        }

        // закрываем "гамбургер" меню, если открыто, при нажатии на окно, но не на само меню
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WorkingWithData.ExitHamburgerMenu();
        }
    }
}
