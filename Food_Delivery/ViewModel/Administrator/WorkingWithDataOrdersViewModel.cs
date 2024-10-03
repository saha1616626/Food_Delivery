using Food_Delivery.Data;
using Food_Delivery.Helper;
using Food_Delivery.Model;
using Food_Delivery.Model.DPO;
using Food_Delivery.View.Administrator.MenuSectionPages;
using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Printing;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace Food_Delivery.ViewModel.Administrator
{
    public class WorkingWithDataOrdersViewModel : INotifyPropertyChanged
    {
        public WorkingWithDataOrdersViewModel()
        {
            // после получения фокуса данного приложения запукаем закрытый Popup
            WorkingWithData._launchPopupAfterReceivingFocusOrders += LaunchPopupAfterReceivingFocusOrders;
        }

        // работа над добавлением заказа
        #region WorkingWithAddingDishes

        // общий список блюд для добавления в заказ с учетом выбранных пользователем
        private ObservableCollection<CompositionOrderDPO> _ListCompositionOrders { get; set; } = new ObservableCollection<CompositionOrderDPO>();
        public ObservableCollection<CompositionOrderDPO> ListCompositionOrders
        {
            get { return _ListCompositionOrders; }
            set { _ListCompositionOrders = value; OnPropertyChanged(nameof(ListCompositionOrders)); }
        }


        // дублирующий список товаров, чтобы при обновлении списка не сбрасвался товар выбранный ранее
        private ObservableCollection<CompositionOrderDPO> _listOrderCopy { get; set; } = new ObservableCollection<CompositionOrderDPO>();
        public ObservableCollection<CompositionOrderDPO> ListOrderCopy
        {
            get { return _listOrderCopy; }
            set { _listOrderCopy = value; OnPropertyChanged(nameof(ListOrderCopy)); }
        }

        // метод обновления списка
        private async Task WeGetListOfDishes()
        {
            //ListOrderCopy.Clear();
            // копируем список
            ListOrderCopy = new ObservableCollection<CompositionOrderDPO>(ListCompositionOrders);

            // подключаемся к БД
            using (FoodDeliveryContext foodDeliveryContext = new FoodDeliveryContext())
            {
                List<Dishes> dishes = await foodDeliveryContext.Dishes.ToListAsync();

                // изменяем список в зависимости от режима работы (добавление или редактирование данных)
                if (IsAddData) // добавление данных
                {
                    ListCompositionOrders.Clear(); // очищаем список

                    // передаем все блюда в список
                    foreach (Dishes dishesItem in dishes)
                    {
                        // проверяем, было ли блюдо добавлено ранее.
                        CompositionOrderDPO compositionOrder = await Task.Run(() => ListOrderCopy.FirstOrDefault(c => c.dishesId == dishesItem.id));
                        if (compositionOrder != null) // блюдо было добавлено ранее
                        {
                            CompositionOrderDPO compositionOrderDPO = new CompositionOrderDPO();
                            compositionOrderDPO = await compositionOrderDPO.CompositionOrder(dishesItem);
                            // изменяем кол-во товаров
                            compositionOrderDPO.QuantityInOrder = compositionOrder.QuantityInOrder;
                            // выключаем кнопку добавить
                            compositionOrderDPO.IsAddDishButton = compositionOrder.IsAddDishButton;
                            // включаем кнопки управлением кол-вом
                            compositionOrderDPO.IsEditDishButton = compositionOrder.IsEditDishButton;
                            // сумма денег по выбранному товару
                            compositionOrderDPO.AmountProduct = compositionOrder.AmountProduct;
                            ListCompositionOrders.Add(compositionOrderDPO);
                        }
                        else // блюдо не было добавлено ранее
                        {
                            if (dishesItem.stopList == false) // исключаем блюда, которые в стоп листе
                            {
                                CompositionOrderDPO compositionOrderDPO = new CompositionOrderDPO();
                                compositionOrderDPO = await compositionOrderDPO.CompositionOrder(dishesItem);
                                ListCompositionOrders.Add(compositionOrderDPO);
                            }
                        }
                    }

                    // сортируем массив. сначала добавленные в список элементы
                    var ListCompositionOrdersOrderBy = ListCompositionOrders.OrderByDescending(o => o.IsEditDishButton).ToArray();
                    ListCompositionOrders = new ObservableCollection<CompositionOrderDPO>(ListCompositionOrdersOrderBy);

                    // резервируем список ListCompositionOrders и копируем в ListOrderCopy
                    ListOrderCopy = new ObservableCollection<CompositionOrderDPO>(ListCompositionOrders);
                }
                else // редактирование данных
                {
                    // добавляем блюда, которых нет в списке при редактирование
                    // Внимание! Список для редактирования может содержать в себе: блюда архива, удаленные блюда. Фотографию, название, описание в 
                    // добавленных ранее блюдах не обновляем

                    foreach (Dishes item in dishes)
                    {
                        CompositionOrderDPO compositionOrderDPO = await Task.Run(() => ListCompositionOrders.FirstOrDefault(d => d.dishesId == item.id));
                        if (compositionOrderDPO == null) // если блюдо отсутствует в списке заказов, то мы его добавляем 
                        {
                            compositionOrderDPO = new CompositionOrderDPO();
                            if (item.stopList == false) // проверяем, что товар не в стоп листе
                            {
                                ListCompositionOrders.Add(await compositionOrderDPO.CompositionOrder(item));
                            }
                        }
                    }
                }

            }
        }

        // метод добавления товара в список
        public async Task AddProductList(CompositionOrderDPO compositionOrderDPO)
        {
            // находим нужный товар в общем списке 
            CompositionOrderDPO resCompositionOrderDPO = await Task.Run(() => ListCompositionOrders.FirstOrDefault(c => c.id == compositionOrderDPO.id)); // создаём новый фоновый поток, в
                                                                                                                                                          // котором выполлняем метод FirstOrDefault. ObservableCollection - синхронная коллекция
            if (resCompositionOrderDPO != null)
            {
                // проверяем, что кол-во не >0
                if (resCompositionOrderDPO.QuantityInOrder >= 0)
                {
                    // изменяем кол-во товаров
                    resCompositionOrderDPO.QuantityInOrder += 1;

                    // выключаем кнопку добавить
                    resCompositionOrderDPO.IsAddDishButton = false;
                    // включаем кнопки управлением кол-вом
                    resCompositionOrderDPO.IsEditDishButton = true;
                    // сумма денег по выбранному товару
                    resCompositionOrderDPO.AmountProduct = compositionOrderDPO.QuantityInOrder * compositionOrderDPO.price;
                    // изменяем итоговую сумму денег
                    CostPrice += compositionOrderDPO.QuantityInOrder * compositionOrderDPO.price;
                    OutCostPrice = CostPrice.ToString();
                    // обновляем список
                    int index = ListCompositionOrders.IndexOf(resCompositionOrderDPO);
                    ListCompositionOrders[index] = resCompositionOrderDPO;

                    // резервируем список ListCompositionOrders и копируем в ListOrderCopy
                    ListOrderCopy = new ObservableCollection<CompositionOrderDPO>(ListCompositionOrders);
                }
            }
        }

        // изменение кол-во товара в списке
        public async Task EditProductList(CompositionOrderDPO compositionOrderDPO, bool typeOperation)
        {
            // находим нужный товар в общем списке 
            CompositionOrderDPO resCompositionOrderDPO = await Task.Run(() => ListCompositionOrders.FirstOrDefault(c => c.id == compositionOrderDPO.id));
            if (resCompositionOrderDPO != null)
            {
                // проверка операции. Добавление или убавление кол-ва
                if (typeOperation) // прибавить
                {
                    resCompositionOrderDPO.QuantityInOrder += 1;
                    // сумма денег по выбранному товару
                    resCompositionOrderDPO.AmountProduct = resCompositionOrderDPO.price * resCompositionOrderDPO.QuantityInOrder;
                    // изменяем итоговую сумму денег
                    CostPrice += resCompositionOrderDPO.price;
                    OutCostPrice = CostPrice.ToString();
                }
                else // убавить
                {
                    if (compositionOrderDPO.QuantityInOrder == 1)
                    {
                        resCompositionOrderDPO.QuantityInOrder = 0;
                        // включаем кнопку добавить
                        resCompositionOrderDPO.IsAddDishButton = true;
                        // выключаем кнопки управлением кол-вом
                        resCompositionOrderDPO.IsEditDishButton = false;
                        // сумма денег по выбранному товару
                        resCompositionOrderDPO.AmountProduct = 0;
                        // изменяем итоговую сумму денег
                        CostPrice -= resCompositionOrderDPO.price;
                        OutCostPrice = CostPrice.ToString();
                    }
                    else // если позиции больше чем еденица
                    {
                        resCompositionOrderDPO.QuantityInOrder -= 1;
                        // сумма денег по выбранному товару
                        resCompositionOrderDPO.AmountProduct = resCompositionOrderDPO.price * resCompositionOrderDPO.QuantityInOrder;
                        // изменяем итоговую сумму денег
                        CostPrice -= resCompositionOrderDPO.price;
                        OutCostPrice = CostPrice.ToString();
                    }
                    if (CostPrice < 0) // если сумма приняла отрицательный коэффицент
                    {
                        CostPrice = 0;
                        OutCostPrice = CostPrice.ToString();
                    }
                }

                // обновляем список
                int index = ListCompositionOrders.IndexOf(resCompositionOrderDPO);
                ListCompositionOrders[index] = resCompositionOrderDPO;

                // резервируем список ListCompositionOrders и копируем в ListOrderCopy
                ListOrderCopy = new ObservableCollection<CompositionOrderDPO>(ListCompositionOrders);
            }
        }

        // добавление или редактирование данных
        private RelayCommand _btn_SaveData { get; set; }
        public RelayCommand Btn_SaveData
        {
            get
            {
                return _btn_SaveData ??
                    (_btn_SaveData = new RelayCommand(async (obj) =>
                    {
                        // проверка наличия обязательных данных
                        if (!string.IsNullOrWhiteSpace(OutClientName) && !string.IsNullOrWhiteSpace(OutClientSurname) &&
                        !string.IsNullOrWhiteSpace(OutClientCity) && !string.IsNullOrWhiteSpace(OutClientStreet) &&
                        !string.IsNullOrWhiteSpace(OutClientHouse) && !string.IsNullOrWhiteSpace(OutClientNumberPhone) &&
                        SelectedOrderStatus != null)
                        {
                            // проверяем корректность введенных данных
                            bool isCheckingNumbers = false; // переменная корректности введённого номера телефона
                            bool isCheckingHouse = false; // переменная корректности введённого дома
                            bool isCheckingApartment = false; // переменная корректности введённой квартиры
                            bool isCheckingEmail = false; // переменная корректности введённого Email
                            bool isAmountChange = false; // переменная коррекности введённой суммы сдачи

                            ErrorInput = ""; // очищаем поле ошибки

                            // проверяем целое число в поле "Дом"
                            isCheckingHouse = int.TryParse(OutClientHouse.Trim(), out int house);
                            if (!isCheckingHouse) // число не получено
                            {
                                StartFieldIllumination(AnimationOutHouse); // подсветка поля
                                ErrorInput = "Введите целое число!"; // сообщение с обибкой
                            }

                            // проверяем цело число в поле "Квартира"
                            if (OutClientApartment != null && OutClientApartment.Trim() != "")
                            {
                                isCheckingApartment = int.TryParse(OutClientApartment.Trim(), out int apartament);
                                if (!isCheckingApartment) // число не получено
                                {
                                    StartFieldIllumination(AnimationOutApartment); // подсветка поля
                                    ErrorInput = "Введите целое число!"; // сообщение с обибкой
                                }
                            }
                            else
                            {
                                isCheckingApartment = true; // если квартира не выбрана, то проверка пройдена, так как атрибут не обязатльный
                            }

                            // проверяем цело число в поле "Номер телефона"
                            isCheckingNumbers = (double.TryParse(OutClientNumberPhone.Trim(), out double number));
                            if (!isCheckingNumbers) // если все числа корректны
                            {
                                StartFieldIllumination(AnimationOutNumberPhone); // подсветка поля
                                ErrorInput += "\nФормат номера телефона нарушен! Только цифры!"; // сообщение с ошибкой
                            }
                            else
                            {
                                if (OutClientNumberPhone.Trim().Length == 11) // проверяем на кол-во цифр в номере телефона
                                {
                                    if (OutClientNumberPhone.Trim().StartsWith("7")) // проверка на 7 в начале
                                    {

                                    }
                                    else
                                    {
                                        isCheckingNumbers = false;
                                        StartFieldIllumination(AnimationOutNumberPhone); // подсветка поля
                                        ErrorInput += "\nНомер телефона должен начинаться с \"7\"!"; // сообщение с ошибкой
                                    }
                                }
                                else
                                {
                                    isCheckingNumbers = false;
                                    StartFieldIllumination(AnimationOutNumberPhone); // подсветка поля
                                    ErrorInput += "\nФормат номера телефона нарушен! 11 цифр!"; // сообщение с ошибкой
                                }
                            }

                            // проверяем Email, если введен
                            if (OutClientEmail != null && OutClientEmail.Trim() != "")
                            {
                                if (!OutClientEmail.Contains("@"))
                                {
                                    StartFieldIllumination(AnimationOutEmail); // подсветка поля
                                    ErrorInput += "\nФормат Email нарушен! Нет знака \"@\"!"; // сообщение с ошибкой
                                }
                            }
                            else
                            {
                                isCheckingEmail = true;
                            }


                            // проверяем сумму сдачи, если выбраны наличные
                            if (!string.IsNullOrWhiteSpace(OutAmountChange))
                            {
                                isAmountChange = int.TryParse(OutAmountChange.Trim(), out int amount);
                                if (!isAmountChange)
                                {
                                    StartFieldIllumination(AnimationAmountChange); // подсветка поля
                                    ErrorInput += "\nВведите целое число!"; // сообщение с ошибкой
                                }
                            }
                            else
                            {
                                isAmountChange = true; //если поле не заполнено, то условие успешно,
                                                       //так как данные не обязательны для заполнения
                            }

                            BeginFadeAnimation(AnimationErrorInput); // затухание сообщения об ошибке
                            //ErrorInput = "";

                            // проверка формата данных
                            if (isCheckingNumbers && isCheckingApartment && isCheckingHouse && isCheckingEmail && isAmountChange)
                            {
                                // проверка на наличие выбранного(ых) блюда
                                List<CompositionOrderDPO> compositionOrderDPOs = await Task.Run(() => ListCompositionOrders.Where(c => c.IsEditDishButton == true).ToList());

                                if (compositionOrderDPOs.Count > 0)
                                {
                                    // проверка на добавление или редактирование
                                    if (IsAddData) // добавление данных
                                    {
                                        using (FoodDeliveryContext foodDeliveryContext = new FoodDeliveryContext())
                                        {
                                            List<Order> orders = await foodDeliveryContext.Orders.ToListAsync();
                                            List<CompositionCart> carts = await foodDeliveryContext.CompositionCarts.ToListAsync();

                                            // сначала добавляем данные о заказе
                                            Order order = new Order();
                                            order.dateTime = DateTime.Now;

                                            // получаем id пользователя, если заказ оформлял определенный клиент

                                            order.startDesiredDeliveryTime = new DateTime(SelectedDate.Year, SelectedDate.Month,
                                                SelectedDate.Day, SelectedStartTimeDelivery.Hour, SelectedStartTimeDelivery.Minute,
                                                SelectedStartTimeDelivery.Second);
                                            order.endDesiredDeliveryTime = new DateTime(SelectedDate.Year, SelectedDate.Month,
                                                SelectedDate.Day, SelectedEndTimeDelivery.Hour, SelectedEndTimeDelivery.Minute,
                                                SelectedEndTimeDelivery.Second);
                                            order.orderStatusId = SelectedOrderStatus.id;
                                            order.name = OutClientName.Trim();
                                            order.surname = OutClientSurname.Trim();
                                            if (!string.IsNullOrWhiteSpace(OutClientPatronymic))
                                            {
                                                order.patronymic = OutClientPatronymic.Trim();
                                            }
                                            order.city = OutClientCity.Trim();
                                            order.street = OutClientStreet.Trim();
                                            order.house = OutClientHouse.Trim();
                                            if (!string.IsNullOrWhiteSpace(OutClientApartment))
                                            {
                                                order.apartment = OutClientApartment.Trim();
                                            }
                                            order.numberPhone = OutClientNumberPhone.Trim();
                                            if (!string.IsNullOrWhiteSpace(OutClientEmail))
                                            {
                                                order.email = OutClientEmail.Trim();
                                            }
                                            order.costPrice = int.Parse(OutCostPrice.Trim());

                                            // проверка статуса оплаты
                                            if (IsOptionCardSelected) // если выбрана карта
                                            {
                                                order.typePayment = "Карта";
                                            }
                                            else // если выбраны наличные
                                            {
                                                // получаем сумму сдачи
                                                order.typePayment = "Наличные";
                                            }

                                            if (!string.IsNullOrWhiteSpace(OutAmountChange))
                                            {
                                                order.prepareChangeMoney = int.Parse((string)OutAmountChange.Trim());
                                            }

                                            await foodDeliveryContext.Orders.AddAsync(order); // добавляем данные в список БД
                                            await foodDeliveryContext.SaveChangesAsync(); // cохраняем изменения в базе данных

                                            // теперь добавляем даннные заказа (список блюд)
                                            foreach (CompositionOrderDPO item in compositionOrderDPOs)
                                            {
                                                CompositionOrder compositionOrder = new CompositionOrder();
                                                compositionOrder.orderId = (int)order.id; // берём id из созданного заказа
                                                if (item.dishesId != null)
                                                {
                                                    compositionOrder.dishesId = item.dishesId;
                                                }
                                                compositionOrder.nameDishes = item.nameDishes;
                                                if (item.descriptionDishes != null)
                                                {
                                                    compositionOrder.descriptionDishes = item.descriptionDishes;
                                                }
                                                if (item.calories != null)
                                                {
                                                    compositionOrder.calories = item.calories;
                                                }
                                                if (item.squirrels != null)
                                                {
                                                    compositionOrder.squirrels = item.squirrels;
                                                }
                                                if (item.fats != null)
                                                {
                                                    compositionOrder.fats = item.fats;
                                                }
                                                if (item.carbohydrates != null)
                                                {
                                                    compositionOrder.carbohydrates = item.carbohydrates;
                                                }
                                                if (item.weight != null)
                                                {
                                                    compositionOrder.weight = item.weight;
                                                }
                                                compositionOrder.quantity = item.QuantityInOrder;
                                                compositionOrder.price = item.price;

                                                // преобразовываем изображение в массив байтов
                                                using (MemoryStream memoryStream = new MemoryStream())
                                                {
                                                    PngBitmapEncoder encoder = new PngBitmapEncoder(); // кодируем в формат PNG
                                                    encoder.Frames.Add(BitmapFrame.Create(item.image)); // преобразовываем полученное изображение в нужный формат
                                                    encoder.Save(memoryStream);
                                                    compositionOrder.image = memoryStream.ToArray();
                                                }

                                                await foodDeliveryContext.CompositionOrders.AddAsync(compositionOrder); // добавляем данные в список БД
                                            }

                                            await foodDeliveryContext.SaveChangesAsync(); // cохраняем изменения в базе данных

                                            // закрываем страницу добавления данных
                                            WorkingWithData.ClosingCorkWithDataOrdersPage(); // вызываем событие перехода на страницу "заказы"
                                        }
                                    }
                                    else // редактирование данных
                                    {
                                        using (FoodDeliveryContext foodDeliveryContext = new FoodDeliveryContext())
                                        {
                                            List<Order> orders = await foodDeliveryContext.Orders.ToListAsync();
                                            List<CompositionCart> carts = await foodDeliveryContext.CompositionCarts.ToListAsync();

                                            // находим объект для изменения в БД
                                            Order order = await foodDeliveryContext.Orders.FirstOrDefaultAsync(d => d.id == SelectedOrder.id);
                                            if (order != null)
                                            {
                                                // изменяем данные

                                                order.startDesiredDeliveryTime = new DateTime(SelectedDate.Year, SelectedDate.Month,
                                                    SelectedDate.Day, SelectedStartTimeDelivery.Hour, SelectedStartTimeDelivery.Minute,
                                                    SelectedStartTimeDelivery.Second);
                                                order.endDesiredDeliveryTime = new DateTime(SelectedDate.Year, SelectedDate.Month,
                                                    SelectedDate.Day, SelectedEndTimeDelivery.Hour, SelectedEndTimeDelivery.Minute,
                                                    SelectedEndTimeDelivery.Second);
                                                order.orderStatusId = SelectedOrderStatus.id;
                                                order.name = OutClientName.Trim();
                                                order.surname = OutClientSurname.Trim();
                                                if (!string.IsNullOrWhiteSpace(OutClientPatronymic))
                                                {
                                                    order.patronymic = OutClientPatronymic.Trim();
                                                }
                                                order.city = OutClientCity.Trim();
                                                order.street = OutClientStreet.Trim();
                                                order.house = OutClientHouse.Trim();
                                                if (!string.IsNullOrWhiteSpace(OutClientApartment))
                                                {
                                                    order.apartment = OutClientApartment.Trim();
                                                }
                                                order.numberPhone = OutClientNumberPhone.Trim();
                                                if (!string.IsNullOrWhiteSpace(OutClientEmail))
                                                {
                                                    order.email = OutClientEmail.Trim();
                                                }
                                                order.costPrice = int.Parse(OutCostPrice.Trim());

                                                // проверка статуса оплаты
                                                if (IsOptionCardSelected) // если выбрана карта
                                                {
                                                    order.typePayment = "Карта";
                                                }
                                                else // если выбраны наличные
                                                {
                                                    // получаем сумму сдачи
                                                    order.typePayment = "Наличные";
                                                }

                                                if (!string.IsNullOrWhiteSpace(OutAmountChange))
                                                {
                                                    order.prepareChangeMoney = int.Parse((string)OutAmountChange.Trim());
                                                }

                                                await foodDeliveryContext.SaveChangesAsync(); // cохраняем изменения в базе данных

                                            }

                                            // список товаров в заказе
                                            List<CompositionOrder> composition = new List<CompositionOrder>();

                                            // преобразовываем список заказов в нужный тип данных
                                            foreach (CompositionOrderDPO item in compositionOrderDPOs)
                                            {
                                                CompositionOrder compositionOrder = new CompositionOrder();
                                                compositionOrder.orderId = SelectedOrder.id; // берём id из выбранного заказа
                                                if (item.dishesId != null)
                                                {
                                                    compositionOrder.dishesId = item.dishesId;
                                                }
                                                compositionOrder.nameDishes = item.nameDishes;
                                                if (item.descriptionDishes != null)
                                                {
                                                    compositionOrder.descriptionDishes = item.descriptionDishes;
                                                }
                                                if (item.calories != null)
                                                {
                                                    compositionOrder.calories = item.calories;
                                                }
                                                if (item.squirrels != null)
                                                {
                                                    compositionOrder.squirrels = item.squirrels;
                                                }
                                                if (item.fats != null)
                                                {
                                                    compositionOrder.fats = item.fats;
                                                }
                                                if (item.carbohydrates != null)
                                                {
                                                    compositionOrder.carbohydrates = item.carbohydrates;
                                                }
                                                if (item.weight != null)
                                                {
                                                    compositionOrder.weight = item.weight;
                                                }
                                                compositionOrder.quantity = item.QuantityInOrder;
                                                compositionOrder.price = item.price;


                                                // преобразовываем изображение в массив байтов
                                                using (MemoryStream memoryStream = new MemoryStream())
                                                {
                                                    PngBitmapEncoder encoder = new PngBitmapEncoder(); // кодируем в формат PNG
                                                    encoder.Frames.Add(BitmapFrame.Create(item.image)); // преобразовываем полученное изображение в нужный формат
                                                    encoder.Save(memoryStream);
                                                    compositionOrder.image = memoryStream.ToArray();
                                                }

                                                composition.Add(compositionOrder);
                                            }

                                            // берем все блюда данного заказа, удаляем и добавляем новые
                                            List<CompositionOrder> compositionOrders = await Task.Run(() => foodDeliveryContext.CompositionOrders.Where(c => c.orderId == SelectedOrder.id).ToListAsync());
                                            if (compositionOrders != null)
                                            {
                                                foreach (CompositionOrder itemOrder in compositionOrders)
                                                {
                                                    // удаляем товар из БД
                                                    foodDeliveryContext.CompositionOrders.Remove(itemOrder);
                                                }

                                                // Добавляем обновленные товары в БД
                                                foreach (var itemCompositionOrder in composition)
                                                {
                                                    await foodDeliveryContext.CompositionOrders.AddAsync(itemCompositionOrder);
                                                }

                                                // обновляем список БД
                                                await foodDeliveryContext.SaveChangesAsync(); // cохраняем изменения в базе данных
                                            }

                                            // закрываем страницу добавления данных
                                            WorkingWithData.ClosingCorkWithDataOrdersPage(); // вызываем событие перехода на страницу "заказы"
                                        }
                                    }
                                }
                                else
                                {
                                    ErrorInput = "Выберите блюдо(а)!"; // сообщение с ошибкой
                                    BeginFadeAnimation(AnimationErrorInput); // затухание сообщения об ошибке
                                }

                            }
                        }
                        else
                        {
                            ErrorInput = ""; // очищаем сообщение об ошибке

                            if (string.IsNullOrWhiteSpace(OutClientName))
                            {
                                StartFieldIllumination(AnimationOutName); // подсветка поля
                                ErrorInput = "Заполните обязательные поля!"; // сообщение с ошибкой
                            }

                            if (string.IsNullOrWhiteSpace(OutClientSurname))
                            {
                                StartFieldIllumination(AnimationOutSurname); // подсветка поля
                                ErrorInput = "Заполните обязательные поля!"; // сообщение с ошибкой
                            }

                            if (string.IsNullOrWhiteSpace(OutClientCity))
                            {
                                StartFieldIllumination(AnimationOutCity); // подсветка поля
                                ErrorInput = "Заполните обязательные поля!"; // сообщение с ошибкой
                            }

                            if (string.IsNullOrWhiteSpace(OutClientStreet))
                            {
                                StartFieldIllumination(AnimationOutStreet); // подсветка поля
                                ErrorInput = "Заполните обязательные поля!"; // сообщение с ошибкой
                            }

                            if (string.IsNullOrWhiteSpace(OutClientHouse))
                            {
                                StartFieldIllumination(AnimationOutHouse); // подсветка поля
                                ErrorInput = "Заполните обязательные поля!"; // сообщение с ошибкой
                            }

                            if (string.IsNullOrWhiteSpace(OutClientNumberPhone))
                            {
                                StartFieldIllumination(AnimationOutNumberPhone); // подсветка поля
                                ErrorInput = "Заполните обязательные поля!"; // сообщение с ошибкой
                            }

                            if (SelectedOrderStatus == null)
                            {
                                StartFieldIllumination(AnimationOrderStatus); // подсветка поля
                                ErrorInput = "Заполните обязательные поля!"; // сообщение с ошибкой
                            }

                            BeginFadeAnimation(AnimationErrorInput); // затухание сообщения об ошибке
                        }
                    }, (obj) => true));
            }
        }

        #endregion

        // подготовка страницы
        #region PreparingPage

        bool IsAddData; // редактируем или добавляем данные

        // изменяем названия страницы (редактируем или добавлем новые записи)
        public async Task ChangingName(bool IsAddData, OrderDPO SelectedOrder)
        {
            // получаем список статусов заказа
            await OrderStatuses(IsAddData, SelectedOrder);

            this.IsAddData = IsAddData; // передаём значение

            if (IsAddData) // добавление данных
            {
                HeadingPage = "Создание заказа";

                // если добавление данных, то мы подгатавливаем поля
                SelectedStartTimeDelivery = new DateTime((int)DateTime.Now.Year, (int)DateTime.Now.Month, (int)DateTime.Now.Day, 9, 0, 0, 0); // устанавливаем начальное время
                SelectedDate = (new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)); // установка начальной даты заказа
                IsOptionCardSelected = true; // по умолчанию выбрана карта
                IsFieldVisibilityTypePayment = true; // делаем недоступное поле для ввода суммы сдачи, так как выбрана карта
                DarkBackground = Visibility.Collapsed; // фон для Popup скрыт
                OutCostPrice = "0"; // нулевая начальная стоимость заказа
            }
            else // редактирование данных
            {
                HeadingPage = "Изменение заказа";
                // проверяем, получили ли мы выбранный заказ, если да, то заполняем поля
                if (SelectedOrder != null)
                {
                    this.SelectedOrder = SelectedOrder; // сохраняем в переменную для дальнейшей работы выбранный заказ
                    DarkBackground = Visibility.Collapsed; // фон для Popup скрыт

                    // устанавливаем начальные данные
                    OutClientName = SelectedOrder.name;
                    OutClientSurname = SelectedOrder.surname;
                    if (SelectedOrder.patronymic != null)
                    {
                        OutClientPatronymic = SelectedOrder.patronymic;
                    }
                    OutClientCity = SelectedOrder.city;
                    OutClientStreet = SelectedOrder.street;
                    OutClientHouse = SelectedOrder.house;
                    if (SelectedOrder.apartment != null)
                    {
                        OutClientApartment = SelectedOrder.apartment;
                    }
                    OutClientNumberPhone = SelectedOrder.numberPhone;
                    if (SelectedOrder.email != null)
                    {
                        OutClientEmail = SelectedOrder.email;
                    }
                    CostPrice = SelectedOrder.costPrice;

                    if (SelectedOrder.typePayment == "Карта") // если выбрана карта
                    {
                        IsOptionCashSelected = false;
                        IsOptionCardSelected = true;
                        IsFieldVisibilityTypePayment = true; // скрываем возможность ввода
                                                             // суммы подготовки сдачи, если выбранна карта
                    }
                    else // если выбраны наличные
                    {
                        IsOptionCashSelected = true;
                        IsOptionCardSelected = false;

                        if (SelectedOrder.prepareChangeMoney != null)
                        {
                            OutAmountChange = SelectedOrder.prepareChangeMoney.ToString();
                        }
                    }

                    SelectedDate = (DateTime)SelectedOrder.startDesiredDeliveryTime;
                    SelectedStartTimeDelivery = (DateTime)SelectedOrder.startDesiredDeliveryTime;
                    SelectedEndTimeDelivery = (DateTime)SelectedOrder.endDesiredDeliveryTime;

                    // добавляем товары в список
                    using (FoodDeliveryContext foodDeliveryContext = new FoodDeliveryContext())
                    {
                        List<CompositionOrder> compositionOrders = await foodDeliveryContext.CompositionOrders.ToListAsync();
                        List<Dishes> dishes = await foodDeliveryContext.Dishes.ToListAsync();

                        // получаем список товаров в заказе
                        List<CompositionOrder> compositions = new List<CompositionOrder>();
                        compositions = await Task.Run(() => compositionOrders.Where(c => c.orderId == SelectedOrder.id).ToList());
                        if (compositionOrders.Count > 0)
                        {
                            int costPrice = 0; // сумма заказа
                            foreach (CompositionOrder item in compositions)
                            {
                                CompositionOrderDPO compositionOrderDPO = new CompositionOrderDPO();
                                ListCompositionOrders.Add(await compositionOrderDPO.CompositionOrder(item));

                                costPrice += (int)item.price * (int)item.quantity;
                            }

                            OutCostPrice += costPrice.ToString(); // сумма заказа

                            // добавляем данные из списка основных блюд, в список выбранных.
                            await WeGetListOfDishes();
                        }
                    }
                }
            }
        }

        // возврат на страницу "заказы"
        private RelayCommand _btn_ReturnPreviousPage { get; set; }
        public RelayCommand Btn_ReturnPreviousPage
        {
            get
            {
                return _btn_ReturnPreviousPage ??
                    (_btn_ReturnPreviousPage = new RelayCommand(async (obj) =>
                    {
                        // вызываем событие перехода на страницу "заказы"
                        WorkingWithData.ClosingCorkWithDataOrdersPage();
                    }, (obj) => true));
            }
        }

        #endregion

        // работа с товарами в заказе
        #region Popup

        // путь к json работа окна Popup
        readonly string pathDataPopup = @"E:\3comm\Documents\Предметы\Курс 3.2\Курсовая\Приложение\Программа\Food_Delivery\Food_Delivery\Data\СheckPopup.json";


        private bool IsCheckAddAndEditOrDeleteFocus; // true - добавление, редактирование или удаление данных.
                                                     // для удержания фокуса на приложении при переходе между окнами

        // запуск Popup при добавлении данных
        private RelayCommand _btn_AddDishes { get; set; }
        public RelayCommand Btn_AddDishes
        {
            get
            {
                return _btn_AddDishes ??
                    (_btn_AddDishes = new RelayCommand(async (obj) =>
                    {
                        DarkBackground = Visibility.Visible; // показать фон
                        StartPoupAddDishes = true; // запускаем Poup
                        IsCheckAddAndEditOrDeleteFocus = true; // режим добавления данных                                 
                        await WeGetListOfDishes(); // обновляем список доступных блюд, с учётом добавленных
                        NotificationOfThePopupLaunchJson(); // оповещаем JSON, чтомы запустили Popup
                    }, (obj) => true));
            }
        }

        // скрываем Popup
        private RelayCommand _closePopup { get; set; }
        public RelayCommand ClosePopup
        {
            get
            {
                return _closePopup ??
                    (_closePopup = new RelayCommand((obj) =>
                    {
                        DarkBackground = Visibility.Visible; // показать фон
                        StartPoupAddDishes = false; // закрываем Poup
                    }, (obj) => true));
            }
        }

        // записываем в JSON, что мы запустили Popup данной страницы
        private void NotificationOfThePopupLaunchJson()
        {
            // передаём в JSON, что мы запустили Popup
            var jsonData = new { popup = "Orders" };
            // Преобразуем объект в JSON-строку
            string jsonText = JsonConvert.SerializeObject(jsonData);
            File.WriteAllText(pathDataPopup, jsonText);
        }

        // запускаем Popup (для редактирования или удаления)
        private void LaunchPopupAfterReceivingFocusOrders(object sender, EventAggregator eventAggregator)
        {
            if (IsCheckAddAndEditOrDeleteFocus) // если это добавление или редактирование
            {
                StartPoupAddDishes = true; // отображаем Popup
            }
            else // если это удаление данных 
            {

            }
            DarkBackground = Visibility.Visible; // показать фон
            WorkingWithData.ExitHamburgerMenu(); // закрываем, если открыто "гамбургер меню"
        }

        #endregion

        // свойства
        #region Features

        public OrderDPO SelectedOrder; // выбранное блюдо
        public TextBox AnimationOutName { get; set; } // поле для ввода текста "имя клиента". Вывод подсветки поля
        public TextBox AnimationOutSurname { get; set; } // поле для ввода текста "фамилия клиента". Вывод подсветки поля
        public TextBox AnimationOutPatronymic { get; set; } // поле для ввода текста "отчество клиента". Вывод подсветки поля
        public TextBox AnimationOutCity { get; set; } // поле для ввода текста "город клиента". Вывод подсветки поля
        public TextBox AnimationOutStreet { get; set; } // поле для ввода текста "улица клиента". Вывод подсветки поля
        public TextBox AnimationOutHouse { get; set; } // поле для ввода текста "дом клиента". Вывод подсветки поля
        public TextBox AnimationOutApartment { get; set; } // поле для ввода текста "квартира клиента". Вывод подсветки поля
        public TextBox AnimationOutNumberPhone { get; set; } // поле для ввода текста "номер телефона клиента". Вывод подсветки поля
        public TextBox AnimationOutEmail { get; set; } // поле для ввода текста "email клиента". Вывод подсветки поля
        public DatePicker AnimationDeliveryDate { get; set; } // поле для ввода даты доставки. Вывод подсветки поля
        public TimePicker AnimationStartDesiredDeliveryTime { get; set; } // поле для ввода начала интервала доставки. Вывод подсветки поля
        public TimePicker AnimationEndDesiredDeliveryTime { get; set; } // поле для ввода конца интервала доставки. Вывод подсветки поля
        public TextBox AnimationAmountChange { get; set; } // поле для ввода текста "сумма сдачи". Вывод подсветки поля
        public ComboBox AnimationOrderStatus { get; set; } // поле для выбора статуса заказа. Вывод подсветки поля
        public TextBox AnimationCostPrice { get; set; } // поле ценой заказа. Вывод подсветки поля
        public TextBlock AnimationErrorInputPopup { get; set; } // объект текстового поля. Анимация затухания текста после вывода сообщения в Poup.
        public TextBlock AnimationErrorInput { get; set; } // объект текстового поля. Анимация затухания текста после вывода сообщения.
        public Storyboard FieldIllumination { get; set; } // анимация объектов

        // ассинхронно получаем информацию из PageWorkingWithDataOrders 
        public async Task InitializeAsync(TextBlock AnimationErrorInputPopup, Storyboard FieldIllumination, TextBox AnimationOutName,
            TextBox AnimationOutSurname, TextBox AnimationOutPatronymic, TextBox AnimationOutCity, TextBox AnimationOutStreet,
            TextBox AnimationOuttHouse, TextBox AnimationOutApartment, TextBox AnimationOutNumberPhone, TextBox AnimationOutEmail,
            DatePicker AnimationDeliveryDate, TimePicker AnimationStartDesiredDeliveryTime, TimePicker AnimationEndDesiredDeliveryTime,
            TextBox AnimationAmountChange, ComboBox AnimationOrderStatus, TextBox AnimationCostPrice, TextBlock AnimationErrorInput)
        {
            if (AnimationErrorInputPopup != null)
            {
                this.AnimationErrorInputPopup = AnimationErrorInputPopup;
            }
            if (FieldIllumination != null)
            {
                this.FieldIllumination = FieldIllumination;
            }
            if (AnimationOutName != null)
            {
                this.AnimationOutName = AnimationOutName;
            }
            if (AnimationOutSurname != null)
            {
                this.AnimationOutSurname = AnimationOutSurname;
            }
            if (AnimationOutPatronymic != null)
            {
                this.AnimationOutPatronymic = AnimationOutPatronymic;
            }
            if (AnimationOutCity != null)
            {
                this.AnimationOutCity = AnimationOutCity;
            }
            if (AnimationOutStreet != null)
            {
                this.AnimationOutStreet = AnimationOutStreet;
            }
            if (AnimationOuttHouse != null)
            {
                this.AnimationOutHouse = AnimationOuttHouse;
            }
            if (AnimationOutApartment != null)
            {
                this.AnimationOutApartment = AnimationOutApartment;
            }
            if (AnimationOutNumberPhone != null)
            {
                this.AnimationOutNumberPhone = AnimationOutNumberPhone;
            }
            if (AnimationOutEmail != null)
            {
                this.AnimationOutEmail = AnimationOutEmail;
            }
            if (AnimationDeliveryDate != null)
            {
                this.AnimationDeliveryDate = AnimationDeliveryDate;
            }
            if (AnimationStartDesiredDeliveryTime != null)
            {
                this.AnimationStartDesiredDeliveryTime = AnimationStartDesiredDeliveryTime;
            }
            if (AnimationEndDesiredDeliveryTime != null)
            {
                this.AnimationEndDesiredDeliveryTime = AnimationEndDesiredDeliveryTime;
            }
            if (AnimationAmountChange != null)
            {
                this.AnimationAmountChange = AnimationAmountChange;
            }
            if (AnimationOrderStatus != null)
            {
                this.AnimationOrderStatus = AnimationOrderStatus;
            }
            if (AnimationCostPrice != null)
            {
                this.AnimationCostPrice = AnimationCostPrice;
            }
            if (AnimationErrorInput != null)
            {
                this.AnimationErrorInput = AnimationErrorInput;
            }
        }

        #region Client

        // переменная с суммой заказа
        private int? CostPrice = 0;

        // поле с суммой заказа
        private string _outCostPrice;
        public string OutCostPrice
        {
            get { return _outCostPrice; }
            set
            {
                _outCostPrice = value;
                OnPropertyChanged(nameof(OutCostPrice));
            }
        }

        // выбранный статус заказа
        private OrderStatus _selectedOrderStatus { get; set; }
        public OrderStatus SelectedOrderStatus
        {
            get { return _selectedOrderStatus; }
            set
            {
                _selectedOrderStatus = value; OnPropertyChanged(nameof(SelectedOrderStatus));
                // если заказ имеет один из статусов, то мы не даём нажать на кноку "добавить товар"
                if (SelectedOrderStatus.name == "Готов" || SelectedOrderStatus.name == "Доставляется" ||
                    SelectedOrderStatus.name == "Отменен" || SelectedOrderStatus.name == "Отклонен" || SelectedOrderStatus.name == "Доставлен")
                {
                    IsAddDishes = false;
                }
                else
                {
                    IsAddDishes = true; // можно добавить товар
                }
            }
        }

        // список статусов заказа
        private List<OrderStatus> _optionsOrderStatus { get; set; }
        public List<OrderStatus> OptionsOrderStatus
        {
            get
            {
                return _optionsOrderStatus;
            }
            set
            {
                _optionsOrderStatus = value;
                OnPropertyChanged(nameof(OptionsOrderStatus));
            }
        }

        // получаем список статусов заказа
        private async Task OrderStatuses(bool IsAddData, OrderDPO SelectedOrder)
        {
            using (FoodDeliveryContext foodDeliveryContext = new FoodDeliveryContext())
            {
                List<OrderStatus> orderStatuses = await foodDeliveryContext.OrderStatus.ToListAsync(); // получаем список статусов заказов
                OptionsOrderStatus = new List<OrderStatus>(orderStatuses);


                if (IsAddData) // устанавливаем начальный статус, если добавление данных
                {
                    OrderStatus orderStatus = await Task.Run(() => orderStatuses.FirstOrDefault(o => o.id == 1));
                    if (orderStatus != null)
                    {
                        SelectedOrderStatus = orderStatus; // при создании заказа по умолчанию ставим "Новый заказ"
                    }
                }
                else
                {
                    OrderStatus orderStatus = await Task.Run(() => orderStatuses.FirstOrDefault(o => o.id == SelectedOrder.orderStatusId));
                    if (orderStatus != null)
                    {
                        SelectedOrderStatus = orderStatus; // при создании заказа по умолчанию ставим "Новый заказ"
                    }
                }
            }
        }

        // выбор оплата картой
        private bool _isOptionCardSelected { get; set; }
        public bool IsOptionCardSelected
        {
            get { return _isOptionCardSelected; }
            set
            {
                _isOptionCardSelected = value; OnPropertyChanged(nameof(IsOptionCardSelected));
                IsFieldVisibilityTypePayment = false;
            } // делаем поле для ввода сдачи недоступным
        }

        // выбор оплата наличными
        private bool _isOptionCashSelected { get; set; }
        public bool IsOptionCashSelected
        {
            get { return _isOptionCashSelected; }
            set
            {
                _isOptionCashSelected = value; OnPropertyChanged(nameof(IsOptionCashSelected));
                IsFieldVisibilityTypePayment = true;
            } // делаем поле для ввода сдачи доступным
        }

        // видимость поля способа оплаты
        private bool _isFieldVisibilityTypePayment;
        public bool IsFieldVisibilityTypePayment
        {
            get { return _isFieldVisibilityTypePayment; }
            set
            {
                _isFieldVisibilityTypePayment = value;
                OnPropertyChanged(nameof(IsFieldVisibilityTypePayment));
            }
        }

        // поле подготовить сдачу от суммы
        private string _outAmountChange { get; set; }
        public string OutAmountChange
        {
            get { return _outAmountChange; }
            set { _outAmountChange = value; OnPropertyChanged(nameof(OutAmountChange)); }
        }

        // дата доставки 
        private DateTime _selectedDate { get; set; }
        public DateTime SelectedDate
        {
            get { return _selectedDate; }
            set { _selectedDate = value; OnPropertyChanged(nameof(SelectedDate)); }
        }

        // начальное время доставки
        private DateTime _selectedStartTimeDelivery { get; set; }
        public DateTime SelectedStartTimeDelivery
        {
            get { return _selectedStartTimeDelivery; }
            set { _selectedStartTimeDelivery = value; OnPropertyChanged(nameof(SelectedStartTimeDelivery)); LimitationInitialDeliveryTime(); }
        }

        // конечное время доставки
        private DateTime _selectedEndTimeDelivery { get; set; }
        public DateTime SelectedEndTimeDelivery
        {
            get { return _selectedEndTimeDelivery; }
            set { _selectedEndTimeDelivery = value; OnPropertyChanged(nameof(SelectedEndTimeDelivery)); }
        }

        // email клиента
        private string _outClientEmail { get; set; }
        public string OutClientEmail
        {
            get { return _outClientEmail; }
            set { _outClientEmail = value; OnPropertyChanged(nameof(OutClientEmail)); }
        }

        // номер телефона клиента
        private string _outClientNumberPhone { get; set; }
        public string OutClientNumberPhone
        {
            get { return _outClientNumberPhone; }
            set { _outClientNumberPhone = value; OnPropertyChanged(nameof(OutClientNumberPhone)); }
        }

        // квартира клиента
        private string _outClientApartment { get; set; }
        public string OutClientApartment
        {
            get { return _outClientApartment; }
            set { _outClientApartment = value; OnPropertyChanged(nameof(OutClientApartment)); }
        }

        // дом клиента
        private string _outClientHouse { get; set; }
        public string OutClientHouse
        {
            get { return _outClientHouse; }
            set { _outClientHouse = value; OnPropertyChanged(nameof(OutClientHouse)); }
        }

        // улица клиента
        private string _outClientStreet { get; set; }
        public string OutClientStreet
        {
            get { return _outClientStreet; }
            set { _outClientStreet = value; OnPropertyChanged(nameof(OutClientStreet)); }
        }

        // город клиента
        private string _outClientCity { get; set; }
        public string OutClientCity
        {
            get { return _outClientCity; }
            set { _outClientCity = value; OnPropertyChanged(nameof(OutClientCity)); }
        }

        // отчество клиента
        private string _outClientPatronymic { get; set; }
        public string OutClientPatronymic
        {
            get { return _outClientPatronymic; }
            set { _outClientPatronymic = value; OnPropertyChanged(nameof(OutClientPatronymic)); }
        }

        // фамилия клиента
        private string _outClientSurname { get; set; }
        public string OutClientSurname
        {
            get { return _outClientSurname; }
            set { _outClientSurname = value; OnPropertyChanged(nameof(OutClientSurname)); }
        }

        // имя клиента
        private string _outClientName { get; set; }
        public string OutClientName
        {
            get { return _outClientName; }
            set { _outClientName = value; OnPropertyChanged(nameof(OutClientName)); }
        }

        #endregion

        // свойство для вывода текстовой ошибки при добавлении или редактировании данных
        private string _errorInput { get; set; }
        public string ErrorInput
        {
            get { return _errorInput; }
            set { _errorInput = value; OnPropertyChanged(nameof(ErrorInput)); }
        }

        // фон для Popup
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

        //  запуск Popup добавления товаров в заказ
        private bool _startPoupAddDishes { get; set; }
        public bool StartPoupAddDishes
        {
            get { return _startPoupAddDishes; }
            set
            {
                _startPoupAddDishes = value;
                OnPropertyChanged(nameof(StartPoupAddDishes));
            }
        }

        // видимость кнопки "добавить товар в заказ"
        private bool _isAddDishes { get; set; }
        public bool IsAddDishes
        {
            get { return _isAddDishes; }
            set
            {
                _isAddDishes = value; OnPropertyChanged(nameof(IsAddDishes));
            }
        }

        // название страницы
        private string _headingPage { get; set; }
        public string HeadingPage
        {
            get { return _headingPage; }
            set { _headingPage = value; OnPropertyChanged(nameof(HeadingPage)); }
        }

        // свойство для вывода ошибки при добавлении или редактировании данных
        private string _errorInputPopup { get; set; }
        public string ErrorInputPopup
        {
            get { return _errorInputPopup; }
            set { _errorInputPopup = value; OnPropertyChanged(nameof(ErrorInputPopup)); }
        }

        #endregion

        // работа выбора диапозона время и даты доставки
        #region DataTimeDelivry

        // настройка начального диапозона времени
        private async Task LimitationInitialDeliveryTime()
        {

            TimeSpan minTime = new TimeSpan(9, 0, 0);
            TimeSpan maxTime = new TimeSpan(20, 0, 0);
            TimeSpan currentTimeSpan = DateTime.Now.TimeOfDay; // получаем время сейчас

            if (SelectedStartTimeDelivery != null) // проверка, было ли выбранно время начального интервала доставки
            {
                TimeSpan selectedTime = SelectedStartTimeDelivery.TimeOfDay; // получаем время выбранное пользователем
                if (selectedTime < minTime) // если выбранное время меньше времени открытия доставки, то ставим
                                            // минимальную дату доставки, а также ближайшее время по конечному интервалу доставки
                {
                    SelectedStartTimeDelivery = new DateTime((int)DateTime.Now.Year, (int)DateTime.Now.Month, (int)DateTime.Now.Day, (int)minTime.TotalHours, (int)minTime.Minutes, (int)minTime.Seconds);
                    SelectedEndTimeDelivery = new DateTime((int)DateTime.Now.Year, (int)DateTime.Now.Month, (int)DateTime.Now.Day, 11, 0, 0);
                }
                else if (selectedTime > maxTime) // если выбранное время больше времени закрытия ссервиса доставки, то ставим
                                                 // максимальную дату доставки, а также ближайшее время по конечному интервалу доставки
                {
                    SelectedStartTimeDelivery = new DateTime((int)DateTime.Now.Year, (int)DateTime.Now.Month, (int)DateTime.Now.Day, (int)maxTime.TotalHours, (int)maxTime.Minutes, (int)maxTime.Seconds);
                    SelectedEndTimeDelivery = new DateTime((int)DateTime.Now.Year, (int)DateTime.Now.Month, (int)DateTime.Now.Day, 22, 0, 0);
                }
                else
                {
                    SelectedEndTimeDelivery = new DateTime((int)DateTime.Now.Year, (int)DateTime.Now.Month, (int)DateTime.Now.Day, (int)selectedTime.TotalHours + 2, (int)selectedTime.Minutes, (int)selectedTime.Seconds);
                }
            }

        }

        #endregion

        // поиск данных в таблице
        #region SearchCompositionOrder

        public async Task HandlerTextBoxChanged(string searchByValue)
        {
            searchByValue = searchByValue.Trim(); // убираем пробелы
            if (!string.IsNullOrWhiteSpace(searchByValue))
            {
                ListCompositionOrders = ListOrderCopy; // копируем список с последними изменениями
                await WeGetListOfDishes(); // обновляем список (нужно для того, если вдруг появились новые блюда в БД)
                // создаём список с поиском по введенным данным в таблице
                var searchResult = ListCompositionOrders.Where(c => c.nameDishes.ToLowerInvariant()
                .Contains(searchByValue.ToLowerInvariant())).ToList();

                ListCompositionOrders.Clear(); // очищаем список отображения данных в таблице
                // вносим актуальные данные основного списка с учётом фильтра
                ListCompositionOrders = new ObservableCollection<CompositionOrderDPO>(searchResult);
            }
            else
            {
                ListCompositionOrders.Clear(); // очищаем список отображения данных в таблице
                ListCompositionOrders = ListOrderCopy; // обновляем список
            }

            if (ListCompositionOrders.Count == 0)
            {
                ErrorInputPopup = "Блюдо не найдено!"; // собщение об ошибке
                BeginFadeAnimation(AnimationErrorInputPopup); // анимация затухания ошибки
            }
        }

        #endregion

        // анимации
        #region Animation

        // анимация затухания текста
        private async Task BeginFadeAnimation(TextBlock textBlock)
        {
            textBlock.IsEnabled = true;
            textBlock.Opacity = 1.0;

            Storyboard storyboard = new Storyboard();
            DoubleAnimation fadeAnimation = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                Duration = TimeSpan.FromSeconds(1)
            };
            Storyboard.SetTargetProperty(fadeAnimation, new PropertyPath(TextBlock.OpacityProperty));
            storyboard.Children.Add(fadeAnimation);
            storyboard.Completed += (s, e) => textBlock.IsEnabled = false;
            storyboard.Begin(textBlock);
        }

        // запуск анимации подсвечивания объекта
        private void StartFieldIllumination(TextBox textBox)
        {
            FieldIllumination.Begin(textBox);
        }
        private void StartFieldIllumination(ComboBox comboBox)
        {
            FieldIllumination.Begin(comboBox);
        }
        private void StartFieldIllumination(DatePicker datePicker)
        {
            FieldIllumination.Begin(datePicker);
        }
        private void StartFieldIllumination(TimePicker timePicker)
        {
            FieldIllumination.Begin(timePicker);
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
