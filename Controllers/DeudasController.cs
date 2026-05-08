using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MercadoApp.Models;
using MercadoApp.Repositories.Interfaces;
using System.Linq;
using System;

namespace MercadoApp.Controllers
{
    public class DeudasController : Controller
    {
        private readonly IDeudaRepository _deudaRepository;
        private readonly IPuestoRepository _puestoRepository;

        public DeudasController(IDeudaRepository deudaRepository, IPuestoRepository puestoRepository)
        {
            _deudaRepository = deudaRepository;
            _puestoRepository = puestoRepository;
        }

        public IActionResult Index(bool? pagada = null)
        {
            var deudas = _deudaRepository.GetAll();
            if (pagada.HasValue)
            {
                deudas = deudas.Where(d => d.Pagada == pagada.Value);
            }
            return View(deudas.ToList());
        }

        public IActionResult Create()
        {
            ViewBag.Puestos = new SelectList(_puestoRepository.GetAll(), "Id", "NumeroPuesto");
            return View(new Deuda { FechaGenerada = DateTime.Today });
        }

        [HttpPost]
        public IActionResult Create(Deuda deuda)
        {
            if (ModelState.IsValid)
            {
                _deudaRepository.RegistrarDeuda(deuda);
                TempData["Mensaje"] = "Deuda registrada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Puestos = new SelectList(_puestoRepository.GetAll(), "Id", "NumeroPuesto", deuda.IdPuesto);
            return View(deuda);
        }

        public IActionResult Details(int id)
        {
            var deuda = _deudaRepository.GetById(id);
            if (deuda == null) return NotFound();
            return View(deuda);
        }

        public IActionResult Edit(int id)
        {
            var deuda = _deudaRepository.GetById(id);
            if (deuda == null) return NotFound();
            ViewBag.Puestos = new SelectList(_puestoRepository.GetAll(), "Id", "NumeroPuesto", deuda.IdPuesto);
            return View(deuda);
        }

        [HttpPost]
        public IActionResult Edit(Deuda deuda)
        {
            if (ModelState.IsValid)
            {
                _deudaRepository.Update(deuda);
                TempData["Mensaje"] = "Deuda actualizada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Puestos = new SelectList(_puestoRepository.GetAll(), "Id", "NumeroPuesto", deuda.IdPuesto);
            return View(deuda);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            _deudaRepository.Delete(id);
            TempData["Mensaje"] = "Deuda eliminada correctamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}
