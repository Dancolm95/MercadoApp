using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MercadoApp.Models;
using MercadoApp.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MercadoApp.Controllers.Api
{
    [Route("api/puestos")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class PuestosApiController : ControllerBase
    {
        private readonly IPuestoRepository _puestoRepository;

        public PuestosApiController(IPuestoRepository puestoRepository)
        {
            _puestoRepository = puestoRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Puesto>>> GetPuestos()
        {
            var puestos = await _puestoRepository.GetAllAsync();
            return Ok(puestos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Puesto>> GetPuesto(int id)
        {
            var puesto = await _puestoRepository.GetByIdAsync(id);

            if (puesto == null)
            {
                return NotFound();
            }

            return Ok(puesto);
        }

        [HttpPost]
        public async Task<ActionResult<Puesto>> PostPuesto([FromBody] Puesto puesto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _puestoRepository.CreateAsync(puesto);
            
            return CreatedAtAction(nameof(GetPuesto), new { id = puesto.Id }, puesto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPuesto(int id, [FromBody] Puesto puesto)
        {
            if (id != puesto.Id)
            {
                return BadRequest("El ID del puesto no coincide.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingPuesto = await _puestoRepository.GetByIdAsync(id);
            if (existingPuesto == null)
            {
                return NotFound();
            }

            await _puestoRepository.UpdateAsync(puesto);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePuesto(int id)
        {
            var puesto = await _puestoRepository.GetByIdAsync(id);
            if (puesto == null)
            {
                return NotFound();
            }

            await _puestoRepository.DeleteAsync(id);

            return NoContent();
        }
    }
}