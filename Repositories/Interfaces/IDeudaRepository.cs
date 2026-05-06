using System.Collections.Generic;
using MercadoApp.Models;

namespace MercadoApp.Repositories.Interfaces
{
    public interface IDeudaRepository
    {
        IEnumerable<Deuda> GetAll();
        IEnumerable<Deuda> GetByPuesto(int idPuesto);
        void RegistrarDeuda(Deuda deuda);
    }
}
