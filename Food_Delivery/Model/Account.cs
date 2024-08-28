using Food_Delivery.Model.DPO;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Food_Delivery.Model
{
    // Аккаунт пользователя приожения
    public class Account
    {
        public int id { get; set; }
        public int roleId { get; set; }
        public string? name { get; set; }
        public string? surname { get; set; }
        public string? patronymic { get; set; }
        public DateTime? registrationDate { get; set; }
        public string? email { get; set; }
        public string? numberPhone { get; set; }
        public string login { get; set; }
        public string password { get; set; }
        public string? city { get; set; }
        public string? street { get; set; }
        public string? house { get; set; }
        public string? apartment { get; set; }

        // устанваливаем внешний ключ на таблицу Role
        public virtual Role Role { get; set; } = null!;

        // связываем ShoppingCart и Account (установка внешнего ключа)
        public virtual ICollection<ShoppingCart> ShoppingCarts { get; set; } = new List<ShoppingCart>();

        // связываем Order и Account (установка внешнего ключа)
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

        public Account() { }

        public Account(int id, int roleId, string name, string surname, string patronymic, DateTime registrationDate, string email, string numberPhone, string login, string password, string city, string street, string house, string apartment, Role role)
        {
            this.id = id;
            this.roleId = roleId;
            this.name = name;
            this.surname = surname;
            this.patronymic = patronymic;
            this.registrationDate = registrationDate;
            this.email = email;
            this.numberPhone = numberPhone;
            this.login = login;
            this.password = password;
            this.city = city;
            this.street = street;
            this.house = house;
            this.apartment = apartment;
            Role = role;
        }

        // получаем аккаунт из AccountDPO
        public async Task<Account> CopyFromAccountDPO(AccountDPO accountDPO)
        {
            Account account = new Account();

            account.id = accountDPO.id;
            account.roleId = accountDPO.roleId;
            if (accountDPO.name != null)
            {
                account.name = accountDPO.name;
            }
            if(accountDPO.surname != null)
            {
                account.surname = accountDPO.surname;
            }
            if(accountDPO.patronymic != null)
            {
                account.patronymic = accountDPO.patronymic;
            }
            if(accountDPO.registrationDate != null)
            {
                account.registrationDate = accountDPO.registrationDate;
            }
            if(accountDPO.email != null)
            {
                account.email = accountDPO.email;
            }
            if(accountDPO.numberphone  != null)
            {
                account.numberPhone = accountDPO.numberphone;
            }
            if(accountDPO.login != null)
            {
                account.login = accountDPO.login;
            }
            if(account.password != null)
            {
                account.password = accountDPO.password;
            }
            if(account.city != null)
            {
                account.city = accountDPO.city;
            }
            if(account.street != null)
            {
                account.street = accountDPO.street;
            }
            if(account.house != null)
            {
                account.house = accountDPO.house;
            }
            if(account.apartment != null)
            {
                account.apartment = accountDPO.apartament;
            }

            return account;
        }

    }
}
