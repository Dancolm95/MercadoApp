using Microsoft.AspNetCore.Mvc;
using MercadoApp.Models;
using MercadoApp.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MercadoApp.Controllers.Api
{
    [Route("api/deudas")]
    [ApiController]
    public class DeudasApiController : ControllerBase
    {
        private readonly IDeudaRepository _deudaRepository;

        public DeudasApiController(IDeudaRepository deudaRepository)
        {
            _deudaRepository = deudaRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Deuda>>> GetDeudas()
        {
            var deudas = await _deudaRepository.GetAllAsync();
            return Ok(deudas);
        }

        [HttpGet("puesto/{idPuesto}")]
        public async Task<ActionResult<IEnumerable<Deuda>>> GetDeudasByPuesto(int idPuesto)
        {
            var deudas = await _deudaRepository.GetByPuestoAsync(idPuesto);
            return Ok(deudas);
        }

        [HttpPost]
        public async Task<ActionResult<Deuda>> PostDeuda([FromBody] Deuda deuda)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _deudaRepository.CreateAsync(deuda);
            
            return Ok(new { message = "Deuda registrada exitosamente", deuda });
        }
    }
}