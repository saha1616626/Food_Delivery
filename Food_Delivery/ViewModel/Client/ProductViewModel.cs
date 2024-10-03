using Food_Delivery.Data;
using Food_Delivery.Helper;
using Food_Delivery.Model;
using Food_Delivery.Model.DPO;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Food_Delivery.ViewModel.Client
{
    public class ProductViewModel : INotifyPropertyChanged
    {
        AuthorizationViewModel authorizationViewModel = new AuthorizationViewModel();
        public ProductViewModel()
        {
            GetListProduct(); // отображаем список товаров

            // подписываемся на событие - обновляем список товаров
            WorkingWithData._updatingListProducts += UpdatingListProducts;
        }

        // отображение списка товаров
        #region DisplayingListProducts

        // обновляем список товаров после скрытия корзины
        private async void UpdatingListProducts(object sender, EventAggregator e)
        {
            GetListProduct();
        }

        // коллекция отображения списка товаров
        private ObservableCollection<ProductViewDPO> _listProductView { get; set; } = new ObservableCollection<ProductViewDPO>();
        public ObservableCollection<ProductViewDPO> ListProductView
        {
            get { return _listProductView; }
            set { _listProductView = value; OnPropertyChanged(nameof(ListProductView)); }
        }

        // отображаем список товаров
        private async Task GetListProduct()
        {
            ListProductView.Clear(); // очищаем список

            using (FoodDeliveryContext foodDeliveryContext = new FoodDeliveryContext())
            {
                // проходимся по всем категориям
                foreach (var itemCategory in await foodDeliveryContext.Categories.ToListAsync())
                {
                    // проверяем, есть ли товары в данной категории
                    List<Dishes> dishes = await foodDeliveryContext.Dishes.Where(d => d.categoryId == itemCategory.id).ToListAsync();
                    if (dishes.Any())
                    {
                        // если товары есть
                        // Задаем новую группу товаров
                        ProductViewDPO productViewDPO = new ProductViewDPO();
                        productViewDPO.name = itemCategory.name;

                        // добавляем найденные товары в данную группу
                        foreach (var itemDishes in dishes)
                        {
                            // проверка на стоп лист
                            if (!itemDishes.stopList)
                            {

                                DishesDPO dishesDPO = new DishesDPO();
                                dishesDPO = await dishesDPO.CopyFromDishes(itemDishes);

                                // если товар есть у пользователя в корзине, то мы отображаем кнопки изменения кол-во в корзине
                                if (await CheckingProductShoppingCart(dishesDPO))
                                {
                                    // если есть товар
                                    dishesDPO.IsAddedToCart = true;
                                }
                                else
                                {
                                    dishesDPO.IsAddedToCart = false;
                                }

                                // изменяем кол-во товара в корзине
                                dishesDPO.numberIitemsCart = await WeGetQuantityProductBasket(dishesDPO);

                                productViewDPO.Disheses.Add(dishesDPO);
                            }
                        }

                        // добавляем товар в список
                        ListProductView.Add(productViewDPO);
                    }
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

        // работа над товарами в общем списке
        #region WorkingWithProducts

        private readonly string pathShoppingCart = @"E:\3comm\Documents\Предметы\Курс 3.2\Курсовая\Приложение\Программа\Food_Delivery\Food_Delivery\Data\ShoppingCart\shoppingCart.json";

        // добавление товара в корзину
        public async Task AddItemToShoppingCart(DishesDPO dishesDPO)
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
                                compositionCart.quantity += 1;

                                // обновляем список с отображаемыми товарами
                                ProductViewDPO product = await Task.Run(()=> ListProductView.FirstOrDefault(p => p.Disheses.Any(d => d.id == dishesDPO.id)));
                                if (product != null)
                                {
                                    // ищем нужный товар в группе товаров
                                    DishesDPO dishes = await Task.Run(() => product.Disheses.FirstOrDefault(d => d.id == dishesDPO.id));
                                    if(dishes != null)
                                    {
                                        dishes.numberIitemsCart += 1;
                                    }
                                }
                            }
                        }
                        else
                        {
                            // товара нет в корзине. Добавляем товар
                            cart.Add(new CompositionCart
                            {
                                id = cart.Count + 1,
                                shoppingCartId = 0,
                                dishesId = dishesDPO.id,
                                quantity = 1
                            });

                            // обновляем список с отображаемыми товарами
                            ProductViewDPO product = await Task.Run(() => ListProductView.FirstOrDefault(p => p.Disheses.Any(d => d.id == dishesDPO.id)));
                            if (product != null)
                            {
                                // ищем нужный товар в группе товаров
                                DishesDPO dishes = await Task.Run(() => product.Disheses.FirstOrDefault(d => d.id == dishesDPO.id));
                                if (dishes != null)
                                {
                                    dishes.numberIitemsCart += 1;
                                    dishes.IsAddedToCart = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        // товара нет в корзине. Добавляем товар
                        cart.Add(new CompositionCart
                        {
                            id = cart.Count + 1,
                            shoppingCartId = 0,
                            dishesId = dishesDPO.id,
                            quantity = 1
                        });

                        // обновляем список с отображаемыми товарами
                        ProductViewDPO product = await Task.Run(() => ListProductView.FirstOrDefault(p => p.Disheses.Any(d => d.id == dishesDPO.id)));
                        if (product != null)
                        {
                            // ищем нужный товар в группе товаров
                            DishesDPO dishes = await Task.Run(() => product.Disheses.FirstOrDefault(d => d.id == dishesDPO.id));
                            if (dishes != null)
                            {
                                dishes.numberIitemsCart += 1;
                                dishes.IsAddedToCart = true;
                            }
                        }
                    }

                    string updatedJsonData = ""; // обновленный JSON
                                                 // Сериализация объектов обновленной коллекции в JSON
                    updatedJsonData = JsonConvert.SerializeObject(cart, Formatting.Indented);
                    // запись в JSON файл обновленных данных
                    File.WriteAllText(pathShoppingCart, updatedJsonData);
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
                                    compositionCart.quantity += 1;
                                    shoppingCart.costPrice += dishesDPO.price; // изменяем стоимость корзины

                                    // сохраняем обновленные данные
                                    await foodDeliveryContext.SaveChangesAsync();

                                    // обновляем список с отображаемыми товарами
                                    ProductViewDPO product = await Task.Run(() => ListProductView.FirstOrDefault(p => p.Disheses.Any(d => d.id == dishesDPO.id)));
                                    if (product != null)
                                    {
                                        // ищем нужный товар в группе товаров
                                        DishesDPO dishes = await Task.Run(() => product.Disheses.FirstOrDefault(d => d.id == dishesDPO.id));
                                        if (dishes != null)
                                        {
                                            dishes.numberIitemsCart += 1;
                                            dishes.IsAddedToCart = true;
                                        }
                                    }
                                }
                                else // если товара нету в корзине
                                {
                                    // добавляем товар в корзину
                                    CompositionCart newDishesCompositionCart = new CompositionCart();
                                    newDishesCompositionCart.shoppingCartId = shoppingCart.id;
                                    newDishesCompositionCart.dishesId = dishesDPO.id;
                                    newDishesCompositionCart.quantity = 1;

                                    shoppingCart.costPrice += dishesDPO.price; // изменяем стоимость корзины

                                    await foodDeliveryContext.CompositionCarts.AddAsync(newDishesCompositionCart);
                                    // сохраняем обновленные данные
                                    await foodDeliveryContext.SaveChangesAsync();

                                    // обновляем список с отображаемыми товарами
                                    ProductViewDPO product = await Task.Run(() => ListProductView.FirstOrDefault(p => p.Disheses.Any(d => d.id == dishesDPO.id)));
                                    if (product != null)
                                    {
                                        // ищем нужный товар в группе товаров
                                        DishesDPO dishes = await Task.Run(() => product.Disheses.FirstOrDefault(d => d.id == dishesDPO.id));
                                        if (dishes != null)
                                        {
                                            dishes.numberIitemsCart += 1;
                                            dishes.IsAddedToCart = true;
                                        }
                                    }
                                }
                            }
                            else // нет корзины
                            {
                                // создаем корзину и добавляем туда товар
                                Model.ShoppingCart shoppingCarts = new Model.ShoppingCart();
                                shoppingCarts.accountId = userId;
                                shoppingCarts.costPrice = dishesDPO.price;

                                await foodDeliveryContext.ShoppingCarts.AddAsync(shoppingCarts);
                                // сохраняем обновленные данные
                                await foodDeliveryContext.SaveChangesAsync();

                                // добавляем товар в корзину
                                CompositionCart compositionCart = new CompositionCart();
                                compositionCart.shoppingCartId = (int)shoppingCarts.id; // берём id созданной корзины
                                compositionCart.dishesId = dishesDPO.id;
                                compositionCart.quantity = 1;

                                await foodDeliveryContext.CompositionCarts.AddAsync(compositionCart);
                                // сохраняем обновленные данные
                                await foodDeliveryContext.SaveChangesAsync();

                                // обновляем список с отображаемыми товарами
                                ProductViewDPO product = await Task.Run(() => ListProductView.FirstOrDefault(p => p.Disheses.Any(d => d.id == dishesDPO.id)));
                                if (product != null)
                                {
                                    // ищем нужный товар в группе товаров
                                    DishesDPO dishes = await Task.Run(() => product.Disheses.FirstOrDefault(d => d.id == dishesDPO.id));
                                    if (dishes != null)
                                    {
                                        dishes.numberIitemsCart += 1;
                                        dishes.IsAddedToCart = true;
                                    }
                                }
                            }
                        }

                    }
                }
            }
        }

        // изменение товар в корзине в меньшую сторону
        public async Task RemoveItemToShoppingCart(DishesDPO dishesDPO)
        {
            // проверяем, гость или авторизованный пользователь. Если гость, то добавляем данные в JSON, инчае в БД
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
                                if (compositionCart.quantity < 1 || compositionCart.quantity == 1) // если товара меньше или равен единицы, то мы его удаляем из списка
                                {
                                    cart.Remove(compositionCart);

                                    // обновляем список с отображаемыми товарами
                                    ProductViewDPO product = await Task.Run(() => ListProductView.FirstOrDefault(p => p.Disheses.Any(d => d.id == dishesDPO.id)));
                                    if (product != null)
                                    {
                                        // ищем нужный товар в группе товаров
                                        DishesDPO dishes = await Task.Run(() => product.Disheses.FirstOrDefault(d => d.id == dishesDPO.id));
                                        if (dishes != null)
                                        {
                                            dishes.numberIitemsCart = 0;
                                            dishes.IsAddedToCart = false;
                                        }
                                    }
                                }
                                else // инчае изменяем кол-во
                                {
                                    compositionCart.quantity -= 1;

                                    // обновляем список с отображаемыми товарами
                                    ProductViewDPO product = await Task.Run(() => ListProductView.FirstOrDefault(p => p.Disheses.Any(d => d.id == dishesDPO.id)));
                                    if (product != null)
                                    {
                                        // ищем нужный товар в группе товаров
                                        DishesDPO dishes = await Task.Run(() => product.Disheses.FirstOrDefault(d => d.id == dishesDPO.id));
                                        if (dishes != null)
                                        {
                                            dishes.numberIitemsCart -= 1;
                                        }
                                    }
                                }

                                string updatedJsonData = ""; // обновленный JSON
                                // Сериализация объектов обновленной коллекции в JSON
                                updatedJsonData = JsonConvert.SerializeObject(cart, Formatting.Indented);
                                // запись в JSON файл обновленных данных
                                File.WriteAllText(pathShoppingCart, updatedJsonData);
                            }
                        }
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
                                    if (compositionCart.quantity < 1 || compositionCart.quantity == 1) // если товара меньше или равен единицы, то мы его удаляем из списка
                                    {
                                        foodDeliveryContext.CompositionCarts.Remove(compositionCart);
                                        shoppingCart.costPrice -= dishesDPO.price; // изменяем стоимость корзины

                                        // обновляем список с отображаемыми товарами
                                        ProductViewDPO product = await Task.Run(() => ListProductView.FirstOrDefault(p => p.Disheses.Any(d => d.id == dishesDPO.id)));
                                        if (product != null)
                                        {
                                            // ищем нужный товар в группе товаров
                                            DishesDPO dishes = await Task.Run(() => product.Disheses.FirstOrDefault(d => d.id == dishesDPO.id));
                                            if (dishes != null)
                                            {
                                                dishes.numberIitemsCart = 0;
                                                dishes.IsAddedToCart = false;
                                            }
                                        }
                                    }
                                    else // инчае изменяем кол-во
                                    {
                                        compositionCart.quantity -= 1;

                                        // обновляем список с отображаемыми товарами
                                        ProductViewDPO product = await Task.Run(() => ListProductView.FirstOrDefault(p => p.Disheses.Any(d => d.id == dishesDPO.id)));
                                        if (product != null)
                                        {
                                            // ищем нужный товар в группе товаров
                                            DishesDPO dishes = await Task.Run(() => product.Disheses.FirstOrDefault(d => d.id == dishesDPO.id));
                                            if (dishes != null)
                                            {
                                                dishes.numberIitemsCart -= 1;
                                            }
                                        }
                                    }

                                    // сохраняем обновленные данные
                                    await foodDeliveryContext.SaveChangesAsync();
                                }
                            }
                        }
                    }
                }
                // обновляем список
                //GetListProduct();
            }
        }

        #endregion

        // общие свойства страницы
        #region Features

        // отображение кнопок на товаре
        private bool _isAddedToCart { get; set; }
        public bool IsAddedToCart
        {
            get { return _isAddedToCart; }
            set { _isAddedToCart = value; OnPropertyChanged(nameof(IsAddedToCart)); }
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
