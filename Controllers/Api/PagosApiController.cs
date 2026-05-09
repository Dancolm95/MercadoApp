using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MercadoApp.Models;
using MercadoApp.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MercadoApp.Controllers.Api
{
    [Route("api/pagos")]
    [ApiController]
    [Authorize(Roles = "Admin,Cajero")]
    public class PagosApiController : ControllerBase
    {
        private readonly IPagoRepository _pagoRepository;

        public PagosApiController(IPagoRepository pagoRepository)
        {
            _pagoRepository = pagoRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pago>>> GetPagos()
        {
            var pagos = await _pagoRepository.GetAllAsync();
            return Ok(pagos);
        }

        [HttpPost]
        public async Task<ActionResult<Pago>> PostPago([FromBody] Pago pago)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _pagoRepository.CreateAsync(pago);
            
            return Ok(new { message = "Pago registrado exitosamente", pago });
        }
    }
}