using System.Collections.Generic;
using System.Threading.Tasks;
using MercadoApp.Models;

namespace MercadoApp.Repositories.Interfaces
{
    public interface IPuestoRepository
    {
        Task<IEnumerable<Puesto>> GetAllAsync();
        Task<Puesto?> GetByIdAsync(int id);
        Task<int> CreateAsync(Puesto puesto);
        Task UpdateAsync(Puesto puesto);
        Task DeleteAsync(int id);
        Task<int> GetCountByEstadoAsync(string estado);
        Task<IEnumerable<Puesto>> GetRecentAsync(int count);
    }
}