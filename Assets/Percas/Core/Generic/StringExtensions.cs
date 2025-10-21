using System;

namespace Percas
{
    public static class StringExtensions
    {
        /// <summary>
        /// A very simple (and not very secure) "encryption" using Base64 encoding.
        /// </summary>
        public static string Encrypt(this string input)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Decrypts the Base64-encoded string.
        /// </summary>
        public static string Decrypt(this string input)
        {
            byte[] bytes = Convert.FromBase64String(input);
            return System.Text.Encoding.UTF8.GetString(bytes);
        }
    }
}
