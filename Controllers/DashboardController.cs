using Microsoft.AspNetCore.Mvc;
using MercadoApp.Models;
using MercadoApp.Repositories.Interfaces;
using System.Threading.Tasks;

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

        public async Task<IActionResult> Index()
        {
            var puestosTask = _puestoRepository.GetAllAsync();
            var puestosLibresTask = _puestoRepository.GetCountByEstadoAsync("Libre");
            var puestosAlquiladosTask = _puestoRepository.GetCountByEstadoAsync("Alquilado");
            var puestosVendidosTask = _puestoRepository.GetCountByEstadoAsync("Vendido");
            
            var deudasTask = _deudaRepository.GetAllAsync();
            var deudasPendientesTask = _deudaRepository.GetCountByEstadoAsync(false);
            var deudasPagadasTask = _deudaRepository.GetCountByEstadoAsync(true);
            var totalMontoPendienteTask = _deudaRepository.GetTotalMontoAsync(false);
            var totalMontoPagadoTask = _deudaRepository.GetTotalMontoAsync(true);
            
            var pagosCountTask = _pagoRepository.GetCountAsync();
            var totalIngresosTask = _pagoRepository.GetTotalIngresosAsync();
            
            var puestosRecientesTask = _puestoRepository.GetRecentAsync(5);
            var deudasRecientesTask = _deudaRepository.GetRecentAsync(5);
            var pagosRecientesTask = _pagoRepository.GetRecentAsync(5);

            await Task.WhenAll(
                puestosTask, puestosLibresTask, puestosAlquiladosTask, puestosVendidosTask,
                deudasTask, deudasPendientesTask, deudasPagadasTask, totalMontoPendienteTask, totalMontoPagadoTask,
                pagosCountTask, totalIngresosTask,
                puestosRecientesTask, deudasRecientesTask, pagosRecientesTask
            );

            var model = new DashboardViewModel
            {
                TotalPuestos = (await puestosTask).Count(),
                PuestosLibres = await puestosLibresTask,
                PuestosAlquilados = await puestosAlquiladosTask,
                PuestosVendidos = await puestosVendidosTask,

                TotalDeudas = (await deudasTask).Count(),
                DeudasPendientes = await deudasPendientesTask,
                DeudasPagadas = await deudasPagadasTask,
                TotalMontoDeudas = await totalMontoPendienteTask + await totalMontoPagadoTask,
                TotalMontoPendiente = await totalMontoPendienteTask,

                TotalPagos = await pagosCountTask,
                TotalIngresos = await totalIngresosTask,

                PuestosRecientes = await puestosRecientesTask,
                DeudasRecientes = await deudasRecientesTask,
                PagosRecientes = await pagosRecientesTask
            };

            return View(model);
        }
    }
}