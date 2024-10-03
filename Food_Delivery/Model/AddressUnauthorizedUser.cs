using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Food_Delivery.Model
{
    public class AddressUnauthorizedUser // адрес гостевого пользователя
    {
        public string city { get; set; }
        public string street { get; set; }
        public string house { get; set; }
        public string apartment { get; set; }

        public AddressUnauthorizedUser() { }

        public AddressUnauthorizedUser(string city, string street, string house, string apartment)
        {
            this.city = city;
            this.street = street;
            this.house = house;
            this.apartment = apartment;
        }
    }
}
