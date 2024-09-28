using Food_Delivery.Data;
using Food_Delivery.Helper;
using Food_Delivery.Model;
using Food_Delivery.View.Administrator.Menu;
using Food_Delivery.View.Authorization;
using Food_Delivery.View.Client.MainPages;
using Food_Delivery.ViewModel;
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

        AuthorizationViewModel authorizationViewModel { get; set; }
        AuthorizationPage authorizationPage { get; set; }
        MainMenuPage mainMenuPage { get; set; }
        MainMenuServicePage mainMenuServicePage { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            // автоматизированный вход в систему
            LogInSystem();

            // при получении фокуса приложения проверяем закрытые Popup
            this.Activated += checkingClosedWindows;

            // подписываемся на событие - успешная авторизация в аккаунт
            WorkingWithData.SuccessfulLogin += EntranceAccount;
            // подписываемся на событие - выход из аккаунта
            WorkingWithData.ExitFromAccount += OutAccount;
        }

        // работа с авторизацией
        #region Authorization

        // автоматизированный вход в систему
        private async Task LogInSystem()
        {
            // очистка памяти
            ClearMemoryAfterFrame(mainMenuPage);
            ClearMemoryAfterFrame(authorizationPage);
            ClearMemoryAfterFrame(mainMenuServicePage);

            // проверка, если пользователь не вошел в аккаунт, то мы
            // его направляем на страницу авторизации
            authorizationViewModel = new AuthorizationViewModel();
            if (authorizationViewModel.IsCheckAccountUser())
            {
                // пользователь авторизовался, проверяем роль и входим на нужную страницу
                string role = await authorizationViewModel.WeGetRoleUser();
                if (role != null)
                {
                    if (role == "Администратор" || role == "Менеджер")
                    {
                        await Task.Run(async () =>
                        {
                            await Task.Delay(1000); // Ждем завершения загрузки
                            System.Windows.Application.Current.Dispatcher.Invoke(() =>
                            {
                                mainFrame.Navigate(mainMenuPage = new MainMenuPage());
                            });
                        });

                    }
                    else if (role == "Гость" || role == "Пользователь") // авторизация как гостя
                    {
                        await Task.Run(async () =>
                        {
                            await Task.Delay(1000); // Ждем завершения загрузки
                            System.Windows.Application.Current.Dispatcher.Invoke(() =>
                            {
                                mainFrame.Navigate(mainMenuServicePage = new MainMenuServicePage());
                            });
                        });
                    }
                }
            }
            else
            {
                // запус страницы авторизации
                await Task.Run(async () =>
                {
                    await Task.Delay(1000); // Ждем завершения загрузки
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        mainFrame.Navigate(authorizationPage = new AuthorizationPage());
                    });
                });
            }
        }

        // успешная авторизация в аккаунт
        private async void EntranceAccount(object sender, EventAggregator e)
        {
            // очистка памяти
            ClearMemoryAfterFrame(mainMenuPage);
            ClearMemoryAfterFrame(authorizationPage);
            ClearMemoryAfterFrame(mainMenuServicePage);

            // проверка, если пользователь не вошел в аккаунт, то мы
            // его направляем на страницу авторизации
            authorizationViewModel = new AuthorizationViewModel();
            if (authorizationViewModel.IsCheckAccountUser())
            {
                // пользователь авторизовался, проверяем роль и входим на нужную страницу
                string role = await authorizationViewModel.WeGetRoleUser();
                if (role != null)
                {
                    if (role == "Администратор" || role == "Менеджер")
                    {
                        await Task.Run(async () =>
                        {
                            await Task.Delay(1000); // Ждем завершения загрузки
                            System.Windows.Application.Current.Dispatcher.Invoke(() =>
                            {
                                mainFrame.Navigate(mainMenuPage = new MainMenuPage());
                            });
                        });

                    }
                    else if (role == "Гость" || role == "Пользователь") // авторизация как гостя
                    {
                        await Task.Run(async () =>
                        {
                            await Task.Delay(1000); // Ждем завершения загрузки
                            System.Windows.Application.Current.Dispatcher.Invoke(() =>
                            {
                                mainFrame.Navigate(mainMenuServicePage = new MainMenuServicePage());
                            });
                        });
                    }
                }
            }
            else
            {
                // запус страницы авторизации
                await Task.Run(async () =>
                {
                    await Task.Delay(1000); // Ждем завершения загрузки
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        mainFrame.Navigate(authorizationPage = new AuthorizationPage());
                    });
                });
            }
        }

        // выход из аккаунта
        private void OutAccount(object sender, EventAggregator e)
        {
            // очистка памяти
            ClearMemoryAfterFrame(mainMenuPage);
            ClearMemoryAfterFrame(authorizationPage);
            ClearMemoryAfterFrame(mainMenuServicePage);

            // Запускаем старницу авторизации
            authorizationPage = new AuthorizationPage();
            mainFrame.Navigate(authorizationPage);
        }

        // очистка памяти
        public void ClearMemoryAfterFrame(FrameworkElement element)
        {
            if (element != null)
            {
                // очистка всех привязанных элементов
                BindingOperations.ClearAllBindings(element);
                // очистка визуальных элементов
                element.Resources.Clear();
                // Очистка ссылки на предыдущий экземпляр
                element = null;
            }

            if (mainFrame != null)
            {
                // очистка фрейма
                mainFrame.Content = null;
            }
        }

        #endregion

        #region IsCheckingClosed

        // проверяем закрытие Popup при смене фокуса на приложении
        private void checkingClosedWindows(object sender, EventArgs e)
        {
            // чтение JSON
            string JSON = File.ReadAllText(pathDataPopup);
            // получаем данные из JSON
            dynamic data = JsonConvert.DeserializeObject(JSON);
            if (data != null)
            {
                // категории
                if (data.popup == "Category")
                {
                    WorkingWithData.LaunchPopupAfterReceivingFocusCategory(); // событие запуска Popup
                }

                // блюда
                if (data.popup == "Dishes")
                {
                    WorkingWithData.LaunchPopupAfterReceivingFocusDish(); // событие запуска Popup
                }

                // заказы
                if (data.popup == "Orders")
                {
                    WorkingWithData.LaunchPopupAfterReceivingFocusOrders(); // событие запуска Popup
                }

                // пользователи
                if (data.popup == "Users")
                {
                    WorkingWithData.LaunchPopupAfterReceivingFocusUsers(); // событие запуска Popup
                }

                // меню администратора
                if (data.popup == "MainMenu")
                {
                    WorkingWithData.LaunchPopupAfterReceivingFocusMainMenu(); // событие запуска Popup
                }
            }
        }

        #endregion

    }
}