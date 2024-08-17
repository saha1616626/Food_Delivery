using Food_Delivery.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Food_Delivery.View.Authorization
{
    /// <summary>
    /// Interaction logic for AuthorizationPage.xaml
    /// </summary>
    public partial class AuthorizationPage : Page
    {
        private readonly AuthorizationViewModel _authorizationViewModel; // связанная модель
        public AuthorizationPage()
        {
            InitializeComponent();
            _authorizationViewModel = (AuthorizationViewModel)this.Resources["AuthorizationViewModel"]; // получаем ссылку на экземпляр привязанной модели

        }

    }
}
