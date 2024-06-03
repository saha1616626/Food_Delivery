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

        public string name { get; set; }

        public string surname { get; set; }

        public string patronymic { get; set; }

        public string registrationDate { get; set; } = null!;

        public string email { get; set; }

        public string numberPhone { get; set; }

        public string login { get; set; } = null!;

        public string password { get; set; } = null!;

        public string city { get; set; }

        public string street { get; set; }

        public string house { get; set; }

        public string apartment { get; set; }

        // устанваливаем внешний ключ на таблицу Role
        public virtual Role Role { get; set; } = null!;

    }
}
