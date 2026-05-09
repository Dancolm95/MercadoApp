using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MercadoApp.Models;
using MercadoApp.Repositories.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MercadoApp.Controllers
{
    [Authorize(Roles = "Admin,Cajero")]
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

        public async Task<IActionResult> Index()
        {
            var pagos = await _pagoRepository.GetAllAsync();
            return View(pagos);
        }

        public async Task<IActionResult> Create()
        {
            var deudas = await _deudaRepository.GetAllAsync();
            var deudasPendientes = deudas.Where(d => !d.Pagada).Select(d => new
            {
                IdDeuda = d.IdDeuda,
                Descripcion = $"{d.TipoServicio} - Puesto {d.NumeroPuesto} - {d.Monto:C}"
            }).ToList();

            var personas = await _personaRepository.GetAllAsync();
            
            ViewBag.Deudas = new SelectList(deudasPendientes, "IdDeuda", "Descripcion");
            ViewBag.Personas = new SelectList(personas, "IdPersona", "Nombres");
            
            return View(new Pago { FechaPago = DateTime.Today });
        }

        [HttpPost]
        public async Task<IActionResult> Create(Pago pago)
        {
            if (ModelState.IsValid)
            {
                await _pagoRepository.CreateAsync(pago);
                TempData["Mensaje"] = "Pago registrado correctamente. La deuda ha sido marcada como pagada.";
                return RedirectToAction(nameof(Index));
            }

            var deudas = await _deudaRepository.GetAllAsync();
            var deudasPendientes = deudas.Where(d => !d.Pagada).Select(d => new
            {
                IdDeuda = d.IdDeuda,
                Descripcion = $"{d.TipoServicio} - Puesto {d.NumeroPuesto} - {d.Monto:C}"
            }).ToList();

            var personas = await _personaRepository.GetAllAsync();

            ViewBag.Deudas = new SelectList(deudasPendientes, "IdDeuda", "Descripcion", pago.IdDeuda);
            ViewBag.Personas = new SelectList(personas, "IdPersona", "Nombres", pago.IdPersona);
            
            return View(pago);
        }

        public async Task<IActionResult> Details(int id)
        {
            var pago = await _pagoRepository.GetByIdAsync(id);
            if (pago == null) return NotFound();
            return View(pago);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var pago = await _pagoRepository.GetByIdAsync(id);
            if (pago == null) return NotFound();

            var deudas = await _deudaRepository.GetAllAsync();
            var deudasList = deudas.Select(d => new
            {
                IdDeuda = d.IdDeuda,
                Descripcion = $"{d.TipoServicio} - Puesto {d.NumeroPuesto} - {d.Monto:C}"
            }).ToList();

            var personas = await _personaRepository.GetAllAsync();

            ViewBag.Deudas = new SelectList(deudasList, "IdDeuda", "Descripcion", pago.IdDeuda);
            ViewBag.Personas = new SelectList(personas, "IdPersona", "Nombres", pago.IdPersona);
            
            return View(pago);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Pago pago)
        {
            if (ModelState.IsValid)
            {
                await _pagoRepository.UpdateAsync(pago);
                TempData["Mensaje"] = "Pago actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }

            var deudas = await _deudaRepository.GetAllAsync();
            var deudasList = deudas.Select(d => new
            {
                IdDeuda = d.IdDeuda,
                Descripcion = $"{d.TipoServicio} - Puesto {d.NumeroPuesto} - {d.Monto:C}"
            }).ToList();

            var personas = await _personaRepository.GetAllAsync();

            ViewBag.Deudas = new SelectList(deudasList, "IdDeuda", "Descripcion", pago.IdDeuda);
            ViewBag.Personas = new SelectList(personas, "IdPersona", "Nombres", pago.IdPersona);
            
            return View(pago);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var pago = await _pagoRepository.GetByIdAsync(id);
            if (pago != null)
            {
                var deuda = await _deudaRepository.GetByIdAsync(pago.IdDeuda);
                if (deuda != null)
                {
                    deuda.Pagada = false;
                    await _deudaRepository.UpdateAsync(deuda);
                }
            }
            await _pagoRepository.DeleteAsync(id);
            TempData["Mensaje"] = "Pago eliminado correctamente. La deuda ha sido marcada como pendiente.";
            return RedirectToAction(nameof(Index));
        }
    }
}