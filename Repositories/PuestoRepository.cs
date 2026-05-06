using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using MercadoApp.Models;
using MercadoApp.Repositories.Interfaces;

namespace MercadoApp.Repositories
{
    public class PuestoRepository : IPuestoRepository
    {
        private readonly string _connectionString;

        public PuestoRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MercadoDB") ?? throw new InvalidOperationException("Connection string 'MercadoDB' not found.");
        }

        public IEnumerable<Puesto> GetAll()
        {
            var puestos = new List<Puesto>();
            using (var con = new SqlConnection(_connectionString))
            {
                // La tabla en el script se llama Puesto y su clave es IdPuesto según el README (IdPuesto, NumeroPuesto, ...).
                // Pero el plan dice: Id, NumeroPuesto... o IdPuesto. Usaremos IdPuesto como columna si la tabla sigue el plan "IdPuesto". 
                // Wait, el plan dice "Puesto — IdPuesto, NumeroPuesto, Seccion, AreaM2, Estado, MontoMensual, IdPersona (FK)".
                // Mi modelo tiene Id. Vamos a mappear IdPuesto a Id.
                var cmd = new SqlCommand("SELECT IdPuesto, NumeroPuesto, Seccion, AreaM2, Estado, MontoMensual, IdPersona FROM Puesto", con);
                con.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        puestos.Add(new Puesto
                        {
                            Id = Convert.ToInt32(reader["IdPuesto"]),
                            NumeroPuesto = reader["NumeroPuesto"].ToString(),
                            Seccion = reader["Seccion"].ToString(),
                            AreaM2 = Convert.ToDecimal(reader["AreaM2"]),
                            Estado = reader["Estado"].ToString(),
                            MontoMensual = Convert.ToDecimal(reader["MontoMensual"]),
                            IdPersona = reader["IdPersona"] != DBNull.Value ? Convert.ToInt32(reader["IdPersona"]) : (int?)null
                        });
                    }
                }
            }
            return puestos;
        }

        public Puesto GetById(int id)
        {
            Puesto? puesto = null;
            using (var con = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand("SELECT IdPuesto, NumeroPuesto, Seccion, AreaM2, Estado, MontoMensual, IdPersona FROM Puesto WHERE IdPuesto = @Id", con);
                cmd.Parameters.AddWithValue("@Id", id);
                con.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        puesto = new Puesto
                        {
                            Id = Convert.ToInt32(reader["IdPuesto"]),
                            NumeroPuesto = reader["NumeroPuesto"].ToString(),
                            Seccion = reader["Seccion"].ToString(),
                            AreaM2 = Convert.ToDecimal(reader["AreaM2"]),
                            Estado = reader["Estado"].ToString(),
                            MontoMensual = Convert.ToDecimal(reader["MontoMensual"]),
                            IdPersona = reader["IdPersona"] != DBNull.Value ? Convert.ToInt32(reader["IdPersona"]) : (int?)null
                        };
                    }
                }
            }
            return puesto;
        }

        public void Create(Puesto puesto)
        {
            using (var con = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand("INSERT INTO Puesto (NumeroPuesto, Seccion, AreaM2, Estado, MontoMensual, IdPersona) VALUES (@NumeroPuesto, @Seccion, @AreaM2, @Estado, @MontoMensual, @IdPersona)", con);
                cmd.Parameters.AddWithValue("@NumeroPuesto", puesto.NumeroPuesto);
                cmd.Parameters.AddWithValue("@Seccion", puesto.Seccion);
                cmd.Parameters.AddWithValue("@AreaM2", puesto.AreaM2);
                cmd.Parameters.AddWithValue("@Estado", puesto.Estado);
                cmd.Parameters.AddWithValue("@MontoMensual", puesto.MontoMensual);
                cmd.Parameters.AddWithValue("@IdPersona", puesto.IdPersona ?? (object)DBNull.Value);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Update(Puesto puesto)
        {
            using (var con = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand("UPDATE Puesto SET NumeroPuesto = @NumeroPuesto, Seccion = @Seccion, AreaM2 = @AreaM2, Estado = @Estado, MontoMensual = @MontoMensual, IdPersona = @IdPersona WHERE IdPuesto = @Id", con);
                cmd.Parameters.AddWithValue("@Id", puesto.Id);
                cmd.Parameters.AddWithValue("@NumeroPuesto", puesto.NumeroPuesto);
                cmd.Parameters.AddWithValue("@Seccion", puesto.Seccion);
                cmd.Parameters.AddWithValue("@AreaM2", puesto.AreaM2);
                cmd.Parameters.AddWithValue("@Estado", puesto.Estado);
                cmd.Parameters.AddWithValue("@MontoMensual", puesto.MontoMensual);
                cmd.Parameters.AddWithValue("@IdPersona", puesto.IdPersona ?? (object)DBNull.Value);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Delete(int id)
        {
            using (var con = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand("DELETE FROM Puesto WHERE IdPuesto = @Id", con);
                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
