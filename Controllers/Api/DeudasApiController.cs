using Microsoft.AspNetCore.Mvc;
using MercadoApp.Models;
using MercadoApp.Repositories.Interfaces;
using System.Collections.Generic;

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

        // GET: api/deudas
        [HttpGet]
        public ActionResult<IEnumerable<Deuda>> GetDeudas()
        {
            var deudas = _deudaRepository.GetAll();
            return Ok(deudas);
        }

        // GET: api/deudas/puesto/5
        [HttpGet("puesto/{idPuesto}")]
        public ActionResult<IEnumerable<Deuda>> GetDeudasByPuesto(int idPuesto)
        {
            var deudas = _deudaRepository.GetByPuesto(idPuesto);
            return Ok(deudas);
        }

        // POST: api/deudas
        [HttpPost]
        public ActionResult<Deuda> PostDeuda([FromBody] Deuda deuda)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _deudaRepository.RegistrarDeuda(deuda);
            
            return Ok(new { message = "Deuda registrada exitosamente", deuda });
        }
    }
}
