using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProvaConsole.Model
{
    public class Item
    {
        [JsonProperty("moeda")]
        public string Moeda { get; set; }

        [JsonProperty("data_inicio")]
        public DateTime DataInicio { get; set; }

        [JsonProperty("data_fim")]
        public DateTime DataFim { get; set; }
    }
}
