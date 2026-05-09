using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using MercadoApp.Data;
using MercadoApp.Models;
using MercadoApp.Repositories.Interfaces;

namespace MercadoApp.Repositories
{
    public class PuestoRepository : IPuestoRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public PuestoRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Puesto>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryAsync<Puesto>(
                "SELECT IdPuesto AS Id, NumeroPuesto, Seccion, AreaM2, Estado, MontoMensual, IdPersona FROM Puesto");
        }

        public async Task<Puesto?> GetByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<Puesto>(
                "SELECT IdPuesto AS Id, NumeroPuesto, Seccion, AreaM2, Estado, MontoMensual, IdPersona FROM Puesto WHERE IdPuesto = @Id",
                new { Id = id });
        }

        public async Task<int> CreateAsync(Puesto puesto)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"INSERT INTO Puesto (NumeroPuesto, Seccion, AreaM2, Estado, MontoMensual, IdPersona) 
                        VALUES (@NumeroPuesto, @Seccion, @AreaM2, @Estado, @MontoMensual, @IdPersona);
                        SELECT CAST(SCOPE_IDENTITY() AS INT);";
            return await connection.ExecuteScalarAsync<int>(sql, puesto);
        }

        public async Task UpdateAsync(Puesto puesto)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"UPDATE Puesto SET NumeroPuesto = @NumeroPuesto, Seccion = @Seccion, 
                        AreaM2 = @AreaM2, Estado = @Estado, MontoMensual = @MontoMensual, IdPersona = @IdPersona 
                        WHERE IdPuesto = @Id";
            await connection.ExecuteAsync(sql, puesto);
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.ExecuteAsync("DELETE FROM Puesto WHERE IdPuesto = @Id", new { Id = id });
        }

        public async Task<int> GetCountByEstadoAsync(string estado)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM Puesto WHERE Estado = @Estado", new { Estado = estado });
        }

        public async Task<IEnumerable<Puesto>> GetRecentAsync(int count)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryAsync<Puesto>(
                $"SELECT TOP {count} IdPuesto AS Id, NumeroPuesto, Seccion, AreaM2, Estado, MontoMensual, IdPersona FROM Puesto ORDER BY IdPuesto DESC");
        }
    }
}