using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using MercadoApp.Models;
using MercadoApp.Repositories.Interfaces;

namespace MercadoApp.Repositories
{
    public class DeudaRepository : IDeudaRepository
    {
        private readonly string _connectionString;

        public DeudaRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MercadoDB") ?? throw new InvalidOperationException("Connection string 'MercadoDB' not found.");
        }

        public IEnumerable<Deuda> GetAll()
        {
            var deudas = new List<Deuda>();
            using (var con = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT d.IdDeuda, d.IdPuesto, d.TipoServicio, d.Monto, d.FechaGenerada, d.Pagada, p.NumeroPuesto 
                    FROM Deuda d
                    INNER JOIN Puesto p ON d.IdPuesto = p.IdPuesto";

                var cmd = new SqlCommand(query, con);
                con.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        deudas.Add(new Deuda
                        {
                            IdDeuda = Convert.ToInt32(reader["IdDeuda"]),
                            IdPuesto = Convert.ToInt32(reader["IdPuesto"]),
                            TipoServicio = reader["TipoServicio"].ToString(),
                            Monto = Convert.ToDecimal(reader["Monto"]),
                            FechaGenerada = Convert.ToDateTime(reader["FechaGenerada"]),
                            Pagada = Convert.ToBoolean(reader["Pagada"]),
                            NumeroPuesto = reader["NumeroPuesto"].ToString()
                        });
                    }
                }
            }
            return deudas;
        }

        public IEnumerable<Deuda> GetByPuesto(int idPuesto)
        {
            var deudas = new List<Deuda>();
            using (var con = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT d.IdDeuda, d.IdPuesto, d.TipoServicio, d.Monto, d.FechaGenerada, d.Pagada, p.NumeroPuesto 
                    FROM Deuda d
                    INNER JOIN Puesto p ON d.IdPuesto = p.IdPuesto
                    WHERE d.IdPuesto = @IdPuesto";

                var cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@IdPuesto", idPuesto);

                con.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        deudas.Add(new Deuda
                        {
                            IdDeuda = Convert.ToInt32(reader["IdDeuda"]),
                            IdPuesto = Convert.ToInt32(reader["IdPuesto"]),
                            TipoServicio = reader["TipoServicio"].ToString(),
                            Monto = Convert.ToDecimal(reader["Monto"]),
                            FechaGenerada = Convert.ToDateTime(reader["FechaGenerada"]),
                            Pagada = Convert.ToBoolean(reader["Pagada"]),
                            NumeroPuesto = reader["NumeroPuesto"].ToString()
                        });
                    }
                }
            }
            return deudas;
        }

        public void RegistrarDeuda(Deuda deuda)
        {
            using (var con = new SqlConnection(_connectionString))
            {
                con.Open();
                using (var transaction = con.BeginTransaction())
                {
                    try
                    {
                        var cmd = new SqlCommand("INSERT INTO Deuda (IdPuesto, TipoServicio, Monto, FechaGenerada, Pagada) VALUES (@IdPuesto, @TipoServicio, @Monto, @FechaGenerada, @Pagada)", con, transaction);
                        cmd.Parameters.AddWithValue("@IdPuesto", deuda.IdPuesto);
                        cmd.Parameters.AddWithValue("@TipoServicio", deuda.TipoServicio);
                        cmd.Parameters.AddWithValue("@Monto", deuda.Monto);
                        cmd.Parameters.AddWithValue("@FechaGenerada", deuda.FechaGenerada);
                        cmd.Parameters.AddWithValue("@Pagada", deuda.Pagada);

                        cmd.ExecuteNonQuery();

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}
