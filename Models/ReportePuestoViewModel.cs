using System.Collections.Generic;

namespace MercadoApp.Models
{
    public class ReportePuestoViewModel
    {
        public List<Puesto> Puestos { get; set; } = new List<Puesto>();
        public int Cantidad { get; set; }
    }
}