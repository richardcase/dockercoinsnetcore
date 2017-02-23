using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WebUI.Models
{
    public class CoinsSummary
    {
        public string Coins { get; set; }

        public int Hashes { get; set; }

        public long Now { get; set; }
    }
}
