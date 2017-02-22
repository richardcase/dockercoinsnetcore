using System;

namespace Hasher.Services
{
    public class MD5Hasher : IHasher
    {
        public string HashString(string toHash)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(toHash);
            var hashAlgoritm = System.Security.Cryptography.MD5.Create();
            bytes = hashAlgoritm.ComputeHash(bytes);

            return Convert.ToBase64String(bytes);
        }
    }
}
