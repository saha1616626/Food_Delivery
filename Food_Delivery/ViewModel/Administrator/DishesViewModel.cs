using Food_Delivery.Data;
using Food_Delivery.Helper;
using Food_Delivery.Model;
using Food_Delivery.Model.DPO;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace Food_Delivery.ViewModel.Administrator
{
    public class DishesViewModel : INotifyPropertyChanged
    {
        // путь к json работа окна Popup
        readonly string pathDataPopup = @"E:\3comm\Documents\Предметы\Курс 3.2\Курсовая\Приложение\Программа\Food_Delivery\Food_Delivery\Data\СheckPopup.json";

        public DishesViewModel()
        {
            GetListCategory();  // вывод данных в таблицу

            // после получения фокуса данного приложения запукаем закрытый Popup
            WorkingWithData._launchPopupAfterReceivingFocusDish += LaunchingPopupWhenGettingFocus;
        }

        // вывод данных в таблицу
        #region workWithTable

        // коллекция отображения данных в таблице
        private ObservableCollection<DishesDPO> _listDishes { get; set; } = new ObservableCollection<DishesDPO>();
        public ObservableCollection<DishesDPO> ListDishes
        {
            get { return _listDishes; }
            set { _listDishes = value; OnPropertyChanged(nameof(ListDishes)); }
        }

        // получаем данные из БД c последующим выводом в таблицу
        private async Task GetListCategory()
        {
            ListDishes.Clear(); // очищаем коллекцию перед заполнением
            using (FoodDeliveryContext foodDeliveryContext = new FoodDeliveryContext())
            {
                List<Dishes> dishes = await foodDeliveryContext.Dishes.ToListAsync();
                if(dishes.Count > 0)
                {
                    dishes.Reverse(); // переворачиваем список
                    // заменяем id на текстовую информацию
                    foreach(Dishes item in dishes)
                    {
                        DishesDPO dishesDPO = new DishesDPO();
                        dishesDPO = await dishesDPO.CopyFromDishes(item);
                        ListDishes.Add(dishesDPO);
                    }  
                }
            }
        }

        #endregion


        // Работа над добавлением, редактированием и удалением данных
        #region Popup

        // состояние: Popup добавление или редактирование; Popup удаление.
        private bool IsCheckAddAndEditOrDelete; // true - добавление или редактирование данных.

        // свойство определющее назаначение запуска Popup (редактирование или добавление данных)
        private bool IsAddData { get; set; } // true - добавление данных; false - редактирование данных
        
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
                        IsCheckAddAndEditOrDelete = true; // режим редактирования или добавления данных (удержания фокуса на Popup)
                        AddAndEditDataPopup.IsOpen = true; // отображаем Popup
                        DarkBackground.Visibility = Visibility.Visible; // показать фон
                        WorkingWithData.ExitHamburgerMenu(); // закрываем, если открыто "гамбургер меню"
                        HeadingPopup = "Добавить блюдо"; // изменяем заголовок Popup
                        ActionConfirmationButton = "Добавить"; // изменение названия кнопки подтверждения действия 
                        // добавляем данные в ComboBox
                        using(FoodDeliveryContext foodDeliveryContext = new FoodDeliveryContext())
                        {
                            List<Category> categories = await foodDeliveryContext.Categories.ToListAsync();

                            // добавляем категории в список
                            OptionsCategory = categories;
                        }

                        ClearingPopup(); // очищаем поля

                        NotificationOfThePopupLaunchJson(); // оповещаем JSON, чтомы запустили Popup

                    }, (obj) => true));
            }
        }

        // запускаем Popup для редактирования данных
        private RelayCommand _btn_OpenPopupToEditData { get; set; }
        public RelayCommand Btn_OpenPopupToEditData
        {
            get
            {
                return _btn_OpenPopupToEditData ??
                    (_btn_OpenPopupToEditData = new RelayCommand(async (obj) =>
                    {
                        ClearingPopup(); // очищаем поля
                        IsAddData = false; // изменяем режим работы Popup на режим добавления данных
                        IsCheckAddAndEditOrDelete = true; // режим редактирования или добавления данных (удержания фокуса на Popup)
                        AddAndEditDataPopup.IsOpen = true; // отображаем Popup
                        DarkBackground.Visibility = Visibility.Visible; // показать фон
                        WorkingWithData.ExitHamburgerMenu(); // закрываем, если открыто "гамбургер меню"
                        HeadingPopup = "Изменить категорию"; // изменяем заголовок Popup
                        ActionConfirmationButton = "Изменить"; // изменение названия кнопки подтверждения действия
                        // добавляем данные в ComboBox
                        using (FoodDeliveryContext foodDeliveryContext = new FoodDeliveryContext())
                        {
                            List<Category> categories = await foodDeliveryContext.Categories.ToListAsync();
                            List<Dishes> dishes = await foodDeliveryContext.Dishes.ToListAsync();

                            // добавляем категории в список
                            OptionsCategory = categories;

                            // ищем нужное блюдо для вывода информации в поля
                            Dishes dishesToChange = dishes.FirstOrDefault(d => d.id == SelectedDishes.id);
                            if(dishesToChange != null)
                            {
                                // отображаем выбранную категорию
                                Category category = categories.FirstOrDefault(c => c.id == dishesToChange.categoryId);// ищем категорию выбранного блюда
                                if (category != null)
                                {
                                    SelectedCategory = category;
                                }

                                OutNameDishes = dishesToChange.name;
                                if (!string.IsNullOrWhiteSpace(dishesToChange.description))
                                {
                                    OutNameDescription = dishesToChange.description;
                                }
                                if (!string.IsNullOrWhiteSpace(dishesToChange.calories.ToString()))
                                {
                                    OutСaloriesDishes = dishesToChange.calories.ToString();
                                }
                                if (!string.IsNullOrWhiteSpace(dishesToChange.squirrels.ToString()))
                                {
                                    OutSquirrelsDishes = dishesToChange.squirrels.ToString();
                                }
                                if (!string.IsNullOrWhiteSpace(dishesToChange.fats.ToString()))
                                {
                                    OutFatsDishes = dishesToChange.fats.ToString();
                                }
                                if (!string.IsNullOrWhiteSpace(dishesToChange.carbohydrates.ToString()))
                                {
                                    OutСarbohydratesDishes = dishesToChange.carbohydrates.ToString();
                                }
                                if (!string.IsNullOrWhiteSpace(dishesToChange.weight.ToString()))
                                {
                                    OutWeightDishes = dishesToChange.weight.ToString();
                                }
                                OutQuantityDishes = dishesToChange.quantity.ToString();
                                OutPriceDishes = dishesToChange.price.ToString();
                                // преобразуем фото в формат CroppedBitmap для отображения
                                CroppedBitmap croppedBitmap = ResizingPhotos(dishesToChange.image, 200);
                                Image = croppedBitmap;
                                imageBd = dishesToChange.image; // выбираем изображение для дальнейшей работы
                                IsCheckedStopList = dishesToChange.stopList;
                            }

                        }

                        NotificationOfThePopupLaunchJson(); // оповещаем JSON, чтомы запустили Popup

                    }, (obj) => true));
            }
        }

        // запускаем Popup для удаления данных
        private RelayCommand _btn_OpenPopupToDeleteData { get; set; }
        public RelayCommand Btn_OpenPopupToDeleteData
        {
            get
            {
                return _btn_OpenPopupToDeleteData ??
                    (_btn_OpenPopupToDeleteData = new RelayCommand((obj) =>
                    {
                        DeleteDataPopup.IsOpen = true; // отображаем Popup
                        IsCheckAddAndEditOrDelete = false; // режим редактирования или добавления данных (удержания фокуса на Popup)
                        DarkBackground.Visibility = Visibility.Visible; // показать фон
                        WorkingWithData.ExitHamburgerMenu(); // закрываем, если открыто "гамбургер меню"
                        // отображаем название категории перед удалением в Popup
                        if (!SelectedDishes.name.IsNullOrEmpty())
                        {
                            NameOfDishesDeleted = "Выбранная блюдо: " + SelectedDishes.name;
                        }

                        NotificationOfThePopupLaunchJson(); // оповещаем JSON, чтомы запустили Popup

                    }, (obj) => true));
            }
        }

        // удаление данных
        private RelayCommand _btn_DeleteData { get; set; }
        public RelayCommand Btn_DeleteData
        {
            get
            {
                return _btn_DeleteData ??
                    (_btn_DeleteData = new RelayCommand(async (obj) =>
                    {
                        using (FoodDeliveryContext foodDeliveryContext = new FoodDeliveryContext())
                        {
                            // ищем нужную категорию для удаления
                            Dishes dishes = await foodDeliveryContext.Dishes.FirstOrDefaultAsync(c => c.id == SelectedDishes.id);
                            if (dishes != null)
                            {
                                foodDeliveryContext.Dishes.Remove(dishes);
                                await foodDeliveryContext.SaveChangesAsync(); // cохраняем изменения в базе данных                       
                                ClosePopupWorkingWithData(); // закрываем Popup
                                GetListCategory(); // обновляем список
                            }
                        }
                    }, (obj) => true));
            }
        }

        // очищаем Popup после закрытия
        private async Task ClearingPopup()
        {
            OutNameDishes = ""; // название
            OutNameDescription = ""; // описание
            SelectedCategory = null; // категория
            OutPriceDishes = ""; // цена блюда
            OutСaloriesDishes = ""; // кол-во каллорий
            OutSquirrelsDishes = ""; // кол-во белков
            OutFatsDishes = ""; // кол-во жиров
            OutСarbohydratesDishes = ""; // кол-во углеводов
            OutWeightDishes = ""; // вес блюда
            OutQuantityDishes = ""; // кол-во штук в упаковке
            IsCheckedStopList = default; // чек-бокс
            Image = null; // изображение
        }

        // получаем изображение при добавлении или редактировании записи
        private RelayCommand _addImage { get; set; }
        public RelayCommand AddImage
        {
            get
            {
                return _addImage ??
                    (_addImage = new RelayCommand((obj) =>
                    {
                        // создание диалогового окна выбора файла
                        Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
                        openFileDialog.Filter = "Изображения (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png";

                        // открытие диалогового окна
                        if (openFileDialog.ShowDialog() == true)
                        {
                            // загрузка изображения из выбранного файла
                            byte[] image;
                            using (var fileSream = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read))
                            {
                                image = new byte[fileSream.Length];
                                fileSream.Read(image, 0, (int)fileSream.Length);
                            }
                            if (image.Length > 0 || image != null)
                            {
                                // изменяем размер изображения, сохраняя пропорции
                                int desiredSize = 200; // Pазмер изображения
                                CroppedBitmap croppedBitmap = ResizingPhotos(image, desiredSize);

                                // преобразуем CroppedBitmap в byte[] для того, чтобы измененное
                                // изображение передать в переменную для дальнейшей работы с БД

                                // преобразование CroppedBitmap в BitmapSource
                                BitmapSource bitmapSource = croppedBitmap;

                                // создание MemoryStream для записи данных
                                using (MemoryStream ms = new MemoryStream())
                                {
                                    // сохранение BitmapSource в MemoryStream в формате PNG
                                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                                    encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                                    encoder.Save(ms);

                                    imageBd = ms.ToArray(); // получаем массив байтов из MemoryStream
                                }

                                Image = croppedBitmap; // выводим изображение на экран
                            }
                        }

                        // после закрытия Popup закрывается, поэтому мы запускаем его снова
                        if (IsAddData) // добавление данных
                        {
                            IsAddData = true; // изменяем режим работы Popup на режим добавления данных
                            AddAndEditDataPopup.IsOpen = true; // отображаем Popup
                            DarkBackground.Visibility = Visibility.Visible; // показать фон
                            WorkingWithData.ExitHamburgerMenu(); // закрываем, если открыто "гамбургер меню"
                            HeadingPopup = "Добавить блюдо"; // изменяем заголовок Popup
                            ActionConfirmationButton = "Добавить"; // изменение названия кнопки подтверждения действия
                        }
                        else
                        {
                            IsAddData = false; // изменяем режим работы Popup на режим редактирования данных
                            AddAndEditDataPopup.IsOpen = true; // отображаем Popup
                            DarkBackground.Visibility = Visibility.Visible; // показать фон
                            WorkingWithData.ExitHamburgerMenu(); // закрываем, если открыто "гамбургер меню"
                            HeadingPopup = "Изменить блюдо"; // изменяем заголовок Popup
                            ActionConfirmationButton = "Изменить"; // изменение названия кнопки подтверждения действия
                        }
                          
                    }, (obj) => true));
            }
        }

        // обрезаем изображение под нужные параметры
        public CroppedBitmap ResizingPhotos(byte[] image, int desiredSize)
        {
            // создание BitmapImage из загруженного изображения
            BitmapImage selectedImage = new BitmapImage();
            selectedImage.BeginInit();
            selectedImage.StreamSource = new MemoryStream(image);
            selectedImage.EndInit();

            int width = selectedImage.PixelWidth; // получаем ширину
            int height = selectedImage.PixelHeight; // получаем высоту
            double aspectRatio = (double)width / height;

            if (aspectRatio > 1) // если ширина больше высоты, то мы устанавливаем нужный размер высоты, а ширину исходя из пропорций
            {
                width = desiredSize;
                height = (int)(desiredSize / aspectRatio);
            }
            else // если высота больше ширины, то высоте ставим нужный размер, а ширину выставляем исходя из пропорций
            {
                height = desiredSize;
                width = (int)(desiredSize * aspectRatio);
            }

            // масштабируем изображение
            TransformedBitmap resizedBitmap = new TransformedBitmap(selectedImage, new ScaleTransform(
                (double)width / selectedImage.PixelWidth,
                (double)height / selectedImage.PixelHeight));

            // обрезаем изображение под квадрат
            int croppedSize = Math.Min(resizedBitmap.PixelWidth, resizedBitmap.PixelHeight); // находим самую корткую сторону
                                                                                             // получаем положение центральной точки в изображении для ровного обрезания
            int x = (resizedBitmap.PixelWidth - croppedSize) / 2;
            int y = (resizedBitmap.PixelHeight - croppedSize) / 2;

            CroppedBitmap croppedBitmap = new CroppedBitmap(resizedBitmap, new Int32Rect(x, y, croppedSize, croppedSize));

            return croppedBitmap;
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
                        ClosePopupWorkingWithData(); // метод закрытия Popup
                    }, (obj) => true));
            }
        }

        // закрываем Popup
        private async Task ClosePopupWorkingWithData()
        {
            // Закрываем Popup
            AddAndEditDataPopup.IsOpen = false;
            DeleteDataPopup.IsOpen = false;
            DarkBackground.Visibility = Visibility.Collapsed; // скрываем фон
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
                        if (!string.IsNullOrWhiteSpace(OutNameDishes) && SelectedCategory != null && 
                        !string.IsNullOrWhiteSpace(OutPriceDishes) && !string.IsNullOrWhiteSpace(OutQuantityDishes) && imageBd != null)
                        {
                            // проверяем корректность введенных данных
                            bool isCheckingPriceDishes = false; // переменная корректности введенных чисел
                            bool isCheckingСaloriesDishes = false; // переменная корректности введенных чисел
                            bool isCheckingFatsDishes = false; // переменная корректности введенных чисел
                            bool isCheckingSquirrelsDishes = false; // переменная корректности введенных чисел
                            bool isCheckingСarbohydratesDishes = false; // переменная корректности введенных чисел
                            bool isCheckingWeightDishes = false; // переменная корректности введенных чисел
                            bool isCheckingQuantityDishes = false; // переменная корректности введенных чисел

                            int price = 0; // цена
                            int calories = 0; // калории
                            int squirrels = 0; // белки
                            int fats = 0; // жиры
                            int carbohydrates = 0; // углеводы
                            int weight = 0; // вес блюда
                            int quantity = 0; // кол-во в упаковке

                            ErrorInputPopup = ""; // очищаем сообщение об ошибке

                            // проверяем цело число в поле "Цена"
                            isCheckingPriceDishes = int.TryParse(OutPriceDishes.Trim(), out price);
                            if (!isCheckingPriceDishes) // число не получено
                            {
                                StartFieldIllumination(AnimationPrice); // подсветка поля
                                ErrorInputPopup = "Введите целое число!"; // сообщение с обибкой
                            }

                            if (!string.IsNullOrWhiteSpace(OutСaloriesDishes)) // если поле не нулевое
                            {
                                // проверяем цело число в поле "калории"
                                isCheckingСaloriesDishes = int.TryParse(OutСaloriesDishes.Trim(), out calories);
                                if (!isCheckingСaloriesDishes) // число не получено
                                {
                                    StartFieldIllumination(AnimationСalories); // подсветка поля
                                    ErrorInputPopup = "Введите целое число!"; // сообщение с обибкой
                                }
                            }

                            if (!string.IsNullOrWhiteSpace(OutFatsDishes)) // если поле не нулевое
                            {
                                // проверяем цело число в поле "жиры"
                                isCheckingFatsDishes = int.TryParse(OutFatsDishes.Trim(), out fats);
                                if (!isCheckingFatsDishes) // число не получено
                                {
                                    StartFieldIllumination(AnimationFats); // подсветка поля
                                    ErrorInputPopup = "Введите целое число!"; // сообщение с обибкой
                                }
                            }

                            if (!string.IsNullOrWhiteSpace(OutSquirrelsDishes)) // если поле не нулевое
                            {
                                // проверяем цело число в поле "белки"
                                isCheckingSquirrelsDishes = int.TryParse(OutSquirrelsDishes.Trim(), out squirrels);
                                if (!isCheckingSquirrelsDishes) // число не получено
                                {
                                    StartFieldIllumination(AnimationSquirrels); // подсветка поля
                                    ErrorInputPopup = "Введите целое число!"; // сообщение с обибкой
                                }
                            }

                            if (!string.IsNullOrWhiteSpace(OutСarbohydratesDishes)) // если поле не нулевое
                            {
                                // проверяем цело число в поле "углеводы"
                                isCheckingСarbohydratesDishes = int.TryParse(OutСarbohydratesDishes.Trim(), out carbohydrates);
                                if (!isCheckingСarbohydratesDishes) // число не получено
                                {
                                    StartFieldIllumination(AnimationCarbohydrates); // подсветка поля
                                    ErrorInputPopup = "Введите целое число!"; // сообщение с обибкой
                                }
                            }

                            if (!string.IsNullOrWhiteSpace(OutWeightDishes)) // если поле не нулевое
                            {
                                // проверяем цело число в поле "белки"
                                isCheckingWeightDishes = int.TryParse(OutWeightDishes.Trim(), out weight);
                                if (!isCheckingWeightDishes) // число не получено
                                {
                                    StartFieldIllumination(AnimationWeight); // подсветка поля
                                    ErrorInputPopup = "Введите целое число!"; // сообщение с обибкой
                                }
                            }

                            // проверяем цело число в поле "Кол-во в упаковке"
                            isCheckingQuantityDishes = int.TryParse(OutQuantityDishes.Trim(), out quantity);
                            if (!isCheckingQuantityDishes) // число не получено
                            {
                                StartFieldIllumination(AnimationQuantity); // подсветка поля
                                ErrorInputPopup = "Введите целое число!"; // сообщение с обибкой
                            }

                            BeginFadeAnimation(AnimationErrorInputPopup); // затухание сообщения об ошибке

                            if (isCheckingPriceDishes && isCheckingСaloriesDishes &&
                            isCheckingFatsDishes && isCheckingSquirrelsDishes &&
                            isCheckingСarbohydratesDishes && isCheckingWeightDishes &&
                            isCheckingQuantityDishes) // если все данные корректны
                            {
                                if (IsAddData) // добавление данных
                                {
                                    // проверяем наличие дубликата в БД
                                    using (FoodDeliveryContext foodDeliveryContext = new FoodDeliveryContext())
                                    {
                                        List<Dishes> dishes = await foodDeliveryContext.Dishes.ToListAsync();
                                        if (!dishes.Any(d => d.name.ToLowerInvariant().Contains(OutNameDishes.ToLowerInvariant().Trim())))
                                        {
                                            // добавляем данные в БД
                                            Dishes dis = new Dishes();
                                            dis.name = OutNameDishes.Trim();
                                            if (!string.IsNullOrWhiteSpace(OutNameDescription))
                                            {
                                                dis.description = OutNameDescription.Trim();
                                            }
                                            dis.categoryId = SelectedCategory.id;
                                            if (!string.IsNullOrWhiteSpace(OutСaloriesDishes))
                                            {
                                                dis.calories = calories;
                                            }
                                            if (!string.IsNullOrWhiteSpace(OutSquirrelsDishes))
                                            {
                                                dis.squirrels = squirrels;
                                            }
                                            if (!string.IsNullOrWhiteSpace(OutFatsDishes))
                                            {
                                                dis.fats = fats;
                                            }
                                            if (!string.IsNullOrWhiteSpace(OutСarbohydratesDishes))
                                            {
                                                dis.carbohydrates = carbohydrates;
                                            }
                                            if (!string.IsNullOrWhiteSpace(OutWeightDishes))
                                            {
                                                dis.weight = weight;
                                            }
                                            if (!string.IsNullOrWhiteSpace(OutQuantityDishes))
                                            {
                                                dis.quantity = quantity;
                                            }
                                            if (!string.IsNullOrWhiteSpace(OutPriceDishes))
                                            {
                                                dis.price = price;
                                            }
                                            dis.image = imageBd;
                                            if(IsCheckedStopList != null)
                                            {
                                                dis.stopList = IsCheckedStopList;
                                            }

                                            await foodDeliveryContext.Dishes.AddAsync(dis);
                                            await foodDeliveryContext.SaveChangesAsync(); // cохраняем изменения в базе данных
                                            ClosePopupWorkingWithData(); // закрываем Popup
                                            GetListCategory(); // обновляем список
                                            ClearingPopup(); // очищаем поля
                                        }
                                        else
                                        {
                                            // данные уже существуют
                                            StartFieldIllumination(AnimationOutName); // подсветка поля
                                            ErrorInputPopup = "Категория с таким названием уже существует!"; // сообщение с обибкой
                                            BeginFadeAnimation(AnimationErrorInputPopup); // затухание сообщения об ошибке
                                        }
                                    }
                                }
                                else // редактирование данных
                                {
                                    // проверяем наличие дубликата в БД (за исключением изменяемого объекта)
                                    using (FoodDeliveryContext foodDeliveryContext = new FoodDeliveryContext())
                                    {
                                        List<Dishes> dishes = await foodDeliveryContext.Dishes.ToListAsync();
                                        dishes = dishes.Where(d => d.id != SelectedDishes.id).ToList(); // исключаем элемент из поиска совпадений,
                                                                                                        // который мы выбрали для редактирования
                                        if(!dishes.Any(d => d.name.ToLowerInvariant() == OutNameDishes.ToLowerInvariant().Trim()))
                                        {
                                            // находим объект для изменения в БД
                                            Dishes dishesToChange = await foodDeliveryContext.Dishes.FirstOrDefaultAsync(d => d.id == SelectedDishes.id);
                                            if(dishesToChange != null)
                                            {
                                                // изменяем данные
                                                dishesToChange.name = OutNameDishes.Trim();
                                                if (!string.IsNullOrWhiteSpace(OutNameDescription))
                                                {
                                                    dishesToChange.description = OutNameDescription.Trim();
                                                }
                                                dishesToChange.categoryId = SelectedCategory.id;
                                                if (!string.IsNullOrWhiteSpace(OutСaloriesDishes))
                                                {
                                                    dishesToChange.calories = calories;
                                                }
                                                if (!string.IsNullOrWhiteSpace(OutSquirrelsDishes))
                                                {
                                                    dishesToChange.squirrels = squirrels;
                                                }
                                                if (!string.IsNullOrWhiteSpace(OutFatsDishes))
                                                {
                                                    dishesToChange.fats = fats;
                                                }
                                                if (!string.IsNullOrWhiteSpace(OutСarbohydratesDishes))
                                                {
                                                    dishesToChange.carbohydrates = carbohydrates;
                                                }
                                                if (!string.IsNullOrWhiteSpace(OutWeightDishes))
                                                {
                                                    dishesToChange.weight = weight;
                                                }
                                                dishesToChange.quantity = quantity;
                                                dishesToChange.price = price;
                                                dishesToChange.image = imageBd;
                                                if (IsCheckedStopList != null)
                                                {
                                                    dishesToChange.stopList = IsCheckedStopList;
                                                }

                                                await foodDeliveryContext.SaveChangesAsync(); // cохраняем изменения в базе данных
                                                ClosePopupWorkingWithData(); // закрываем Popup
                                                GetListCategory(); // обновляем список
                                                ClearingPopup(); // очищаем поля

                                            }
                                        }
                                        else
                                        {
                                            // данные уже существуют
                                            StartFieldIllumination(AnimationOutName); // подсветка поля
                                            ErrorInputPopup = "Блюдо с таким названием уже существует!"; // сообщение с обибкой
                                            BeginFadeAnimation(AnimationErrorInputPopup); // затухание сообщения об ошибке
                                        }

                                    }
                                }
                            }
                        }
                        else
                        {
                            ErrorInputPopup = ""; // очищаем сообщение об ошибке

                            // проверяем заполнение обязательных полей

                            if (string.IsNullOrWhiteSpace(OutNameDishes))
                            {
                                StartFieldIllumination(AnimationOutName); // подсветка поля
                                ErrorInputPopup = "Заполните обязательные поля!"; // сообщение с ошибкой
                            }

                            if(SelectedCategory == null)
                            {
                                StartFieldIllumination(AnimationCbCategory);
                                ErrorInputPopup = "Заполните обязательные поля!"; // сообщение с обибкой
                            }

                            if(string.IsNullOrWhiteSpace(OutPriceDishes))
                            {
                                StartFieldIllumination(AnimationPrice);
                                ErrorInputPopup = "Заполните обязательные поля!"; // сообщение с обибкой
                            }

                            if (string.IsNullOrWhiteSpace(OutQuantityDishes))
                            {
                                StartFieldIllumination(AnimationQuantity);
                                ErrorInputPopup = "Заполните обязательные поля!"; // сообщение с обибкой
                            }

                            if(imageBd == null)
                            {
                                ErrorInputPopup += "\nВыберете изображение!";
                            }

                            BeginFadeAnimation(AnimationErrorInputPopup); // затухание сообщения об ошибке
                        }

                    }, (obj) => true));
            }
        }

        // запускаем Popup (для редактирования или удаления)
        private void LaunchingPopupWhenGettingFocus(object sender, EventAggregator eventAggregator)
        {
            if (IsCheckAddAndEditOrDelete) // если это добавление или редактирование
            {
                AddAndEditDataPopup.IsOpen = true; // отображаем Popup
            }
            else // если это удаление данных 
            {
                DeleteDataPopup.IsOpen = true; // отображаем Popup
            }
            DarkBackground.Visibility = Visibility.Visible; // показать фон
            WorkingWithData.ExitHamburgerMenu(); // закрываем, если открыто "гамбургер меню"
        }

        // записываем в JSON, что мы запустили Popup данной страницы
        private void NotificationOfThePopupLaunchJson()
        {
            // передаём в JSON, что мы запустили Popup
            var jsonData = new { popup = "Dishes" };
            // Преобразуем объект в JSON-строку
            string jsonText = JsonConvert.SerializeObject(jsonData);
            File.WriteAllText(pathDataPopup, jsonText);
        }

        #endregion

        // свойства
        #region Features

        public Border DarkBackground { get; set; } // затемненный фон позади Popup
        public Popup AddAndEditDataPopup { get; set; } // ссылка на Popup для редактирования или добавления данных
        public TextBlock AnimationErrorInput { get; set; } // текстовое поле для вывода ошибки при поиске данных
        public TextBlock AnimationErrorInputPopup { get; set; } // текстовое поле для вывода ошибки при добавление или редактировании данных в Popup
        public TextBox AnimationOutName { get; set; } // поле для ввода текста "название блюда". Вывод подсветки поля
        public TextBox AnimationOutDescription { get; set; } // поле для ввода текста "описание блюда". Вывод подсветки поля
        public ComboBox AnimationCbCategory { get; set; } // поле для выбора категории из списка
        public TextBox AnimationPrice { get; set; } // поле для ввода текста "цена блюда". Вывод подсветки поля
        public TextBox AnimationQuantity { get; set; } // поле для ввода текста "кол-во в упаковке". Вывод подсветки поля
        public TextBox AnimationСalories { get; set; } // поле для ввода текста "кол-во калорий". Вывод подсветки поля
        public TextBox AnimationSquirrels { get; set; } // поле для ввода текста "кол-во белков". Вывод подсветки поля
        public TextBox AnimationFats { get; set; } // поле для ввода текста "кол-во жиров". Вывод подсветки поля
        public TextBox AnimationCarbohydrates { get; set; } // поле для ввода текста "кол-во углеводов". Вывод подсветки поля
        public TextBox AnimationWeight { get; set; } // поле для ввода текста "вес". Вывод подсветки поля
        public Storyboard FieldIllumination { get; set; } // анимация объектов
        public Popup DeleteDataPopup { get; set; } // ссылка на Popup для удаления данных

        // ассинхронно получаем информацию из DishesPage 
        public async Task InitializeAsync(Popup AddAndEditDataPopup, Border DarkBackground, Storyboard FieldIllumination, TextBox AnimationOutName,
            TextBlock AnimationErrorInputPopup, ComboBox AnimationCbCategory, TextBox AnimationPrice, TextBox AnimationQuantity, 
            TextBox AnimationСalories, TextBox AnimationSquirrels, TextBox AnimationFats, TextBox AnimationCarbohydrates,
            TextBox AnimationWeight, Popup DeleteDataPopup, TextBlock AnimationErrorInput)
        {
            if (AddAndEditDataPopup != null)
            {
                this.AddAndEditDataPopup = AddAndEditDataPopup;
            }
            if (DarkBackground != null)
            {
                this.DarkBackground = DarkBackground;
            }
            if(AnimationOutName != null)
            {
                this.AnimationOutName = AnimationOutName;
            }
            if(AnimationErrorInputPopup != null)
            {
                this.AnimationErrorInputPopup = AnimationErrorInputPopup;
            }
            if(FieldIllumination != null)
            {
                this.FieldIllumination = FieldIllumination;
            }
            if(AnimationCbCategory != null)
            {
                this.AnimationCbCategory = AnimationCbCategory;
            }
            if(AnimationPrice != null)
            {
                this.AnimationPrice = AnimationPrice;
            }
            if(AnimationQuantity != null)
            {
                this.AnimationQuantity = AnimationQuantity;
            }
            if(AnimationСalories != null)
            {
                this.AnimationСalories = AnimationСalories;
            }
            if(AnimationSquirrels != null)
            {
                this.AnimationSquirrels = AnimationSquirrels;
            }
            if(AnimationFats != null)
            {
                this.AnimationFats = AnimationFats;
            }
            if(AnimationCarbohydrates != null)
            {
                this.AnimationCarbohydrates = AnimationCarbohydrates;
            }
            if(AnimationWeight != null)
            {
                this.AnimationWeight = AnimationWeight;
            }
            if(DeleteDataPopup != null)
            {
                this.DeleteDataPopup = DeleteDataPopup;
            }
            if(AnimationErrorInput != null)
            {
                this.AnimationErrorInput = AnimationErrorInput;
            }
        }

        // свойство для вывода ошибки при поиске данных в таблице
        private string _errorInput { get; set; }
        public string ErrorInput
        {
            get { return _errorInput; }
            set { _errorInput = value; OnPropertyChanged(nameof(ErrorInput)); }
        }

        // отображение название блюда в Popup для удаления данных
        private string _nameOfDishesDeleted { get; set; }
        public string NameOfDishesDeleted
        {
            get { return _nameOfDishesDeleted; }
            set { _nameOfDishesDeleted = value; OnPropertyChanged(nameof(NameOfDishesDeleted)); }
        }

        // выбранное блюдо
        private DishesDPO _selectedDishes { get; set; }
        public DishesDPO SelectedDishes
        {
            get { return _selectedDishes; }
            set { _selectedDishes = value; OnPropertyChanged(nameof(SelectedDishes)); OnPropertyChanged(nameof(IsWorkButtonEnable)); }
        }

        // отображение кнопки "удалить" и "редакировать"
        private bool _isWorkButtonEnable { get; set; }
        public bool IsWorkButtonEnable
        {
            get { return SelectedDishes != null; } // если в таблице выбранн объект, то кнопки работают
            set { _isWorkButtonEnable = value; OnPropertyChanged(nameof(IsWorkButtonEnable)); }
        }

        // свойство для вывода ошибки при добавлении или редактировании данных
        private string _errorInputPopup { get; set; }
        public string ErrorInputPopup
        {
            get { return _errorInputPopup; }
            set { _errorInputPopup = value; OnPropertyChanged(nameof(ErrorInputPopup)); }
        }

        // название блюда
        private string _outNameDishes { get; set; }
        public string OutNameDishes
        {
            get { return _outNameDishes; }
            set { _outNameDishes = value; OnPropertyChanged(nameof(OutNameDishes)); }
        }

        // описание блюда
        private string _outNameDescription { get; set; }
        public string OutNameDescription
        {
            get { return _outNameDescription; }
            set { _outNameDescription = value; OnPropertyChanged(nameof(OutNameDescription)); }
        }

        // выбранная категория
        private Category _selectedCategory { get; set; }
        public Category SelectedCategory
        {
            get { return _selectedCategory; }
            set { _selectedCategory = value; OnPropertyChanged(nameof(SelectedCategory)); }
        }

        // ComBox категории
        private List<Category> _optionsCategory { get; set; }
        public List<Category> OptionsCategory
        {
            get { return _optionsCategory; }
            set
            {
                _optionsCategory = value;
                OnPropertyChanged(nameof(OptionsCategory));
            }
        }

        // цена блюда
        private string _outPriceDishes { get; set; }
        public string OutPriceDishes
        {
            get { return _outPriceDishes; }
            set { _outPriceDishes = value; OnPropertyChanged(nameof(OutPriceDishes)); }
        }

        // кол-во каллорий
        private string _outСaloriesDishes { get; set; }
        public string OutСaloriesDishes
        {
            get { return _outСaloriesDishes; }
            set { _outСaloriesDishes = value; OnPropertyChanged(nameof(OutСaloriesDishes)); }
        }

        // кол-во белков
        private string _outSquirrelsDishes { get; set; }
        public string OutSquirrelsDishes
        {
            get { return _outSquirrelsDishes; }
            set { _outSquirrelsDishes = value; OnPropertyChanged(nameof(OutSquirrelsDishes)); }
        }

        // кол-во жиров
        private string _outFatsDishes { get; set; }
        public string OutFatsDishes
        {
            get { return _outFatsDishes; }
            set { _outFatsDishes = value; OnPropertyChanged(nameof(OutFatsDishes)); }
        }

        // кол-во углеводов
        private string _outСarbohydratesDishes { get; set; }
        public string OutСarbohydratesDishes
        {
            get { return _outСarbohydratesDishes; }
            set { _outСarbohydratesDishes = value; OnPropertyChanged(nameof(OutСarbohydratesDishes)); }
        }

        // вес блюда
        private string _outWeightDishes { get; set; }
        public string OutWeightDishes
        {
            get { return _outWeightDishes; }
            set { _outWeightDishes = value; OnPropertyChanged(nameof(OutWeightDishes)); }
        }

        // кол-во штук в упаковке
        private string _outQuantityDishes { get; set; }
        public string OutQuantityDishes
        {
            get { return _outQuantityDishes; }
            set { _outQuantityDishes = value; OnPropertyChanged(nameof(OutQuantityDishes)); }
        }

        // изображение выбранное пользователем
        public byte[] imageBd { get; set; }

        // чек-бокс
        private bool _isCheckedStopList { get; set; }
        public bool IsCheckedStopList
        {
            get { return _isCheckedStopList; }
            set
            {
                _isCheckedStopList = value;
                OnPropertyChanged(nameof(IsCheckedStopList));
            }
        }

        // получаем объект для установки предпросмотра изображения
        private CroppedBitmap _image { get; set; }
        public CroppedBitmap Image
        {
            get { return _image; }
            set
            {
                _image = value;
                OnPropertyChanged(nameof(Image));
            }
        }

        // название кнопки для подверждения действия при удалении или редактировании
        private string _actionConfirmationButton { get; set; }
        public string ActionConfirmationButton
        {
            get { return _actionConfirmationButton; }
            set { _actionConfirmationButton = value; OnPropertyChanged(nameof(ActionConfirmationButton)); }
        }

        // свойство заголовка Popup
        private string _headingPopup { get; set; }
        public string HeadingPopup
        {
            get { return _headingPopup; }
            set { _headingPopup = value; OnPropertyChanged(nameof(HeadingPopup)); }
        }

        #endregion

        // поиск данных в таблице
        #region CategorySearch

        // список для фильтров таблицы
        public ObservableCollection<DishesDPO> ListSearch { get; set; } = new ObservableCollection<DishesDPO>();

        public async Task HandlerTextBoxChanged(string searchByValue)
        {
            searchByValue = searchByValue.Trim(); // убираем пробелы
            if (!string.IsNullOrWhiteSpace(searchByValue))
            {
                await GetListCategory(); // обновляем список
                ListSearch = ListDishes; // присваиваем список из таблицы
                // создаём список с поиском по введенным данным в таблице
                var searchResult = ListSearch.Where(c => c.name.ToLowerInvariant()
                .Contains(searchByValue.ToLowerInvariant())).ToList();

                ListDishes.Clear(); // очищаем список отображения данных в таблице
                // вносим актуальные данные основного списка с учётом фильтра
                ListDishes = new ObservableCollection<DishesDPO>(searchResult);
            }
            else
            {
                ListDishes.Clear(); // очищаем список отображения данных в таблице
                await GetListCategory(); // обновляем список
            }

            if (ListDishes.Count == 0)
            {
                ErrorInput = "Категория не найдена!"; // собщение об ошибке
                BeginFadeAnimation(AnimationErrorInput); // анимация затухания ошибки
            }
        }

        #endregion

        // анимации
        #region Animation

        // анимация затухания текста
        private void BeginFadeAnimation(TextBlock textBlock)
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

        // запуск анимацию подсвечивание объекта
        private void StartFieldIllumination(TextBox textBox)
        {
            FieldIllumination.Begin(textBox);
        }
        private void StartFieldIllumination(ComboBox comboBox)
        {
            FieldIllumination.Begin(comboBox);
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
