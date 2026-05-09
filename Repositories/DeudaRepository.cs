using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using MercadoApp.Data;
using MercadoApp.Models;
using MercadoApp.Repositories.Interfaces;

namespace MercadoApp.Repositories
{
    public class DeudaRepository : IDeudaRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public DeudaRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Deuda>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"SELECT d.IdDeuda, d.IdPuesto, d.TipoServicio, d.Monto, d.FechaGenerada, d.Pagada, p.NumeroPuesto 
                        FROM Deuda d INNER JOIN Puesto p ON d.IdPuesto = p.IdPuesto";
            return await connection.QueryAsync<Deuda>(sql);
        }

        public async Task<IEnumerable<Deuda>> GetByPuestoAsync(int idPuesto)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"SELECT d.IdDeuda, d.IdPuesto, d.TipoServicio, d.Monto, d.FechaGenerada, d.Pagada, p.NumeroPuesto 
                        FROM Deuda d INNER JOIN Puesto p ON d.IdPuesto = p.IdPuesto
                        WHERE d.IdPuesto = @IdPuesto";
            return await connection.QueryAsync<Deuda>(sql, new { IdPuesto = idPuesto });
        }

        public async Task<Deuda?> GetByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"SELECT d.IdDeuda, d.IdPuesto, d.TipoServicio, d.Monto, d.FechaGenerada, d.Pagada, p.NumeroPuesto 
                        FROM Deuda d INNER JOIN Puesto p ON d.IdPuesto = p.IdPuesto
                        WHERE d.IdDeuda = @Id";
            return await connection.QuerySingleOrDefaultAsync<Deuda>(sql, new { Id = id });
        }

        public async Task<int> CreateAsync(Deuda deuda)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();
            try
            {
                var sql = @"INSERT INTO Deuda (IdPuesto, TipoServicio, Monto, FechaGenerada, Pagada) 
                            VALUES (@IdPuesto, @TipoServicio, @Monto, @FechaGenerada, @Pagada);
                            SELECT CAST(SCOPE_IDENTITY() AS INT);";
                var id = await connection.ExecuteScalarAsync<int>(sql, deuda, transaction);
                transaction.Commit();
                return id;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task UpdateAsync(Deuda deuda)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"UPDATE Deuda SET IdPuesto = @IdPuesto, TipoServicio = @TipoServicio, 
                        Monto = @Monto, FechaGenerada = @FechaGenerada, Pagada = @Pagada 
                        WHERE IdDeuda = @IdDeuda";
            await connection.ExecuteAsync(sql, deuda);
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.ExecuteAsync("DELETE FROM Deuda WHERE IdDeuda = @Id", new { Id = id });
        }

        public async Task<int> GetCountByEstadoAsync(bool pagada)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM Deuda WHERE Pagada = @Pagada", new { Pagada = pagada });
        }

        public async Task<decimal> GetTotalMontoAsync(bool pagada)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.ExecuteScalarAsync<decimal>(
                "SELECT ISNULL(SUM(Monto), 0) FROM Deuda WHERE Pagada = @Pagada", new { Pagada = pagada });
        }

        public async Task<IEnumerable<Deuda>> GetByFechaAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"SELECT d.IdDeuda, d.IdPuesto, d.TipoServicio, d.Monto, d.FechaGenerada, d.Pagada, p.NumeroPuesto 
                        FROM Deuda d INNER JOIN Puesto p ON d.IdPuesto = p.IdPuesto
                        WHERE d.FechaGenerada BETWEEN @FechaInicio AND @FechaFin
                        ORDER BY d.FechaGenerada DESC";
            return await connection.QueryAsync<Deuda>(sql, new { FechaInicio = fechaInicio, FechaFin = fechaFin });
        }

        public async Task<IEnumerable<Deuda>> GetRecentAsync(int count)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = $@"SELECT TOP {count} d.IdDeuda, d.IdPuesto, d.TipoServicio, d.Monto, d.FechaGenerada, d.Pagada, p.NumeroPuesto 
                         FROM Deuda d INNER JOIN Puesto p ON d.IdPuesto = p.IdPuesto
                         ORDER BY d.IdDeuda DESC";
            return await connection.QueryAsync<Deuda>(sql);
        }
    }
}