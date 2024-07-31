using Food_Delivery.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Food_Delivery.Model.DPO
{
    public class AccountDPO : INotifyPropertyChanged
    {
        public int id {  get; set; }
        private int _roleId { get; set; }
        public int roleId
        {
            get { return _roleId; }
            set { _roleId = value; OnPropertyChanged(nameof(roleId)); }
        }
        private string _nameRole { get; set; }
        public string nameRole
        {
            get { return _nameRole; }
            set { _nameRole = value; OnPropertyChanged(nameof(nameRole)); }
        }

        private string _name {  get; set; }
        public string name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged(nameof(name)); }
        }

        private string _surname {  get; set; }
        public string surname
        {
            get { return _surname; }
            set { _surname = value; OnPropertyChanged(nameof(surname)); }
        }

        private string _patronymic { get; set; }
        public string patronymic
        {
            get { return _patronymic; }
            set { _patronymic = value; OnPropertyChanged(nameof(patronymic)); }
        }

        private DateTime? _registrationDate { get; set; }
        public DateTime? registrationDate
        {
            get { return _registrationDate; }
            set { _registrationDate = value; OnPropertyChanged(nameof(registrationDate)); }
        }

        private string _email { get; set; }
        public string email
        {
            get { return _email; }
            set { _email = value; OnPropertyChanged(nameof(email)); }
        }

        private string _numberphone { get; set; }
        public string numberphone
        {
            get { return _numberphone; }
            set { _numberphone = value; OnPropertyChanged(nameof(numberphone)); }
        }

        private string _login { get; set; }
        public string login
        {
            get { return _login; }
            set { _login = value; OnPropertyChanged(nameof(login)); }
        }

        private string _password { get; set; }
        public string password
        {
            get { return _password; }
            set { _password = value; OnPropertyChanged(nameof(password)); }
        }

        private string _city { get; set; }
        public string city
        {
            get { return _city; }
            set { _city = value; OnPropertyChanged(nameof(city)); }
        }

        private string _street { get; set; }
        public string street
        {
            get { return _street; }
            set { _street = value; OnPropertyChanged(nameof(street)); }
        }

        private string _house { get; set; }
        public string house
        {
            get { return _house; }
            set { _house = value; OnPropertyChanged(nameof(house)); }
        }

        private string _apartament { get; set; }
        public string apartament
        {
            get { return _apartament; }
            set { _apartament = value; OnPropertyChanged(nameof(apartament)); }
        }

        // получаем пользователя из Account с заменой id
        public async Task<AccountDPO> CopyFromAccount(Account account)
        {
            AccountDPO accountDPO = new AccountDPO();

            accountDPO.id = account.id; 
            accountDPO.roleId = account.roleId;

            // поиск роли
            using(FoodDeliveryContext foodDeliveryContext = new FoodDeliveryContext())
            {
                List<Role> roles = await foodDeliveryContext.Roles.ToListAsync();
                // ищем роль присущую данному аккаунту
                Role role = roles.FirstOrDefault(r => r.id == account.id);
                if (role != null)
                {
                    accountDPO.nameRole = role.name;
                }
            }

            if (account.name != null)
            {
                accountDPO.name = account.name;
            }
            
            if(account.surname != null)
            {
                accountDPO.surname = account.surname;
            }

            if(account.patronymic != null)
            {
                accountDPO.patronymic = account.patronymic;
            }

            if(account.registrationDate != null)
            {
                accountDPO.registrationDate = account.registrationDate;
            }

            if(account.email != null)
            {
                accountDPO.email = account.email;
            }

            if(account.numberPhone != null)
            {
                accountDPO.numberphone = account.numberPhone;
            }

            if(account.login != null)
            {
                accountDPO.login = account.login;
            }

            if(account.password != null)
            {
                accountDPO.password = account.password;
            }

            if(account.city != null)
            {
                accountDPO.city = account.city;
            }

            if(account.street != null)
            {
                accountDPO.street = account.street;
            }

            if(account.house != null)
            {
                accountDPO.house = account.house;
            }

            if(account.apartment != null)
            {
                accountDPO.apartament = account.apartment;
            }

            return accountDPO;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
