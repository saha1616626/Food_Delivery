﻿using Food_Delivery.Helper;
using Food_Delivery.ViewModel.Administrator;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Food_Delivery.View.Administrator.MenuSectionPages
{
    /// <summary>
    /// Interaction logic for PageUsers.xaml
    /// </summary>
    public partial class PageUsers : Page
    {
        private readonly UsersViewModel _usersViewModel; // объект класса
        public PageUsers()
        {
            InitializeComponent();
            _usersViewModel = (UsersViewModel)this.Resources["UsersViewModel"];
            _usersViewModel.InitializeAsync(ErrorInput, ErrorInputPopup, NameUser, NameSurname, NamePatronymic,
                ClientCity, ClientStreet, ClientHouse, ClientApartment, NameNumberPhone,
                nameEmail, CbRole, NameLogin, (PasswordBox)NamePassword, (PasswordBox)NameNewPassword, (Storyboard)FindResource("FieldIllumination"),
                ErrorInputData);
        }

        #region Popup

        // скрыть фон при скрытие popup
        private void MyPopup_Closed(object sender, EventArgs e)
        {
            DarkBackground.Visibility = Visibility.Collapsed;
        }

        // после того, как Popap был закрыт, мы оповещаем систему, что не надо запускать Popup после потери фокуса на приложении
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

        // поиск пользователя
        private void OrdersSearch(object sender, TextChangedEventArgs e)
        {
            // получаем текст из поля при поиске данных
            var textInfo = sender as System.Windows.Controls.TextBox;
            if (textInfo != null)
            {
                _usersViewModel.HandlerTextBoxChanged(textInfo.Text);
            }
        }

        // закрываем "гамбургер" меню, если открыто, при нажатии на окно, но не на меню
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WorkingWithData.ExitHamburgerMenu();
        }
    }
}
