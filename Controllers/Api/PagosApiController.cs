using Microsoft.AspNetCore.Mvc;
using MercadoApp.Models;
using MercadoApp.Repositories.Interfaces;
using System.Collections.Generic;

namespace MercadoApp.Controllers.Api
{
    [Route("api/pagos")]
    [ApiController]
    public class PagosApiController : ControllerBase
    {
        private readonly IPagoRepository _pagoRepository;

        public PagosApiController(IPagoRepository pagoRepository)
        {
            _pagoRepository = pagoRepository;
        }

        // GET: api/pagosapi
        [HttpGet]
        public ActionResult<IEnumerable<Pago>> GetPagos()
        {
            var pagos = _pagoRepository.GetAll();
            return Ok(pagos);
        }

        // POST: api/pagosapi
        [HttpPost]
        public ActionResult<Pago> PostPago([FromBody] Pago pago)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _pagoRepository.RegistrarPago(pago);
            
            return Ok(new { message = "Pago registrado exitosamente", pago });
        }
    }
}
