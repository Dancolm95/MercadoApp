using Microsoft.AspNetCore.Mvc;
using MercadoApp.Models;
using MercadoApp.Repositories.Interfaces;
using System.Linq;

namespace MercadoApp.Controllers
{
    public class PersonasController : Controller
    {
        private readonly IPersonaRepository _personaRepository;

        public PersonasController(IPersonaRepository personaRepository)
        {
            _personaRepository = personaRepository;
        }

        public IActionResult Index()
        {
            var personas = _personaRepository.GetAll();
            return View(personas.ToList());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Persona p)
        {
            if (ModelState.IsValid)
            {
                _personaRepository.Create(p);
                TempData["Mensaje"] = "Persona registrada correctamente en la base de datos.";
                return RedirectToAction(nameof(Index));
            }
            return View(p);
        }

        public IActionResult Edit(int id)
        {
            var persona = _personaRepository.GetById(id);
            if (persona == null) return NotFound();
            return View(persona);
        }

        [HttpPost]
        public IActionResult Edit(Persona p)
        {
            if (ModelState.IsValid)
            {
                _personaRepository.Update(p);
                TempData["Mensaje"] = "Persona actualizada correctamente en la base de datos.";
                return RedirectToAction(nameof(Index));
            }
            return View(p);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            _personaRepository.Delete(id);
            TempData["Mensaje"] = "Persona eliminada correctamente de la base de datos.";
            return RedirectToAction(nameof(Index));
        }
    }
}
