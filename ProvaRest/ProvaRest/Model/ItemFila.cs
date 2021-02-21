using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProvaRest.Model
{
    public class ItemFila
    {
        [JsonProperty("moeda")]
        public string Moeda { get; set; }

        [JsonProperty("data_inicio")]
        public DateTime DataInicio { get; set; }

        [JsonProperty("data_fim")]
        public DateTime DataFim { get; set; }
    }
}
