using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PathFinders.Services
{
    public static class PasswordHasher
    {
        public static byte[] HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return new byte[0];
            }

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return bytes;
            }
        }
    }
}
