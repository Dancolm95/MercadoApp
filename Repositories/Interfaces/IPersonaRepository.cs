using System.Collections.Generic;
using System.Threading.Tasks;
using MercadoApp.Models;

namespace MercadoApp.Repositories.Interfaces
{
    public interface IPersonaRepository
    {
        Task<IEnumerable<Persona>> GetAllAsync();
        Task<Persona?> GetByIdAsync(int id);
        Task<int> CreateAsync(Persona persona);
        Task UpdateAsync(Persona persona);
        Task DeleteAsync(int id);
    }
}