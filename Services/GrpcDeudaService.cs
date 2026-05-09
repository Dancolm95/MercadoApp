using Grpc.Core;
using MercadoApp.Protos;
using MercadoApp.Repositories.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace MercadoApp.Services
{
    public class GrpcDeudaService : DeudaService.DeudaServiceBase
    {
        private readonly IDeudaRepository _deudaRepository;

        public GrpcDeudaService(IDeudaRepository deudaRepository)
        {
            _deudaRepository = deudaRepository;
        }

        public override async Task<DeudaListResponse> GetDeudas(EmptyRequest request, ServerCallContext context)
        {
            var deudas = await _deudaRepository.GetAllAsync();
            var response = new DeudaListResponse();

            response.Deudas.AddRange(deudas.Select(d => new DeudaMessage
            {
                IdDeuda = d.IdDeuda,
                IdPuesto = d.IdPuesto,
                TipoServicio = d.TipoServicio ?? "",
                Monto = (double)d.Monto,
                FechaGenerada = d.FechaGenerada.ToString("yyyy-MM-dd"),
                Pagada = d.Pagada,
                NumeroPuesto = d.NumeroPuesto ?? ""
            }));

            return response;
        }

        public override async Task<DeudaListResponse> GetDeudasByPuesto(PuestoRequest request, ServerCallContext context)
        {
            var deudas = await _deudaRepository.GetByPuestoAsync(request.IdPuesto);
            var response = new DeudaListResponse();

            response.Deudas.AddRange(deudas.Select(d => new DeudaMessage
            {
                IdDeuda = d.IdDeuda,
                IdPuesto = d.IdPuesto,
                TipoServicio = d.TipoServicio ?? "",
                Monto = (double)d.Monto,
                FechaGenerada = d.FechaGenerada.ToString("yyyy-MM-dd"),
                Pagada = d.Pagada,
                NumeroPuesto = d.NumeroPuesto ?? ""
            }));

            return response;
        }
    }
}