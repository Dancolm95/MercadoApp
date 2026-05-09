using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using MercadoApp.Data;
using MercadoApp.Models;
using MercadoApp.Repositories.Interfaces;

namespace MercadoApp.Repositories
{
    public class PersonaRepository : IPersonaRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public PersonaRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Persona>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryAsync<Persona>(
                "SELECT IdPersona, Nombres, Apellidos, DNI, Telefono FROM Persona");
        }

        public async Task<Persona?> GetByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<Persona>(
                "SELECT IdPersona, Nombres, Apellidos, DNI, Telefono FROM Persona WHERE IdPersona = @Id",
                new { Id = id });
        }

        public async Task<int> CreateAsync(Persona persona)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"INSERT INTO Persona (Nombres, Apellidos, DNI, Telefono) 
                        VALUES (@Nombres, @Apellidos, @DNI, @Telefono);
                        SELECT CAST(SCOPE_IDENTITY() AS INT);";
            return await connection.ExecuteScalarAsync<int>(sql, persona);
        }

        public async Task UpdateAsync(Persona persona)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"UPDATE Persona SET Nombres = @Nombres, Apellidos = @Apellidos, DNI = @DNI, Telefono = @Telefono 
                        WHERE IdPersona = @IdPersona";
            await connection.ExecuteAsync(sql, persona);
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.ExecuteAsync("DELETE FROM Persona WHERE IdPersona = @Id", new { Id = id });
        }
    }
}