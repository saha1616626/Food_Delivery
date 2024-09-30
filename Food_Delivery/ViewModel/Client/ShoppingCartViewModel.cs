﻿using Food_Delivery.Data;
using Food_Delivery.Helper;
using Food_Delivery.Model.DPO;
using Food_Delivery.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.IO;
using static System.Net.Mime.MediaTypeNames;

namespace Food_Delivery.ViewModel.Client
{
    public class ShoppingCartViewModel : INotifyPropertyChanged
    {
        AuthorizationViewModel authorizationViewModel = new AuthorizationViewModel();
        private readonly string pathShoppingCart = @"E:\3comm\Documents\Предметы\Курс 3.2\Курсовая\Приложение\Программа\Food_Delivery\Food_Delivery\Data\ShoppingCart\shoppingCart.json";


        public ShoppingCartViewModel()
        {
            GetListProduct(); // отображаем список товаров

            IsMenuButtonVisibility = true; // видимость кнопки корзина

            // подписываемся на событие - скрываем корзину
            WorkingWithData._exitShoppingCart += TurnOffSideMenu;
        }

        // отображение списка товаров
        #region DisplayingListProducts

        // обновляем список товаров после скрытия корзины
        private async void UpdatingListProducts(object sender, EventAggregator e)
        {
            GetListProduct();
        }

        // коллекция отображения списка товаров
        private ObservableCollection<CompositionCartDPO> _listCompositionCart { get; set; } = new ObservableCollection<CompositionCartDPO>();
        public ObservableCollection<CompositionCartDPO> ListCompositionCart
        {
            get { return _listCompositionCart; }
            set { _listCompositionCart = value; OnPropertyChanged(nameof(ListCompositionCart)); }
        }

        // отображаем список товаров
        private async Task GetListProduct()
        {
            ListCompositionCart.Clear(); // очищаем список

            using (FoodDeliveryContext foodDeliveryContext = new FoodDeliveryContext())
            {
                CostPrice = 0; // стоимость заказа
                // проверяем, гость или авторизованный пользователь
                string role = await authorizationViewModel.WeGetRoleUser();

                if (role != null)
                {
                    if (role == "Гость")
                    {
                        // чтение файла корзины
                        string jsonDataCart = File.ReadAllText(pathShoppingCart);
                        // получение товаров
                        List<CompositionCart>? cart = JsonConvert.DeserializeObject<List<CompositionCart>>(jsonDataCart);

                        if (cart.Any())
                        {
                            // корзина не пуста. Заполняем список
                            foreach (CompositionCart cartItem in cart)
                            {
                                CompositionCartDPO compositionCartDPO = new CompositionCartDPO();
                                compositionCartDPO = await compositionCartDPO.CopyFromCompositionCart(cartItem);
                                ListCompositionCart.Add(compositionCartDPO);
                                CostPrice += compositionCartDPO.quantity * compositionCartDPO.dishes.price;
                            }

                        }
                    }
                    else if (role == "Пользователь")
                    {

                    }

                    // изменяем работу кнопки "оформить заказ"
                    if (CostPrice == null || CostPrice == 0)
                    {
                        IsEnableButtonCostPrice = false;
                    }
                    else
                    {
                        IsEnableButtonCostPrice = true;
                    }

                    FinalPrice = CostPrice.ToString();
                }
            }
        }

