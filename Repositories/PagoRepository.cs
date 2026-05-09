using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using MercadoApp.Data;
using MercadoApp.Models;
using MercadoApp.Repositories.Interfaces;

namespace MercadoApp.Repositories
{
    public class PagoRepository : IPagoRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public PagoRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Pago>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"SELECT p.IdPago, p.IdDeuda, p.IdPersona, p.FechaPago, p.MontoPagado, p.Referencia,
                              d.TipoServicio + ' - Puesto ' + pue.NumeroPuesto AS DetallesDeuda,
                              per.Nombres + ' ' + per.Apellidos AS NombrePersona
                       FROM Pago p
                       INNER JOIN Deuda d ON p.IdDeuda = d.IdDeuda
                       INNER JOIN Puesto pue ON d.IdPuesto = pue.IdPuesto
                       INNER JOIN Persona per ON p.IdPersona = per.IdPersona";
            return await connection.QueryAsync<Pago>(sql);
        }

        public async Task<Pago?> GetByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"SELECT p.IdPago, p.IdDeuda, p.IdPersona, p.FechaPago, p.MontoPagado, p.Referencia,
                              d.TipoServicio + ' - Puesto ' + pue.NumeroPuesto AS DetallesDeuda,
                              per.Nombres + ' ' + per.Apellidos AS NombrePersona
                       FROM Pago p
                       INNER JOIN Deuda d ON p.IdDeuda = d.IdDeuda
                       INNER JOIN Puesto pue ON d.IdPuesto = pue.IdPuesto
                       INNER JOIN Persona per ON p.IdPersona = per.IdPersona
                       WHERE p.IdPago = @Id";
            return await connection.QuerySingleOrDefaultAsync<Pago>(sql, new { Id = id });
        }

        public async Task<int> CreateAsync(Pago pago)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();
            try
            {
                var sqlInsert = @"INSERT INTO Pago (IdDeuda, IdPersona, FechaPago, MontoPagado, Referencia) 
                                 VALUES (@IdDeuda, @IdPersona, @FechaPago, @MontoPagado, @Referencia);
                                 SELECT CAST(SCOPE_IDENTITY() AS INT);";
                var idPago = await connection.ExecuteScalarAsync<int>(sqlInsert, pago, transaction);

                await connection.ExecuteAsync(
                    "UPDATE Deuda SET Pagada = 1 WHERE IdDeuda = @IdDeuda",
                    new { pago.IdDeuda }, transaction);

                transaction.Commit();
                return idPago;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task UpdateAsync(Pago pago)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();
            try
            {
                var sql = @"UPDATE Pago SET IdDeuda = @IdDeuda, IdPersona = @IdPersona, 
                            FechaPago = @FechaPago, MontoPagado = @MontoPagado, Referencia = @Referencia 
                            WHERE IdPago = @IdPago";
                await connection.ExecuteAsync(sql, pago, transaction);
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.ExecuteAsync("DELETE FROM Pago WHERE IdPago = @Id", new { Id = id });
        }

        public async Task<int> GetCountAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Pago");
        }

        public async Task<decimal> GetTotalIngresosAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.ExecuteScalarAsync<decimal>("SELECT ISNULL(SUM(MontoPagado), 0) FROM Pago");
        }

        public async Task<IEnumerable<Pago>> GetByFechaAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"SELECT p.IdPago, p.IdDeuda, p.IdPersona, p.FechaPago, p.MontoPagado, p.Referencia,
                              d.TipoServicio + ' - Puesto ' + pue.NumeroPuesto AS DetallesDeuda,
                              per.Nombres + ' ' + per.Apellidos AS NombrePersona
                       FROM Pago p
                       INNER JOIN Deuda d ON p.IdDeuda = d.IdDeuda
                       INNER JOIN Puesto pue ON d.IdPuesto = pue.IdPuesto
                       INNER JOIN Persona per ON p.IdPersona = per.IdPersona
                       WHERE p.FechaPago BETWEEN @FechaInicio AND @FechaFin
                       ORDER BY p.FechaPago DESC";
            return await connection.QueryAsync<Pago>(sql, new { FechaInicio = fechaInicio, FechaFin = fechaFin });
        }

        public async Task<IEnumerable<Pago>> GetRecentAsync(int count)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = $@"SELECT TOP {count} p.IdPago, p.IdDeuda, p.IdPersona, p.FechaPago, p.MontoPagado, p.Referencia,
                              d.TipoServicio + ' - Puesto ' + pue.NumeroPuesto AS DetallesDeuda,
                              per.Nombres + ' ' + per.Apellidos AS NombrePersona
                         FROM Pago p
                         INNER JOIN Deuda d ON p.IdDeuda = d.IdDeuda
                         INNER JOIN Puesto pue ON d.IdPuesto = pue.IdPuesto
                         INNER JOIN Persona per ON p.IdPersona = per.IdPersona
                         ORDER BY p.IdPago DESC";
            return await connection.QueryAsync<Pago>(sql);
        }
    }
}