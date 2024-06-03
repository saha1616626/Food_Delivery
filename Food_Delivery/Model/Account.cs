using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Food_Delivery.Model
{
    // Аккаунт пользователя приожения
    public class Account
    {
        public int id { get; set; }

        private string _roleId { get; set; }
        public string roleId
        {
            get { return _roleId; }
            set { _roleId = value; }
        }

        private string _name { get; set; }
        public string name
        {
            get { return _name; }
            set { _name = value; }
        }

        private string _surname { get; set; }
        public string surname
        {
            get { return _surname; }
            set { _surname = value; }
        }

        private string _patronymic { get; set; }
        public string patronymic
        {
            get { return _patronymic; }
            set { _patronymic = value; }
        }

        private string _registrationDate { get; set; }
        public string registrationDate
        {
            get { return _registrationDate; }
            set { _registrationDate = value; }
        }

        private string _email { get; set; }
        public string email
        {
            get { return _email; }
            set { _email = value; }
        }

        private string _numberPhone { get; set; }
        public string numberPhone
        {
            get { return _numberPhone; }
            set { _numberPhone = value; }
        }

        private string _login { get; set; }
        public string login
        {
            get { return _login; }
            set { _login = value; }
        }

        private string _password { get; set; }
        public string password
        {
            get { return _password; }
            set { _password = value; }
        }

        private string _city { get; set; }
        public string city
        {
            get { return _city; }
            set
            {
                _city = value;
            }
        }

        private string _street { get; set; }
        public string street
        {
            get { return _street; }
            set
            {
                _street = value;
            }
        }

        private string _house { get; set; }
        public string house
        {
            get { return _house; }
            set
            {
                _house = value;
            }
        }

        private string _apartment { get; set; }
        public string apartment
        {
            get { return _apartment; }
            set
            {
                _apartment = value;
            }
        }

    }
}
