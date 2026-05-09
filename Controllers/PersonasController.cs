using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MercadoApp.Models;
using MercadoApp.Repositories.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace MercadoApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PersonasController : Controller
    {
        private readonly IPersonaRepository _personaRepository;

        public PersonasController(IPersonaRepository personaRepository)
        {
            _personaRepository = personaRepository;
        }

        public async Task<IActionResult> Index()
        {
            var personas = await _personaRepository.GetAllAsync();
            return View(personas.ToList());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Persona p)
        {
            if (ModelState.IsValid)
            {
                await _personaRepository.CreateAsync(p);
                TempData["Mensaje"] = "Persona registrada correctamente en la base de datos.";
                return RedirectToAction(nameof(Index));
            }
            return View(p);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var persona = await _personaRepository.GetByIdAsync(id);
            if (persona == null) return NotFound();
            return View(persona);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Persona p)
        {
            if (ModelState.IsValid)
            {
                await _personaRepository.UpdateAsync(p);
                TempData["Mensaje"] = "Persona actualizada correctamente en la base de datos.";
                return RedirectToAction(nameof(Index));
            }
            return View(p);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _personaRepository.DeleteAsync(id);
            TempData["Mensaje"] = "Persona eliminada correctamente de la base de datos.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var persona = await _personaRepository.GetByIdAsync(id);
            if (persona == null) return NotFound();
            return View(persona);
        }
    }
}