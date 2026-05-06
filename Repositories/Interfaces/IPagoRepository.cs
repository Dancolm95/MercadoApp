using System.Collections.Generic;
using MercadoApp.Models;

namespace MercadoApp.Repositories.Interfaces
{
    public interface IPagoRepository
    {
        IEnumerable<Pago> GetAll();
        void RegistrarPago(Pago pago);
    }
}
