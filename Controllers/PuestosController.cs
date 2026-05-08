using Microsoft.AspNetCore.Mvc;
using MercadoApp.Models;
using MercadoApp.Repositories.Interfaces;
using System.Linq;

namespace MercadoApp.Controllers
{
    public class PuestosController : Controller
    {
        private readonly IPuestoRepository _puestoRepository;

        public PuestosController(IPuestoRepository puestoRepository)
        {
            _puestoRepository = puestoRepository;
        }

        public IActionResult Index(string? estado = null)
        {
            var puestos = _puestoRepository.GetAll();
            if (!string.IsNullOrEmpty(estado))
            {
                puestos = puestos.Where(p => p.Estado == estado);
            }
            return View(puestos.ToList());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Puesto p)
        {
            if (ModelState.IsValid)
            {
                _puestoRepository.Create(p);
                TempData["Mensaje"] = "Puesto creado correctamente en la base de datos.";
                return RedirectToAction(nameof(Index));
            }
            return View(p);
        }

        public IActionResult Edit(int id)
        {
            var puesto = _puestoRepository.GetById(id);
            if (puesto == null) return NotFound();
            return View(puesto);
        }

        [HttpPost]
        public IActionResult Edit(Puesto p)
        {
            if (ModelState.IsValid)
            {
                _puestoRepository.Update(p);
                TempData["Mensaje"] = "Puesto actualizado correctamente en la base de datos.";
                return RedirectToAction(nameof(Index));
            }
            return View(p);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            _puestoRepository.Delete(id);
            TempData["Mensaje"] = "Puesto eliminado correctamente de la base de datos.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Details(int id)
        {
            var puesto = _puestoRepository.GetById(id);
            if (puesto == null) return NotFound();
            return View(puesto);
        }
    }
}
