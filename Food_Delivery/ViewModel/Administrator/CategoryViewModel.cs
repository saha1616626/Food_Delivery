using Food_Delivery.Data;
using Food_Delivery.Helper;
using Food_Delivery.Model;
using MaterialDesignColors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        public CategoryViewModel()
        {
            GetListCategory(); // вывод данных в таблицу
        }

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
            using(FoodDeliveryContext foodDeliveryContext = new FoodDeliveryContext())
            {
                List<Category> categories = await foodDeliveryContext.Categories.ToListAsync();
                foreach (Category category in categories)
                {
                    ListCategory.Add(category);
                }
            }
        }

        #endregion

        #region Popup

        // запускаем Popup для добавления данных
        private RelayCommand _btn_OpenPopupToAddData { get; set; }
        public RelayCommand Btn_OpenPopupToAddData
        {
            get
            {
                return _btn_OpenPopupToAddData ??
                    (_btn_OpenPopupToAddData = new RelayCommand((obj) =>
                    {
                        // отображаем Popup
                        AddAndEditDataPopup.IsOpen = true;
                        DarkBackground.Visibility = Visibility.Visible; // показать фон
                        WorkingWithData.ExitHamburgerMenu(); // закрываем, если открыто "гамбургер меню"

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
                        AddAndEditDataPopup.IsOpen = false; // Закрыть Popup
                        DarkBackground.Visibility = Visibility.Collapsed; // скрываем фон
                    }, (obj) => true));
            }
        }

        #endregion

        #region Features

        public Border DarkBackground { get; set; } // затемненный фон позади Popup
        public Popup AddAndEditDataPopup { get; set; } // ссылка на Popup
        public TextBlock AnimationErrorInput { get; set; } // текстовое поле для вывода ошибки

        // ассинхронно получаем информацию из CategoryPage 
        public async Task InitializeAsync(Popup AddAndEditDataPopup, Border DarkBackground, TextBlock AnimationErrorInput)
        {
            if(AddAndEditDataPopup != null)
            {
                this.AddAndEditDataPopup = AddAndEditDataPopup;
            }
            if(DarkBackground != null)
            {
                this.DarkBackground = DarkBackground;
            }
            if(AnimationErrorInput != null)
            {
                this.AnimationErrorInput = AnimationErrorInput;
            }
        }

        // свойство для вывода ошибки
        private string _errorInput {  get; set; }
        public string ErrorInput
        {
            get { return _errorInput; }
            set { _errorInput = value; OnPropertyChanged(nameof(ErrorInput)); }
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

            if(ListCategory.Count == 0)
            {
                ErrorInput = "Категория не найдена!"; // собщение об ошибке
                BeginFadeAnimation(AnimationErrorInput); // анимация затухания ошибки
            }
        }

        // анимация затухания ошибки
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

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
