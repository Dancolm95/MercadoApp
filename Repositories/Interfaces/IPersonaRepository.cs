using System.Collections.Generic;
using MercadoApp.Models;

namespace MercadoApp.Repositories.Interfaces
{
    public interface IPersonaRepository
    {
        IEnumerable<Persona> GetAll();
        Persona GetById(int id);
        void Create(Persona persona);
        void Update(Persona persona);
        void Delete(int id);
    }
}
