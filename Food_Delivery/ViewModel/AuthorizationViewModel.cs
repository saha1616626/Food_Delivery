using Food_Delivery.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;


namespace Food_Delivery.ViewModel
{
    public class AuthorizationViewModel
    {
        // путь к json файлу авторизации
        readonly string path = "";
        public string Error { get; set; } // формирование сообщения при генерации ошибки

        public AuthorizationViewModel() { }

        // метод входа в аккаунт
        private RelayCommand _entrance {  get; set; }
        public RelayCommand Entrance
        {
            get
            {
                return _entrance ??
                    (_entrance = new RelayCommand((obj) =>
                    {
                        MessageBox.Show("!!!!!!!!!!!!!!!!!");
                    }, (obj) => true));
            }
        }

        // метод выхода в аккаунт
        public void LogOutYourAccount()
        {

        }

        // свойства 
        #region Features

        // получаем информацию об ошибках и состояниях входа
        public async Task InitializeAsync()
        {

        }

        // выводим сообщения об ошибке с анимацией затухания
        private async void BeginFadeAnimation(TextBlock textBlock)
        {
            textBlock.IsEnabled = true;
            textBlock.Opacity = 1.0;

            Storyboard storyboard = new Storyboard();
            DoubleAnimation fadeAnimation = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                Duration = TimeSpan.FromSeconds(2),
            };
            Storyboard.SetTargetProperty(fadeAnimation, new System.Windows.PropertyPath(TextBlock.OpacityProperty));
            storyboard.Children.Add(fadeAnimation);
            storyboard.Completed += (s, e) => textBlock.IsEnabled = false;
            storyboard.Begin(textBlock);
        }

        // запускаем анимации для TextBox (подсвечивание объекта)
        private void StartAnimation(TextBox textBox)
        {
            
        }

        // запуск анимации для PasswordBox (подсвечивание объекта)
        private void StartAnimation(PasswordBox passwordBox)
        {

        }

        #endregion
    }
}
