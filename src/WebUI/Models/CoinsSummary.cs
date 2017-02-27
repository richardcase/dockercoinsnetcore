using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace WebUI.Models
{
    public class CoinsSummary
    {
        [JsonProperty("coins")]
        public string Coins { get; set; }

        [JsonProperty("hashes")]
        public int Hashes { get; set; }

        [JsonProperty("now")]
        public long Now { get; set; }
    }
}
