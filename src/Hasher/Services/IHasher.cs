using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hasher.Services
{
    public interface IHasher
    {
        string HashString(string toHash);
    }
}
