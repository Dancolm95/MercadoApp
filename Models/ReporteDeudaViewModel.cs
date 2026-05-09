using System.Collections.Generic;

namespace MercadoApp.Models
{
    public class ReporteDeudaViewModel
    {
        public List<Deuda> Deudas { get; set; } = new List<Deuda>();
        public int Cantidad { get; set; }
        public decimal TotalMonto { get; set; }
    }
}