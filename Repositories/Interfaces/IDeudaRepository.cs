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
    }
}
