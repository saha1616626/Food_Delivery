using Food_Delivery.ViewModel.Client;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Food_Delivery.View.Client.UserAccount
{
    /// <summary>
    /// Interaction logic for ClientPersonalDataPage.xaml
    /// </summary>
    public partial class ClientPersonalDataPage : Page
    {
        private readonly ClientPersonalDataViewModel _clientPersonalDataViewModel; // объект класса
        public ClientPersonalDataPage()
        {
            InitializeComponent();
            _clientPersonalDataViewModel = (ClientPersonalDataViewModel)this.Resources["ClientPersonalDataViewModel"];
            _clientPersonalDataViewModel.InitializeAsync((Storyboard)FindResource("FieldIllumination"), ClientName,
                ClientSurname, ClientPatronymic, ClientCity, ClientStreet, ClientHouse, ClientApartment, ClientNumberPhone, ClientEmail,
               ErrorInput);
        }
    }
}
