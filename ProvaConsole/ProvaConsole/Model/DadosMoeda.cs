using System;
using System.Collections.Generic;
using System.Text;

namespace ProvaConsole.Model
{
    public class DadosMoeda
    {
        public string IdMoeda { get; set; }
        public DateTime DataReferencia { get; set; }
        public int? CodigoCotacao { get; set; }
        public decimal? ValorCotacao { get; set; }
    }
}
