using Food_Delivery.Data;
using Food_Delivery.Helper;
using Food_Delivery.Model;
using Food_Delivery.View.Administrator.Menu;
using Food_Delivery.View.Authorization;
using Microsoft.EntityFrameworkCore;
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



        private async Task LaunchAuthorizationPageAsync()
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
                using (FoodDeliveryContext foodDeliveryContext = new FoodDeliveryContext())
                {
                    List<Category> categories = await foodDeliveryContext.Categories.ToListAsync();
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
    }
}