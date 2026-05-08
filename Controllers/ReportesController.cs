using Microsoft.AspNetCore.Mvc;
using MercadoApp.Models;
using MercadoApp.Repositories.Interfaces;
using System;

namespace MercadoApp.Controllers
{
    public class ReportesController : Controller
    {
        private readonly IPuestoRepository _puestoRepository;
        private readonly IDeudaRepository _deudaRepository;
        private readonly IPagoRepository _pagoRepository;

        public ReportesController(IPuestoRepository puestoRepository, IDeudaRepository deudaRepository, IPagoRepository pagoRepository)
        {
            _puestoRepository = puestoRepository;
            _deudaRepository = deudaRepository;
            _pagoRepository = pagoRepository;
        }

        public IActionResult Index()
        {
            var fechaInicio = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var fechaFin = DateTime.Now;

            ViewBag.FechaInicio = fechaInicio.ToString("yyyy-MM-dd");
            ViewBag.FechaFin = fechaFin.ToString("yyyy-MM-dd");

            return View();
        }

        [HttpPost]
        public IActionResult Deudas(string fechaInicio, string fechaFin)
        {
            var inicio = DateTime.Parse(fechaInicio);
            var fin = DateTime.Parse(fechaFin).AddDays(1).AddSeconds(-1);

            var deudas = _deudaRepository.GetByFecha(inicio, fin);

            var model = new ReporteDeudaViewModel
            {
                Deudas = deudas,
                Cantidad = ((System.Collections.Generic.List<Deuda>)deudas).Count,
                TotalMonto = ((System.Collections.Generic.List<Deuda>)deudas).Sum(d => d.Monto)
            };

            ViewBag.FechaInicio = fechaInicio;
            ViewBag.FechaFin = fechaFin;
            ViewBag.Titulo = "Reporte de Deudas";

            return View("ReporteDeudas", model);
        }

        [HttpPost]
        public IActionResult Pagos(string fechaInicio, string fechaFin)
        {
            var inicio = DateTime.Parse(fechaInicio);
            var fin = DateTime.Parse(fechaFin).AddDays(1).AddSeconds(-1);

            var pagos = _pagoRepository.GetByFecha(inicio, fin);

            var model = new ReportePagoViewModel
            {
                Pagos = pagos,
                Cantidad = ((System.Collections.Generic.List<Pago>)pagos).Count,
                TotalIngresos = ((System.Collections.Generic.List<Pago>)pagos).Sum(p => p.MontoPagado)
            };

            ViewBag.FechaInicio = fechaInicio;
            ViewBag.FechaFin = fechaFin;
            ViewBag.Titulo = "Reporte de Ingresos/Pagos";

            return View("ReportePagos", model);
        }

        [HttpPost]
        public IActionResult Puestos(string estado)
        {
            var puestos = _puestoRepository.GetAll();

            if (!string.IsNullOrEmpty(estado) && estado != "Todos")
            {
                puestos = ((System.Collections.Generic.List<Puesto>)puestos).FindAll(p => p.Estado == estado);
            }

            var model = new ReportePuestoViewModel
            {
                Puestos = puestos,
                Cantidad = ((System.Collections.Generic.List<Puesto>)puestos).Count
            };

            ViewBag.Estado = estado;
            ViewBag.Titulo = "Reporte de Puestos";

            return View("ReportePuestos", model);
        }
    }
}