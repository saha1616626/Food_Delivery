using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Food_Delivery.Helper
{
    public class PasswordHasher
    {
        // метод хэширования
        public static string HashPassword(string password)
        {
            // генерация соли
            byte[] salt = GenerateSalt();

            // вычисление хэш пароля с ипользованием соли
            byte[] hash = ComputeHash(password, salt);

            // объединение соли и хэша в одну строку для хранения
            return Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(hash);
        }

        // проверка пароля введенного пользователем
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            // разделение строки на соль и кэш
            string[] parts = hashedPassword.Split(':');
            byte[] salt = Convert.FromBase64String(parts[0]);
            byte[] expectedHash = Convert.FromBase64String(parts[1]);

            // вычисляем хэш введенного пароля с использованием сохраненной соли
            byte[] hash = ComputeHash(password, salt);

            // сравнение хэша пароля пользователя и сохраненного
            return ByteArraysEqual(hash, expectedHash);
        }

        // генерируем соль
        private static byte[] GenerateSalt()
        {
            // генерация случайных байтов для соли
            byte[] salt = new byte[32];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        // вычисление хэша пароля с использованием соли
        private static byte[] ComputeHash(string password, byte[] salt)
        {
            // HMACSHA512 для вычисления хэша с солью
            using (var hmac = new HMACSHA512(salt))
            {
                return hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        // сравнение хэша пароля пользователя и сохраненного
        public static bool ByteArraysEqual(byte[] a, byte[] b)
        {
            // сравнение двух байтовых массива в постоянном времени, чтобы предотвратить утечку информации о различиях
            if (a.Length != b.Length)
            {
                return false;
            }

            int diff = 0;
            for (int i = 0; i < a.Length; i++)
            {
                diff |= a[i] ^ b[i];
            }
            return diff == 0;
        }

    }
}
