using System.Collections.Generic;
using MercadoApp.Models;

namespace MercadoApp.Models
{
    public class DashboardViewModel
    {
        public int TotalPuestos { get; set; }
        public int PuestosLibres { get; set; }
        public int PuestosAlquilados { get; set; }
        public int PuestosVendidos { get; set; }

        public int TotalDeudas { get; set; }
        public int DeudasPendientes { get; set; }
        public int DeudasPagadas { get; set; }
        public decimal TotalMontoDeudas { get; set; }
        public decimal TotalMontoPendiente { get; set; }

        public int TotalPagos { get; set; }
        public decimal TotalIngresos { get; set; }

        public IEnumerable<Puesto> PuestosRecientes { get; set; } = new List<Puesto>();
        public IEnumerable<Deuda> DeudasRecientes { get; set; } = new List<Deuda>();
        public IEnumerable<Pago> PagosRecientes { get; set; } = new List<Pago>();
    }
}