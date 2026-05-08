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

        public Deuda? GetById(int id)
        {
            using (var con = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT d.IdDeuda, d.IdPuesto, d.TipoServicio, d.Monto, d.FechaGenerada, d.Pagada, p.NumeroPuesto 
                    FROM Deuda d
                    INNER JOIN Puesto p ON d.IdPuesto = p.IdPuesto
                    WHERE d.IdDeuda = @Id";

                var cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Id", id);
                con.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Deuda
                        {
                            IdDeuda = Convert.ToInt32(reader["IdDeuda"]),
                            IdPuesto = Convert.ToInt32(reader["IdPuesto"]),
                            TipoServicio = reader["TipoServicio"].ToString(),
                            Monto = Convert.ToDecimal(reader["Monto"]),
                            FechaGenerada = Convert.ToDateTime(reader["FechaGenerada"]),
                            Pagada = Convert.ToBoolean(reader["Pagada"]),
                            NumeroPuesto = reader["NumeroPuesto"].ToString()
                        };
                    }
                }
            }
            return null;
        }

        public void Update(Deuda deuda)
        {
            using (var con = new SqlConnection(_connectionString))
            {
                var query = "UPDATE Deuda SET IdPuesto = @IdPuesto, TipoServicio = @TipoServicio, Monto = @Monto, FechaGenerada = @FechaGenerada, Pagada = @Pagada WHERE IdDeuda = @IdDeuda";
                var cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@IdDeuda", deuda.IdDeuda);
                cmd.Parameters.AddWithValue("@IdPuesto", deuda.IdPuesto);
                cmd.Parameters.AddWithValue("@TipoServicio", deuda.TipoServicio);
                cmd.Parameters.AddWithValue("@Monto", deuda.Monto);
                cmd.Parameters.AddWithValue("@FechaGenerada", deuda.FechaGenerada);
                cmd.Parameters.AddWithValue("@Pagada", deuda.Pagada);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Delete(int id)
        {
            using (var con = new SqlConnection(_connectionString))
            {
                var query = "DELETE FROM Deuda WHERE IdDeuda = @Id";
                var cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Id", id);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public int GetCountByEstado(bool pagada)
        {
            using (var con = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand("SELECT COUNT(*) FROM Deuda WHERE Pagada = @Pagada", con);
                cmd.Parameters.AddWithValue("@Pagada", pagada);
                con.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        public decimal GetTotalMonto(bool pagada)
        {
            using (var con = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand("SELECT ISNULL(SUM(Monto), 0) FROM Deuda WHERE Pagada = @Pagada", con);
                cmd.Parameters.AddWithValue("@Pagada", pagada);
                con.Open();
                return Convert.ToDecimal(cmd.ExecuteScalar());
            }
        }

        public IEnumerable<Deuda> GetByFecha(DateTime fechaInicio, DateTime fechaFin)
        {
            var deudas = new List<Deuda>();
            using (var con = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT d.IdDeuda, d.IdPuesto, d.TipoServicio, d.Monto, d.FechaGenerada, d.Pagada, p.NumeroPuesto 
                    FROM Deuda d
                    INNER JOIN Puesto p ON d.IdPuesto = p.IdPuesto
                    WHERE d.FechaGenerada BETWEEN @FechaInicio AND @FechaFin
                    ORDER BY d.FechaGenerada DESC";

                var cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio);
                cmd.Parameters.AddWithValue("@FechaFin", fechaFin);
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

        public IEnumerable<Deuda> GetRecent(int count)
        {
            var deudas = new List<Deuda>();
            using (var con = new SqlConnection(_connectionString))
            {
                var query = $@"
                    SELECT TOP {count} d.IdDeuda, d.IdPuesto, d.TipoServicio, d.Monto, d.FechaGenerada, d.Pagada, p.NumeroPuesto 
                    FROM Deuda d
                    INNER JOIN Puesto p ON d.IdPuesto = p.IdPuesto
                    ORDER BY d.IdDeuda DESC";

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
    }
}
