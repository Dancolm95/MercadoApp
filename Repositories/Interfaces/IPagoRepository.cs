using System.Collections.Generic;
using MercadoApp.Models;

namespace MercadoApp.Repositories.Interfaces
{
    public interface IPagoRepository
    {
        IEnumerable<Pago> GetAll();
        Pago? GetById(int id);
        void RegistrarPago(Pago pago);
        void Update(Pago pago);
        void Delete(int id);
        int GetCount();
        decimal GetTotalIngresos();
        IEnumerable<Pago> GetByFecha(DateTime fechaInicio, DateTime fechaFin);
        IEnumerable<Pago> GetRecent(int count);
    }
}
