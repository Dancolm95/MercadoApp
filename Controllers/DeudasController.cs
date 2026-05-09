using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MercadoApp.Models;
using MercadoApp.Repositories.Interfaces;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace MercadoApp.Controllers
{
    [Authorize(Roles = "Admin,Cajero")]
    public class DeudasController : Controller
    {
        private readonly IDeudaRepository _deudaRepository;
        private readonly IPuestoRepository _puestoRepository;

        public DeudasController(IDeudaRepository deudaRepository, IPuestoRepository puestoRepository)
        {
            _deudaRepository = deudaRepository;
            _puestoRepository = puestoRepository;
        }

        public async Task<IActionResult> Index(bool? pagada = null)
        {
            var deudas = await _deudaRepository.GetAllAsync();
            if (pagada.HasValue)
            {
                deudas = deudas.Where(d => d.Pagada == pagada.Value);
            }
            return View(deudas.ToList());
        }

        public async Task<IActionResult> Create()
        {
            var puestos = await _puestoRepository.GetAllAsync();
            ViewBag.Puestos = new SelectList(puestos, "Id", "NumeroPuesto");
            return View(new Deuda { FechaGenerada = DateTime.Today });
        }

        [HttpPost]
        public async Task<IActionResult> Create(Deuda deuda)
        {
            if (ModelState.IsValid)
            {
                await _deudaRepository.CreateAsync(deuda);
                TempData["Mensaje"] = "Deuda registrada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            var puestos = await _puestoRepository.GetAllAsync();
            ViewBag.Puestos = new SelectList(puestos, "Id", "NumeroPuesto", deuda.IdPuesto);
            return View(deuda);
        }

        public async Task<IActionResult> Details(int id)
        {
            var deuda = await _deudaRepository.GetByIdAsync(id);
            if (deuda == null) return NotFound();
            return View(deuda);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var deuda = await _deudaRepository.GetByIdAsync(id);
            if (deuda == null) return NotFound();
            var puestos = await _puestoRepository.GetAllAsync();
            ViewBag.Puestos = new SelectList(puestos, "Id", "NumeroPuesto", deuda.IdPuesto);
            return View(deuda);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Deuda deuda)
        {
            if (ModelState.IsValid)
            {
                await _deudaRepository.UpdateAsync(deuda);
                TempData["Mensaje"] = "Deuda actualizada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            var puestos = await _puestoRepository.GetAllAsync();
            ViewBag.Puestos = new SelectList(puestos, "Id", "NumeroPuesto", deuda.IdPuesto);
            return View(deuda);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _deudaRepository.DeleteAsync(id);
            TempData["Mensaje"] = "Deuda eliminada correctamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}