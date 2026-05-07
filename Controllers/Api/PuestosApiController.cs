using Microsoft.AspNetCore.Mvc;
using MercadoApp.Models;
using MercadoApp.Repositories.Interfaces;
using System.Collections.Generic;

namespace MercadoApp.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class PuestosApiController : ControllerBase
    {
        private readonly IPuestoRepository _puestoRepository;

        public PuestosApiController(IPuestoRepository puestoRepository)
        {
            _puestoRepository = puestoRepository;
        }

        // GET: api/puestosapi
        [HttpGet]
        public ActionResult<IEnumerable<Puesto>> GetPuestos()
        {
            var puestos = _puestoRepository.GetAll();
            return Ok(puestos);
        }

        // GET: api/puestosapi/5
        [HttpGet("{id}")]
        public ActionResult<Puesto> GetPuesto(int id)
        {
            var puesto = _puestoRepository.GetById(id);

            if (puesto == null)
            {
                return NotFound();
            }

            return Ok(puesto);
        }

        // POST: api/puestosapi
        [HttpPost]
        public ActionResult<Puesto> PostPuesto([FromBody] Puesto puesto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _puestoRepository.Create(puesto);
            
            // Assuming Create populates the Id, if not, this still returns Ok
            return CreatedAtAction(nameof(GetPuesto), new { id = puesto.Id }, puesto);
        }

        // PUT: api/puestosapi/5
        [HttpPut("{id}")]
        public IActionResult PutPuesto(int id, [FromBody] Puesto puesto)
        {
            if (id != puesto.Id)
            {
                return BadRequest("El ID del puesto no coincide.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingPuesto = _puestoRepository.GetById(id);
            if (existingPuesto == null)
            {
                return NotFound();
            }

            _puestoRepository.Update(puesto);

            return NoContent();
        }

        // DELETE: api/puestosapi/5
        [HttpDelete("{id}")]
        public IActionResult DeletePuesto(int id)
        {
            var puesto = _puestoRepository.GetById(id);
            if (puesto == null)
            {
                return NotFound();
            }

            _puestoRepository.Delete(id);

            return NoContent();
        }
    }
}
