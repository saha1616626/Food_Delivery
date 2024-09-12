using Food_Delivery.Data;
using Food_Delivery.Helper;
using Food_Delivery.Model;
using Food_Delivery.View.Administrator.Menu;
using Food_Delivery.View.Authorization;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace Food_Delivery
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // путь к json работа окна Popup
        readonly string pathDataPopup = @"E:\3comm\Documents\Предметы\Курс 3.2\Курсовая\Приложение\Программа\Food_Delivery\Food_Delivery\Data\СheckPopup.json";

        public MainWindow()
        {
            InitializeComponent();

            // запуск страницы авторизации
            LaunchAuthorizationPageAsync();

            // при получении фокуса приложения проверяем закрытые Popup
            this.Activated += checkingClosedWindows;
        }

        #region IsCheckingClosed

        // проверяем закрытие Popup при смене фокуса на приложении
        private void checkingClosedWindows(object sender, EventArgs e)
        {
            // чтение JSON
            string JSON = File.ReadAllText(pathDataPopup);
            // получаем данные из JSON
            dynamic data = JsonConvert.DeserializeObject(JSON);
            if(data != null)
            {
                // категории
                if(data.popup == "Category")
                {
                    WorkingWithData.LaunchPopupAfterReceivingFocusCategory(); // событие запуска Popup
                }

                // блюда
                if(data.popup == "Dishes")
                {
                    WorkingWithData.LaunchPopupAfterReceivingFocusDish(); // событие запуска Popup
                }

                // заказы
                if(data.popup == "Orders")
                {
                    WorkingWithData.LaunchPopupAfterReceivingFocusOrders(); // событие запуска Popup
                }
            }
        }

        #endregion

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