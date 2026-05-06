using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using MercadoApp.Models;
using MercadoApp.Repositories.Interfaces;

namespace MercadoApp.Repositories
{
    public class PersonaRepository : IPersonaRepository
    {
        private readonly string _connectionString;

        public PersonaRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MercadoDB") ?? throw new InvalidOperationException("Connection string 'MercadoDB' not found.");
        }

        public IEnumerable<Persona> GetAll()
        {
            var personas = new List<Persona>();
            using (var con = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand("SELECT IdPersona, Nombres, Apellidos, DNI, Telefono FROM Persona", con);
                con.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        personas.Add(new Persona
                        {
                            IdPersona = Convert.ToInt32(reader["IdPersona"]),
                            Nombres = reader["Nombres"].ToString(),
                            Apellidos = reader["Apellidos"].ToString(),
                            DNI = reader["DNI"].ToString(),
                            Telefono = reader["Telefono"].ToString()
                        });
                    }
                }
            }
            return personas;
        }

        public Persona GetById(int id)
        {
            Persona? persona = null;
            using (var con = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand("SELECT IdPersona, Nombres, Apellidos, DNI, Telefono FROM Persona WHERE IdPersona = @Id", con);
                cmd.Parameters.AddWithValue("@Id", id);
                con.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        persona = new Persona
                        {
                            IdPersona = Convert.ToInt32(reader["IdPersona"]),
                            Nombres = reader["Nombres"].ToString(),
                            Apellidos = reader["Apellidos"].ToString(),
                            DNI = reader["DNI"].ToString(),
                            Telefono = reader["Telefono"].ToString()
                        };
                    }
                }
            }
            return persona;
        }

        public void Create(Persona persona)
        {
            using (var con = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand("INSERT INTO Persona (Nombres, Apellidos, DNI, Telefono) VALUES (@Nombres, @Apellidos, @DNI, @Telefono)", con);
                cmd.Parameters.AddWithValue("@Nombres", persona.Nombres);
                cmd.Parameters.AddWithValue("@Apellidos", persona.Apellidos);
                cmd.Parameters.AddWithValue("@DNI", persona.DNI);
                cmd.Parameters.AddWithValue("@Telefono", persona.Telefono ?? (object)DBNull.Value);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Update(Persona persona)
        {
            using (var con = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand("UPDATE Persona SET Nombres = @Nombres, Apellidos = @Apellidos, DNI = @DNI, Telefono = @Telefono WHERE IdPersona = @Id", con);
                cmd.Parameters.AddWithValue("@Id", persona.IdPersona);
                cmd.Parameters.AddWithValue("@Nombres", persona.Nombres);
                cmd.Parameters.AddWithValue("@Apellidos", persona.Apellidos);
                cmd.Parameters.AddWithValue("@DNI", persona.DNI);
                cmd.Parameters.AddWithValue("@Telefono", persona.Telefono ?? (object)DBNull.Value);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Delete(int id)
        {
            using (var con = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand("DELETE FROM Persona WHERE IdPersona = @Id", con);
                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
