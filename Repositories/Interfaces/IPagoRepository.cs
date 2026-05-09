using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MercadoApp.Models;

namespace MercadoApp.Repositories.Interfaces
{
    public interface IPagoRepository
    {
        Task<IEnumerable<Pago>> GetAllAsync();
        Task<Pago?> GetByIdAsync(int id);
        Task<int> CreateAsync(Pago pago);
        Task UpdateAsync(Pago pago);
        Task DeleteAsync(int id);
        Task<int> GetCountAsync();
        Task<decimal> GetTotalIngresosAsync();
        Task<IEnumerable<Pago>> GetByFechaAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<IEnumerable<Pago>> GetRecentAsync(int count);
    }
}