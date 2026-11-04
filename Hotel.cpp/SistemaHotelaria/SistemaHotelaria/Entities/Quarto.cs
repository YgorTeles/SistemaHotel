using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaHotelaria.Entities
{
    public class Quarto
    {
        public Guid Id { get; set; }
        public int Numero { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public int Capacidade { get; set; }
        public decimal ValorDiaria { get; set; }
    }

}
