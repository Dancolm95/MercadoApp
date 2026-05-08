using Microsoft.AspNetCore.Mvc;
using MercadoApp.Models;
using MercadoApp.Repositories.Interfaces;

namespace MercadoApp.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IPuestoRepository _puestoRepository;
        private readonly IDeudaRepository _deudaRepository;
        private readonly IPagoRepository _pagoRepository;

        public DashboardController(IPuestoRepository puestoRepository, IDeudaRepository deudaRepository, IPagoRepository pagoRepository)
        {
            _puestoRepository = puestoRepository;
            _deudaRepository = deudaRepository;
            _pagoRepository = pagoRepository;
        }

        public IActionResult Index()
        {
            var model = new DashboardViewModel
            {
                TotalPuestos = _puestoRepository.GetAll().Count(),
                PuestosLibres = _puestoRepository.GetCountByEstado("Libre"),
                PuestosAlquilados = _puestoRepository.GetCountByEstado("Alquilado"),
                PuestosVendidos = _puestoRepository.GetCountByEstado("Vendido"),

                TotalDeudas = _deudaRepository.GetAll().Count(),
                DeudasPendientes = _deudaRepository.GetCountByEstado(false),
                DeudasPagadas = _deudaRepository.GetCountByEstado(true),
                TotalMontoDeudas = _deudaRepository.GetTotalMonto(false) + _deudaRepository.GetTotalMonto(true),
                TotalMontoPendiente = _deudaRepository.GetTotalMonto(false),

                TotalPagos = _pagoRepository.GetCount(),
                TotalIngresos = _pagoRepository.GetTotalIngresos(),

                PuestosRecientes = _puestoRepository.GetRecent(5),
                DeudasRecientes = _deudaRepository.GetRecent(5),
                PagosRecientes = _pagoRepository.GetRecent(5)
            };

            return View(model);
        }
    }
}