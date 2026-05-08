using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using MercadoApp.Models;
using MercadoApp.Repositories.Interfaces;

namespace MercadoApp.Repositories
{
    public class PagoRepository : IPagoRepository
    {
        private readonly string _connectionString;

        public PagoRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MercadoDB") ?? throw new InvalidOperationException("Connection string 'MercadoDB' not found.");
        }

        public IEnumerable<Pago> GetAll()
        {
            var pagos = new List<Pago>();
            using (var con = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT p.IdPago, p.IdDeuda, p.IdPersona, p.FechaPago, p.MontoPagado, p.Referencia,
                           d.TipoServicio + ' - Puesto ' + pue.NumeroPuesto AS DetallesDeuda,
                           per.Nombres + ' ' + per.Apellidos AS NombrePersona
                    FROM Pago p
                    INNER JOIN Deuda d ON p.IdDeuda = d.IdDeuda
                    INNER JOIN Puesto pue ON d.IdPuesto = pue.IdPuesto
                    INNER JOIN Persona per ON p.IdPersona = per.IdPersona";

                var cmd = new SqlCommand(query, con);
                con.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        pagos.Add(new Pago
                        {
                            IdPago = Convert.ToInt32(reader["IdPago"]),
                            IdDeuda = Convert.ToInt32(reader["IdDeuda"]),
                            IdPersona = Convert.ToInt32(reader["IdPersona"]),
                            FechaPago = Convert.ToDateTime(reader["FechaPago"]),
                            MontoPagado = Convert.ToDecimal(reader["MontoPagado"]),
                            Referencia = reader["Referencia"].ToString(),
                            DetallesDeuda = reader["DetallesDeuda"].ToString(),
                            NombrePersona = reader["NombrePersona"].ToString()
                        });
                    }
                }
            }
            return pagos;
        }

        public void RegistrarPago(Pago pago)
        {
            using (var con = new SqlConnection(_connectionString))
            {
                con.Open();
                using (var transaction = con.BeginTransaction())
                {
                    try
                    {
                        var cmdInsert = new SqlCommand(
                            "INSERT INTO Pago (IdDeuda, IdPersona, FechaPago, MontoPagado, Referencia) " +
                            "VALUES (@IdDeuda, @IdPersona, @FechaPago, @MontoPagado, @Referencia)", con, transaction);
                        
                        cmdInsert.Parameters.AddWithValue("@IdDeuda", pago.IdDeuda);
                        cmdInsert.Parameters.AddWithValue("@IdPersona", pago.IdPersona);
                        cmdInsert.Parameters.AddWithValue("@FechaPago", pago.FechaPago);
                        cmdInsert.Parameters.AddWithValue("@MontoPagado", pago.MontoPagado);
                        cmdInsert.Parameters.AddWithValue("@Referencia", pago.Referencia ?? (object)DBNull.Value);

                        cmdInsert.ExecuteNonQuery();

                        var cmdUpdateDeuda = new SqlCommand(
                            "UPDATE Deuda SET Pagada = 1 WHERE IdDeuda = @IdDeuda", con, transaction);
                        cmdUpdateDeuda.Parameters.AddWithValue("@IdDeuda", pago.IdDeuda);
                        
                        cmdUpdateDeuda.ExecuteNonQuery();

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

        public Pago? GetById(int id)
        {
            using (var con = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT p.IdPago, p.IdDeuda, p.IdPersona, p.FechaPago, p.MontoPagado, p.Referencia,
                           d.TipoServicio + ' - Puesto ' + pue.NumeroPuesto AS DetallesDeuda,
                           per.Nombres + ' ' + per.Apellidos AS NombrePersona
                    FROM Pago p
                    INNER JOIN Deuda d ON p.IdDeuda = d.IdDeuda
                    INNER JOIN Puesto pue ON d.IdPuesto = pue.IdPuesto
                    INNER JOIN Persona per ON p.IdPersona = per.IdPersona
                    WHERE p.IdPago = @Id";

                var cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Id", id);
                con.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Pago
                        {
                            IdPago = Convert.ToInt32(reader["IdPago"]),
                            IdDeuda = Convert.ToInt32(reader["IdDeuda"]),
                            IdPersona = Convert.ToInt32(reader["IdPersona"]),
                            FechaPago = Convert.ToDateTime(reader["FechaPago"]),
                            MontoPagado = Convert.ToDecimal(reader["MontoPagado"]),
                            Referencia = reader["Referencia"].ToString(),
                            DetallesDeuda = reader["DetallesDeuda"].ToString(),
                            NombrePersona = reader["NombrePersona"].ToString()
                        };
                    }
                }
            }
            return null;
        }

        public void Update(Pago pago)
        {
            using (var con = new SqlConnection(_connectionString))
            {
                con.Open();
                using (var transaction = con.BeginTransaction())
                {
                    try
                    {
                        var cmd = new SqlCommand(
                            "UPDATE Pago SET IdDeuda = @IdDeuda, IdPersona = @IdPersona, FechaPago = @FechaPago, MontoPagado = @MontoPagado, Referencia = @Referencia WHERE IdPago = @IdPago", con, transaction);
                        cmd.Parameters.AddWithValue("@IdPago", pago.IdPago);
                        cmd.Parameters.AddWithValue("@IdDeuda", pago.IdDeuda);
                        cmd.Parameters.AddWithValue("@IdPersona", pago.IdPersona);
                        cmd.Parameters.AddWithValue("@FechaPago", pago.FechaPago);
                        cmd.Parameters.AddWithValue("@MontoPagado", pago.MontoPagado);
                        cmd.Parameters.AddWithValue("@Referencia", pago.Referencia ?? (object)DBNull.Value);
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

        public void Delete(int id)
        {
            using (var con = new SqlConnection(_connectionString))
            {
                var query = "DELETE FROM Pago WHERE IdPago = @Id";
                var cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Id", id);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
