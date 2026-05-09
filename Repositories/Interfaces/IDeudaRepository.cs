using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MercadoApp.Models;

namespace MercadoApp.Repositories.Interfaces
{
    public interface IDeudaRepository
    {
        Task<IEnumerable<Deuda>> GetAllAsync();
        Task<IEnumerable<Deuda>> GetByPuestoAsync(int idPuesto);
        Task<Deuda?> GetByIdAsync(int id);
        Task<int> CreateAsync(Deuda deuda);
        Task UpdateAsync(Deuda deuda);
        Task DeleteAsync(int id);
        Task<int> GetCountByEstadoAsync(bool pagada);
        Task<decimal> GetTotalMontoAsync(bool pagada);
        Task<IEnumerable<Deuda>> GetByFechaAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<IEnumerable<Deuda>> GetRecentAsync(int count);
    }
}