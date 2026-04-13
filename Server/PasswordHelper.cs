using System;
using System.Security.Cryptography;
using System.Text;

namespace ASCIIAssault_Server
{
    public class PasswordHelper
    {
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int Iterations = 100000;

        public static string HashPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                salt,
                Iterations,
                HashSize);

            return Convert.ToBase64String(salt) + "." + Convert.ToBase64String(hash);
        }

        public static bool VerifyPassword(string password, string correctHash, string correctSalt)
        {
            byte[] saltBytes = Convert.FromBase64String(correctSalt);
            byte[] hashBytes = Convert.FromBase64String(correctHash);

            byte[] computedHash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                saltBytes,
                Iterations,
                HashSize);

            return CryptographicOperations.FixedTimeEquals(computedHash, hashBytes);
        }
    }
}
