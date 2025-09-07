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

        /// <summary>
        /// Verifies a provided password (passport number) against a stored hash.
        /// </summary>
        /// <param name="providedPassword">The plain-text passport number entered by the user.</param>
        /// <param name="storedHash">The hashed passport number retrieved from the database.</param>
        /// <returns>True if the provided password matches the stored hash, otherwise false.</returns>
        public static bool VerifyClientPassword(string providedPassword, byte[] storedHash)
        {
            if (string.IsNullOrEmpty(providedPassword) || storedHash == null || storedHash.Length == 0)
            {
                return false;
            }

            // Hash the provided password
            byte[] providedPasswordHash = HashPassword(providedPassword);

            // Compare the newly created hash with the stored hash
            if (providedPasswordHash.Length != storedHash.Length)
            {
                return false;
            }

            for (int i = 0; i < providedPasswordHash.Length; i++)
            {
                if (providedPasswordHash[i] != storedHash[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}

