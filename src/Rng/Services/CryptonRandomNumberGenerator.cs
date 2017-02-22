using System;
using System.Security.Cryptography;

namespace Rng.Services
{
    public class CryptonRandomNumberGenerator : IRandonNumberGenerator
    {
        private readonly RandomNumberGenerator generator;

        public CryptonRandomNumberGenerator()
        {
            generator = RandomNumberGenerator.Create();
        }

        public string Generate(int length)
        {
            var buffer = new byte[length];
            generator.GetBytes(buffer);

            long generatedNumber = BitConverter.ToInt64(buffer, 0);

            return generatedNumber.ToString();
        }
    }
}
