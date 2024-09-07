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
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Food_Delivery.ViewModel.Administrator
{
    public class DishesViewModel : INotifyPropertyChanged
    {
        // путь к json работа окна Popup
        readonly string pathDataPopup = @"E:\3comm\Documents\Предметы\Курс 3.2\Курсовая\Приложение\Программа\Food_Delivery\Food_Delivery\Data\СheckPopup.json";

        public DishesViewModel()
        {


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

                        // передаём в JSON, что мы запустили Popup
                        var jsonData = new { popup = "Dishes" };
                        // Преобразуем объект в JSON-строку
                        string jsonText = JsonConvert.SerializeObject(jsonData);
                        File.WriteAllText(pathDataPopup, jsonText);

                    }, (obj) => true));
            }
        }

        // очищаем Popup после закрытия
        private async Task ClearingPopup()
        {
            OutNameDishes = ""; // название
            OutNameDescription = ""; // описание
            SelectedCategory = ""; // категория
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
                                
                                // передаём набор байтов в переменную для дальнейшей работы с БД
                                imageBd = image;

                                // изменяем размер изображения, сохраняя пропорции
                                int desiredSize = 200; // Pазмер изображения
                                CroppedBitmap croppedBitmap = ResizingPhotos(image, desiredSize);
                                Image = croppedBitmap; // выводим изображение на экран
                            }
                        }

                        // после закрытия Popup закрывается, поэтому мы запускаем его снова
                        IsAddData = true; // изменяем режим работы Popup на режим добавления данных
                        AddAndEditDataPopup.IsOpen = true; // отображаем Popup
                        DarkBackground.Visibility = Visibility.Visible; // показать фон
                        WorkingWithData.ExitHamburgerMenu(); // закрываем, если открыто "гамбургер меню"
                        HeadingPopup = "Добавить блюдо"; // изменяем заголовок Popup
                        ActionConfirmationButton = "Добавить"; // изменение названия кнопки подтверждения действия

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

        // скрываем Popup с кнопки
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
            //DeleteDataPopup.IsOpen = false;
            DarkBackground.Visibility = Visibility.Collapsed; // скрываем фон
        }

        // запускаем Popup (для редактирования или удаления)
        private void LaunchingPopupWhenGettingFocus(object sender, EventAggregator eventAggregator)
        {
            if (IsCheckAddAndEditOrDelete) // если это добавление или редактирование
            {
                AddAndEditDataPopup.IsOpen = true; // отображаем Popup
                DarkBackground.Visibility = Visibility.Visible; // показать фон
                WorkingWithData.ExitHamburgerMenu(); // закрываем, если открыто "гамбургер меню"
            }
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
        //public TextBox AnimationOutCategoryName { get; set; } // поле для ввода текста "название категории". Вывод подсветки поля

        // ассинхронно получаем информацию из DishesPage 
        public async Task InitializeAsync(Popup AddAndEditDataPopup, Border DarkBackground)
        {
            if (AddAndEditDataPopup != null)
            {
                this.AddAndEditDataPopup = AddAndEditDataPopup;
            }
            if (DarkBackground != null)
            {
                this.DarkBackground = DarkBackground;
            }
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
        private string _selectedCategory { get; set; }
        public string SelectedCategory
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

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
