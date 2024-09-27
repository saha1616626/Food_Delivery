using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Food_Delivery.Model
{
    public class AuthorizationEntrance // класс текущей сессии пользователя
    {
        // состояние входа 

        private bool entrance { get; set; } // состояние входа (true - пользователь авторизован)
        public bool Entrance
        {
            get { return entrance; }
            set { entrance = value; }
        }

        private int? userId { get; set; } // id пользователя, который вошел
        public int? UserId
        {
            get { return userId; }
            set { userId = value; }
        }

        private string userRole { get; set; } // роль пользователя
        public string UserRole
        {
            get { return userRole; }
            set { userRole = value; }
        }

        public AuthorizationEntrance() { }
        public AuthorizationEntrance(bool Entrance, int userId, string userRole)
        {
            this.Entrance = Entrance;
            this.UserId = userId;
            this.UserRole = userRole;
        }
    }
}
