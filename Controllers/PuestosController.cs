using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MercadoApp.Models;
using MercadoApp.Repositories.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace MercadoApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PuestosController : Controller
    {
        private readonly IPuestoRepository _puestoRepository;

        public PuestosController(IPuestoRepository puestoRepository)
        {
            _puestoRepository = puestoRepository;
        }

        public async Task<IActionResult> Index(string? estado = null)
        {
            var puestos = await _puestoRepository.GetAllAsync();
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
        public async Task<IActionResult> Create(Puesto p)
        {
            if (ModelState.IsValid)
            {
                await _puestoRepository.CreateAsync(p);
                TempData["Mensaje"] = "Puesto creado correctamente en la base de datos.";
                return RedirectToAction(nameof(Index));
            }
            return View(p);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var puesto = await _puestoRepository.GetByIdAsync(id);
            if (puesto == null) return NotFound();
            return View(puesto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Puesto p)
        {
            if (ModelState.IsValid)
            {
                await _puestoRepository.UpdateAsync(p);
                TempData["Mensaje"] = "Puesto actualizado correctamente en la base de datos.";
                return RedirectToAction(nameof(Index));
            }
            return View(p);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _puestoRepository.DeleteAsync(id);
            TempData["Mensaje"] = "Puesto eliminado correctamente de la base de datos.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var puesto = await _puestoRepository.GetByIdAsync(id);
            if (puesto == null) return NotFound();
            return View(puesto);
        }
    }
}