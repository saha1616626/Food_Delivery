using Food_Delivery.Data;
using Food_Delivery.Helper;
using Food_Delivery.Model;
using MaterialDesignColors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.IdentityModel.Tokens;
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
using System.Windows.Media.Animation;

namespace Food_Delivery.ViewModel.Administrator
{
    class CategoryViewModel : INotifyPropertyChanged
    {
        // путь к json работа окна Popup
        readonly string pathDataPopup = @"E:\3comm\Documents\Предметы\Курс 3.2\Курсовая\Приложение\Программа\Food_Delivery\Food_Delivery\Data\СheckPopup.json";

        public CategoryViewModel()
        {
            GetListCategory(); // вывод данных в таблицу

            // после получения фокуса данного приложения запукаем закрытый Popup
            WorkingWithData._launchPopupAfterReceivingFocusCategory += LaunchingPopupWhenGettingFocus;
        }

        // вывод данных в таблицу
        #region workWithTable

        // коллекция отображения данных в таблице
        private ObservableCollection<Category> _listCategory { get; set; } = new ObservableCollection<Category>();
        public ObservableCollection<Category> ListCategory
        {
            get { return _listCategory; }
            set { _listCategory = value; OnPropertyChanged(nameof(ListCategory)); }
        }

        // получаем данные из БД c последующим выводом в таблицу
        private async Task GetListCategory()
        {
            ListCategory.Clear(); // очищаем коллекцию перед заполнением
            using (FoodDeliveryContext foodDeliveryContext = new FoodDeliveryContext())
            {
                List<Category> categories = await foodDeliveryContext.Categories.ToListAsync();
                categories.Reverse(); // переворачиваем список
                ListCategory = categories.Count > 0 ? new ObservableCollection<Category>(categories) : new ObservableCollection<Category>();
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
                    (_btn_OpenPopupToAddData = new RelayCommand((obj) =>
                    {
                        IsAddData = true; // изменяем режим работы Popup на режим добавления данных
                        AddAndEditDataPopup.IsOpen = true; // отображаем Popup
                        IsCheckAddAndEditOrDelete = true; // режим редактирования или добавления данных (удержания фокуса на Popup)
                        DarkBackground.Visibility = Visibility.Visible; // показать фон
                        WorkingWithData.ExitHamburgerMenu(); // закрываем, если открыто "гамбургер меню"
                        HeadingPopup = "Добавить категорию"; // изменяем заголовок Popup
                        ActionConfirmationButton = "Добавить"; // изменение названия кнопки подтверждения действия 
                        OutNameDescription = ""; OutNameCategory = ""; // убираем введенные значения

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
                    (_btn_OpenPopupToEditData = new RelayCommand((obj) =>
                    {
                        IsAddData = false; // изменяем режим работы Popup на режим редактирования
                        AddAndEditDataPopup.IsOpen = true; // отображаем Popup
                        IsCheckAddAndEditOrDelete = true; // режим редактирования или добавления данных (удержания фокуса на Popup)
                        DarkBackground.Visibility = Visibility.Visible; // показать фон
                        WorkingWithData.ExitHamburgerMenu(); // закрываем, если открыто "гамбургер меню"
                        HeadingPopup = "Изменить категорию"; // изменяем заголовок Popup
                        ActionConfirmationButton = "Изменить"; // изменение названия кнопки подтверждения действия
                        // вносим данные в поля для изменения
                        if (!SelectedCategory.name.IsNullOrEmpty())
                        {
                            OutNameCategory = SelectedCategory.name;
                        }
                        if (!SelectedCategory.description.IsNullOrEmpty())
                        {
                            OutNameDescription = SelectedCategory.description;
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
                        if (!SelectedCategory.name.IsNullOrEmpty())
                        {
                            NameOfCategoryDeleted = "Выбранная категория: " + SelectedCategory.name;
                        }

                        NotificationOfThePopupLaunchJson(); // оповещаем JSON, чтомы запустили Popup

                    }, (obj) => true));
            }
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
            DeleteDataPopup.IsOpen = false;
            DarkBackground.Visibility = Visibility.Collapsed; // скрываем фон
            // убираем введенные значения
            OutNameDescription = ""; OutNameCategory = "";

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
                        if (!string.IsNullOrWhiteSpace(OutNameCategory))
                        {
                            if (IsAddData) // добавление данных
                            {
                                // проверяем наличие дубликата в БД
                                using (FoodDeliveryContext foodDeliveryContext = new FoodDeliveryContext())
                                {
                                    List<Category> categories = await foodDeliveryContext.Categories.ToListAsync();
                                    if (!categories.Any(c => c.name.ToLowerInvariant()
                == OutNameCategory.ToLowerInvariant().Trim()))
                                    {
                                        // добавляем данные в БД
                                        Category category = new Category();
                                        category.name = OutNameCategory.Trim();
                                        if (!string.IsNullOrWhiteSpace(OutNameDescription))
                                        {
                                            category.description = OutNameDescription.Trim();
                                        }
                                        await foodDeliveryContext.Categories.AddAsync(category);
                                        await foodDeliveryContext.SaveChangesAsync(); // cохраняем изменения в базе данных
                                        ClosePopupWorkingWithData(); // закрываем Popup
                                        GetListCategory(); // обновляем список
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
                                    List<Category> categories = await foodDeliveryContext.Categories.ToListAsync();
                                    categories = categories.Where(c => c.id != SelectedCategory.id).ToList(); // исключаем элемент из поиска совпадений,
                                                                                                              // который мы выбрали для редактирования
                                    if (!categories.Any(c => c.name.ToLowerInvariant()
               .Contains(OutNameCategory.ToLowerInvariant().Trim())))
                                    {
                                        // находим объект для изменения в БД
                                        Category categoryToChange = await foodDeliveryContext.Categories.FirstOrDefaultAsync(c => c.id == SelectedCategory.id);
                                        if (categoryToChange != null)
                                        {
                                            // изменяем данные
                                            categoryToChange.name = OutNameCategory.Trim();
                                            if (!string.IsNullOrWhiteSpace(OutNameDescription))
                                            {
                                                categoryToChange.description = OutNameDescription.Trim();
                                            }
                                            else
                                            {
                                                categoryToChange.description = "";
                                            }
                                            await foodDeliveryContext.SaveChangesAsync(); // cохраняем изменения в базе данных
                                            ClosePopupWorkingWithData(); // закрываем Popup
                                            GetListCategory(); // обновляем список
                                        }
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
                        }
                        else
                        {
                            // название категории не заполнено
                            StartFieldIllumination(AnimationOutName); // подсветка поля
                            ErrorInputPopup = "Заполните обязательные поля!"; // сообщение с обибкой
                            BeginFadeAnimation(AnimationErrorInputPopup); // затухание сообщения об ошибке
                        }

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
                            Category category = await foodDeliveryContext.Categories.FirstOrDefaultAsync(c => c.id == SelectedCategory.id);
                            if (category != null)
                            {
                                foodDeliveryContext.Categories.Remove(category);
                                await foodDeliveryContext.SaveChangesAsync(); // cохраняем изменения в базе данных                       
                                ClosePopupWorkingWithData(); // закрываем Popup
                                GetListCategory(); // обновляем список
                            }
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
                DarkBackground.Visibility = Visibility.Visible; // показать фон
                WorkingWithData.ExitHamburgerMenu(); // закрываем, если открыто "гамбургер меню"
            }
            else
            {
                DeleteDataPopup.IsOpen = true; // отображаем Popup
                DarkBackground.Visibility = Visibility.Visible; // показать фон
                WorkingWithData.ExitHamburgerMenu(); // закрываем, если открыто "гамбургер меню"
                                                     // отображаем название категории перед удалением в Popup
                if (!SelectedCategory.name.IsNullOrEmpty())
                {
                    NameOfCategoryDeleted = "Выбранная категория: " + SelectedCategory.name;
                }
            }
        }

        // записываем в JSON, что мы запустили Popup данной страницы
        private void NotificationOfThePopupLaunchJson()
        {
            // передаём в JSON, что мы запустили Popup
            var jsonData = new { popup = "Category" };
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
        public TextBox AnimationOutName { get; set; } // поле для ввода текста "название категории". Вывод подсветки поля
        public TextBox AnimationOutDescription { get; set; } // поле для ввода текста "описание категории". Вывод подсветки поля
        public Storyboard FieldIllumination { get; set; } // анимация объектов
        public Popup DeleteDataPopup { get; set; } // ссылка на Popup для удаления данных

        // ассинхронно получаем информацию из CategoryPage 
        public async Task InitializeAsync(Popup AddAndEditDataPopup, Border DarkBackground, TextBlock AnimationErrorInput,
            TextBlock AnimationErrorInputPopup, TextBox AnimationOutName, TextBox AnimationOutDescription,
            Storyboard FieldIllumination, Popup DeleteDataPopup)
        {
            if (AddAndEditDataPopup != null)
            {
                this.AddAndEditDataPopup = AddAndEditDataPopup;
            }
            if (DarkBackground != null)
            {
                this.DarkBackground = DarkBackground;
            }
            if (AnimationErrorInput != null)
            {
                this.AnimationErrorInput = AnimationErrorInput;
            }
            if (AnimationErrorInputPopup != null)
            {
                this.AnimationErrorInputPopup = AnimationErrorInputPopup;
            }
            if (AnimationOutName != null)
            {
                this.AnimationOutName = AnimationOutName;
            }
            if (AnimationOutDescription != null)
            {
                this.AnimationOutDescription = AnimationOutDescription;
            }
            if (FieldIllumination != null)
            {
                this.FieldIllumination = FieldIllumination;
            }
            if (DeleteDataPopup != null)
            {
                this.DeleteDataPopup = DeleteDataPopup;
            }
        }

        // отображение название категории в Popup для удаления данных
        private string _nameOfCategoryDeleted { get; set; }
        public string NameOfCategoryDeleted
        {
            get { return _nameOfCategoryDeleted; }
            set { _nameOfCategoryDeleted = value; OnPropertyChanged(nameof(NameOfCategoryDeleted)); }
        }

        // название кнопки для подверждения действия при удалении или редактировании
        private string _actionConfirmationButton { get; set; }
        public string ActionConfirmationButton
        {
            get { return _actionConfirmationButton; }
            set { _actionConfirmationButton = value; OnPropertyChanged(nameof(ActionConfirmationButton)); }
        }

        // выбор объекта в таблице
        private Category _selectedCategory { get; set; }
        public Category SelectedCategory
        {
            get { return _selectedCategory; }
            set { _selectedCategory = value; OnPropertyChanged(nameof(SelectedCategory)); OnPropertyChanged(nameof(IsWorkButtonEnable)); }
        }

        // отображение кнопки "удалить" и "редакировать"
        private bool _isWorkButtonEnable { get; set; }
        public bool IsWorkButtonEnable
        {
            get { return SelectedCategory != null; } // если в таблице выбранн объект, то кнопки работают
            set { _isWorkButtonEnable = value; OnPropertyChanged(nameof(IsWorkButtonEnable)); }
        }

        // свойство для вывода текста в поле "название категории"
        private string _outNameCategory { get; set; }
        public string OutNameCategory
        {
            get { return _outNameCategory; }
            set { _outNameCategory = value; OnPropertyChanged(nameof(OutNameCategory)); }
        }

        // свойство для вывода текста в поле "описание категории"
        private string _outNameDescription { get; set; }
        public string OutNameDescription
        {
            get { return _outNameDescription; }
            set { _outNameDescription = value; OnPropertyChanged(nameof(OutNameDescription)); }
        }

        // свойство для вывода ошибки при поиске данных в таблице
        private string _errorInput { get; set; }
        public string ErrorInput
        {
            get { return _errorInput; }
            set { _errorInput = value; OnPropertyChanged(nameof(ErrorInput)); }
        }

        // свойство для вывода ошибки при добавлении или редактировании данных
        private string _errorInputPopup { get; set; }
        public string ErrorInputPopup
        {
            get { return _errorInputPopup; }
            set { _errorInputPopup = value; OnPropertyChanged(nameof(ErrorInputPopup)); }
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
        public ObservableCollection<Category> ListSearch { get; set; } = new ObservableCollection<Category>();

        public async Task HandlerTextBoxChanged(string searchByValue)
        {
            searchByValue = searchByValue.Trim(); // убираем пробелы
            if (!string.IsNullOrWhiteSpace(searchByValue))
            {
                await GetListCategory(); // обновляем список
                ListSearch = ListCategory; // присваиваем список из таблицы
                // создаём список с поиском по введенным данным в таблице
                var searchResult = ListSearch.Where(c => c.name.ToLowerInvariant()
                .Contains(searchByValue.ToLowerInvariant())).ToList();

                ListCategory.Clear(); // очищаем список отображения данных в таблице
                // вносим актуальные данные основного списка с учётом фильтра
                ListCategory = new ObservableCollection<Category>(searchResult);
            }
            else
            {
                ListCategory.Clear(); // очищаем список отображения данных в таблице
                await GetListCategory(); // обновляем список
            }

            if (ListCategory.Count == 0)
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

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
