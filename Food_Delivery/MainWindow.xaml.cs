using Food_Delivery.View.Administrator.Menu;
using Food_Delivery.View.Authorization;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Food_Delivery
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public  MainWindow()
        {
            InitializeComponent();

            // запуск страницы авторизации
            LaunchAuthorizationPageAsync();
        }



        private async void LaunchAuthorizationPageAsync()
        {
            try
            {
                AuthorizationPage authorizationPage = new AuthorizationPage();
                MainMenuPage mainMenuPage = new MainMenuPage();
                if (this.Dispatcher.CheckAccess())
                {
                    await Task.Run(async () =>
                    {
                        await Task.Delay(500); // Ждем завершения загрузки
                        System.Windows.Application.Current.Dispatcher.Invoke(() =>
                        {
                            mainFrame.Navigate(mainMenuPage);
                        });
                    });
                    
                }
                else
                {
                    this.Dispatcher.Invoke(() => mainFrame.Navigate(authorizationPage));
                }
            }
            catch (Exception ex)
            {
                if (this.Dispatcher.CheckAccess())
                {
                    MessageBox.Show($"Ошибка при загрузке страницы авторизации: {ex.Message}");
                }
                else
                {
                    this.Dispatcher.Invoke(() => MessageBox.Show($"Ошибка при загрузке страницы авторизации: {ex.Message}"));
                }
            }
        }



        private async void AuthorizationPage_Loaded(object sender, RoutedEventArgs e )
        {
            AuthorizationPage authorizationPage = new AuthorizationPage();
            mainFrame.Navigate(authorizationPage);
            //MessageBox.Show("111111");
        }
    }
}