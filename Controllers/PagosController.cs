using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MercadoApp.Models;
using MercadoApp.Repositories.Interfaces;
using System;
using System.Linq;

namespace MercadoApp.Controllers
{
    public class PagosController : Controller
    {
        private readonly IPagoRepository _pagoRepository;
        private readonly IDeudaRepository _deudaRepository;
        private readonly IPersonaRepository _personaRepository;

        public PagosController(IPagoRepository pagoRepository, IDeudaRepository deudaRepository, IPersonaRepository personaRepository)
        {
            _pagoRepository = pagoRepository;
            _deudaRepository = deudaRepository;
            _personaRepository = personaRepository;
        }

        public IActionResult Index()
        {
            var pagos = _pagoRepository.GetAll();
            return View(pagos);
        }

        public IActionResult Create()
        {
            var deudasPendientes = _deudaRepository.GetAll().Where(d => !d.Pagada).Select(d => new
            {
                IdDeuda = d.IdDeuda,
                Descripcion = $"{d.TipoServicio} - Puesto {d.NumeroPuesto} - {d.Monto:C}"
            }).ToList();

            ViewBag.Deudas = new SelectList(deudasPendientes, "IdDeuda", "Descripcion");
            ViewBag.Personas = new SelectList(_personaRepository.GetAll(), "IdPersona", "Nombres");
            
            return View(new Pago { FechaPago = DateTime.Today });
        }

        [HttpPost]
        public IActionResult Create(Pago pago)
        {
            if (ModelState.IsValid)
            {
                _pagoRepository.RegistrarPago(pago);
                TempData["Mensaje"] = "Pago registrado correctamente. La deuda ha sido marcada como pagada.";
                return RedirectToAction(nameof(Index));
            }

            var deudasPendientes = _deudaRepository.GetAll().Where(d => !d.Pagada).Select(d => new
            {
                IdDeuda = d.IdDeuda,
                Descripcion = $"{d.TipoServicio} - Puesto {d.NumeroPuesto} - {d.Monto:C}"
            }).ToList();

            ViewBag.Deudas = new SelectList(deudasPendientes, "IdDeuda", "Descripcion", pago.IdDeuda);
            ViewBag.Personas = new SelectList(_personaRepository.GetAll(), "IdPersona", "Nombres", pago.IdPersona);
            
            return View(pago);
        }

        public IActionResult Details(int id)
        {
            var pago = _pagoRepository.GetById(id);
            if (pago == null) return NotFound();
            return View(pago);
        }

        public IActionResult Edit(int id)
        {
            var pago = _pagoRepository.GetById(id);
            if (pago == null) return NotFound();

            var deudasPendientes = _deudaRepository.GetAll().Select(d => new
            {
                IdDeuda = d.IdDeuda,
                Descripcion = $"{d.TipoServicio} - Puesto {d.NumeroPuesto} - {d.Monto:C}"
            }).ToList();

            ViewBag.Deudas = new SelectList(deudasPendientes, "IdDeuda", "Descripcion", pago.IdDeuda);
            ViewBag.Personas = new SelectList(_personaRepository.GetAll(), "IdPersona", "Nombres", pago.IdPersona);
            
            return View(pago);
        }

        [HttpPost]
        public IActionResult Edit(Pago pago)
        {
            if (ModelState.IsValid)
            {
                _pagoRepository.Update(pago);
                TempData["Mensaje"] = "Pago actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }

            var deudasPendientes = _deudaRepository.GetAll().Select(d => new
            {
                IdDeuda = d.IdDeuda,
                Descripcion = $"{d.TipoServicio} - Puesto {d.NumeroPuesto} - {d.Monto:C}"
            }).ToList();

            ViewBag.Deudas = new SelectList(deudasPendientes, "IdDeuda", "Descripcion", pago.IdDeuda);
            ViewBag.Personas = new SelectList(_personaRepository.GetAll(), "IdPersona", "Nombres", pago.IdPersona);
            
            return View(pago);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var pago = _pagoRepository.GetById(id);
            if (pago != null)
            {
                var deuda = _deudaRepository.GetById(pago.IdDeuda);
                if (deuda != null)
                {
                    deuda.Pagada = false;
                    _deudaRepository.Update(deuda);
                }
            }
            _pagoRepository.Delete(id);
            TempData["Mensaje"] = "Pago eliminado correctamente. La deuda ha sido marcada como pendiente.";
            return RedirectToAction(nameof(Index));
        }
    }
}
