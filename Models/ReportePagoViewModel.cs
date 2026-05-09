using System.Collections.Generic;

namespace MercadoApp.Models
{
    public class ReportePagoViewModel
    {
        public List<Pago> Pagos { get; set; } = new List<Pago>();
        public int Cantidad { get; set; }
        public decimal TotalIngresos { get; set; }
    }
}