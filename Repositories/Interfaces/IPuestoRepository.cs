using System.Collections.Generic;
using MercadoApp.Models;

namespace MercadoApp.Repositories.Interfaces
{
    public interface IPuestoRepository
    {
        IEnumerable<Puesto> GetAll();
        Puesto GetById(int id);
        void Create(Puesto puesto);
        void Update(Puesto puesto);
        void Delete(int id);
        int GetCountByEstado(string estado);
        IEnumerable<Puesto> GetRecent(int count);
    }
}
