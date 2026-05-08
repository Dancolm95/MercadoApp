using System.Collections.Generic;
using MercadoApp.Models;

namespace MercadoApp.Repositories.Interfaces
{
    public interface IDeudaRepository
    {
        IEnumerable<Deuda> GetAll();
        IEnumerable<Deuda> GetByPuesto(int idPuesto);
        Deuda? GetById(int id);
        void RegistrarDeuda(Deuda deuda);
        void Update(Deuda deuda);
        void Delete(int id);
        int GetCountByEstado(bool pagada);
        decimal GetTotalMonto(bool pagada);
        IEnumerable<Deuda> GetByFecha(DateTime fechaInicio, DateTime fechaFin);
        IEnumerable<Deuda> GetRecent(int count);
    }
}