        // проверка товара в корзине пользователя
        private async Task<bool> CheckingProductShoppingCart(DishesDPO dishesDPO)
        {
            // проверяем, гость или авторизованный пользователь
            string role = await authorizationViewModel.WeGetRoleUser();

            if (role != null)
            {
                if (role == "Гость")
                {
                    // чтение файла корзины
                    string jsonDataCart = File.ReadAllText(pathShoppingCart);
                    // получение товаров
                    List<CompositionCart>? cart = JsonConvert.DeserializeObject<List<CompositionCart>>(jsonDataCart);
                    if (cart.Any())
                    {
                        // корзина не пуста
                        // проверяем, есть ли выбранный товар в корзине
                        if (await Task.Run(() => cart.Any(c => c.dishesId == dishesDPO.id)))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (role == "Пользователь")
                {
                    // работает с БД, так как пользователь авторизовался
                    using (FoodDeliveryContext foodDeliveryContext = new FoodDeliveryContext())
                    {
                        // получаем id авторизованного пользователя
                        int userId = await authorizationViewModel.WeGetIdUser();

                        if (userId != null)
                        {
                            // ищем корзину пользователя
                            Model.ShoppingCart shoppingCart = await Task.Run(() => foodDeliveryContext.ShoppingCarts.FirstOrDefaultAsync(c => c.accountId == userId));

                            if (shoppingCart != null) // есть корзина
                            {
                                // проверка товара в корзине
                                CompositionCart compositionCart = await Task.Run(() => foodDeliveryContext.CompositionCarts.FirstOrDefaultAsync(c => c.dishesId == dishesDPO.id));
                                if (compositionCart != null) // если товар есть
                                {
                                    return true;
                                }
                                else
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        // получаем кол-во товара в корзине
        private async Task<int> WeGetQuantityProductBasket(DishesDPO dishesDPO)
        {
            // проверяем, гость или авторизованный Если гость, то добавляем данные в JSON, инчае в БД
            string role = await authorizationViewModel.WeGetRoleUser();

            if (role != null)
            {
                if (role == "Гость")
                {
                    // чтение файла корзины
                    string jsonDataCart = File.ReadAllText(pathShoppingCart);
                    // получение товаров
                    List<CompositionCart>? cart = JsonConvert.DeserializeObject<List<CompositionCart>>(jsonDataCart);
                    if (cart.Any())
                    {
                        // корзина не пуста
                        // проверяем, есть ли выбранный товар в корзине
                        if (await Task.Run(() => cart.Any(c => c.dishesId == dishesDPO.id)))
                        {
                            // товар есть в корзине
                            //получаем данный товар и изменяем кол-во
                            CompositionCart compositionCart = await Task.Run(() => cart.FirstOrDefault(c => c.dishesId == dishesDPO.id));
                            if (compositionCart != null)
                            {
                                return compositionCart.quantity;
                            }
                            else
                            {
                                return 0;
                            }
                        }
                        else
                        {
                            return 0;
                        }
                    }
                    else
                    {
                        return 0;
                    }
                }
                else if (role == "Пользователь")
                {
                    // работает с БД, так как пользователь авторизовался
                    using (FoodDeliveryContext foodDeliveryContext = new FoodDeliveryContext())
                    {
                        // получаем id авторизованного пользователя
                        int userId = await authorizationViewModel.WeGetIdUser();

                        if (userId != null)
                        {
                            // ищем корзину пользователя
                            Model.ShoppingCart shoppingCart = await Task.Run(() => foodDeliveryContext.ShoppingCarts.FirstOrDefaultAsync(c => c.accountId == userId));

                            if (shoppingCart != null) // есть корзина
                            {
                                // проверка товара в корзине
                                CompositionCart compositionCart = await Task.Run(() => foodDeliveryContext.CompositionCarts.FirstOrDefaultAsync(c => c.dishesId == dishesDPO.id));
                                if (compositionCart != null) // если товар есть
                                {
                                    return compositionCart.quantity;
                                }
                                else
                                {
                                    return 0;
                                }
                            }
                            else
                            {
                                return 0;
                            }
                        }
                        else
                        {
                            return 0;
                        }
                    }
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        #endregion

        // свойства отвечающие за роботу корзины
        #region workShoppingCart

        // работа кнопки для заказа
        private bool _isEnableButtonCostPrice { get; set; } // видимость меню
        public bool IsEnableButtonCostPrice
        {
            get { return _isEnableButtonCostPrice; }
            set
            {
                _isEnableButtonCostPrice = value; OnPropertyChanged(nameof(IsEnableButtonCostPrice));
            }
        }

        public int CostPrice { get; set; } // стоимость заказа

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


        // запуск корзины
        private RelayCommand _shoppingCartButton { get; set; }
        public RelayCommand ShoppingCartButton
        {
            get
            {
                return _shoppingCartButton ??
                    (_shoppingCartButton = new RelayCommand(async (obj) =>
                    {
                        await ToggleSideMenu();
                        WorkingWithData.BackgroundForShopping(); // отображаем фон
                        await GetListProduct(); // обновляем список
                    }, (obj) => true));
            }
        }


        // закрываем корзину
        private RelayCommand _closeShoppingCartButton { get; set; }
        public RelayCommand CloseShoppingCartButton
        {
            get
            {
                return _closeShoppingCartButton ??
                    (_closeShoppingCartButton = new RelayCommand(async (obj) =>
                    {
                        await ToggleSideMenu();
                        WorkingWithData.BackgroundForShopping(); // скрываем фон                                                                
                        WorkingWithData.UpdatingListProducts();  // обновляем список товаров
                    }, (obj) => true));
            }
        }

        // работа меню
        private async Task ToggleSideMenu()
        {
            IsSideMenuVisible = !IsSideMenuVisible; // при каждом вызове меняем видимость
            SideMenuWidth = IsSideMenuVisible ? 400 : 0; // изменяем ширину
            IsMenuButtonVisibility = IsSideMenuVisible ? false : true; // скрываем кнопку или показываем
        }

        // закрываем меню
        private async void TurnOffSideMenu(object sender, EventAggregator e)
        {
            IsSideMenuVisible = false; // невидимое меню
            SideMenuWidth = 0; // изменяем ширину
            IsMenuButtonVisibility = IsSideMenuVisible ? false : true; // скрываем кнопку или показываем
        }

        #endregion

        // свойства 
        #region Features

        // финальная цена
        private string _finalPrice { get; set; }
        public string FinalPrice
        {
            get { return _finalPrice; }
            set { _finalPrice = value; OnPropertyChanged(nameof(FinalPrice)); }
        }

        // свойство видимости кнопки "пользователи"
        private bool _isUserSettings { get; set; }
        public bool IsUserSettings
        {
            get { return _isUserSettings; }
            set { _isUserSettings = value; OnPropertyChanged(nameof(IsUserSettings)); }
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
