using System;
using System.ComponentModel.DataAnnotations;

namespace MercadoApp.Models
{
    public class Pago
    {
        public int IdPago { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una deuda")]
        [Display(Name = "Deuda Asociada")]
        public int IdDeuda { get; set; }

        [Required(ErrorMessage = "Debe asociar una persona")]
        [Display(Name = "Persona que paga")]
        public int IdPersona { get; set; }

        [Required]
        [Display(Name = "Fecha de Pago")]
        [DataType(DataType.Date)]
        public DateTime FechaPago { get; set; }

        [Required(ErrorMessage = "El monto pagado es obligatorio")]
        [Range(0.01, 100000, ErrorMessage = "El monto debe ser mayor a 0")]
        [Display(Name = "Monto Pagado")]
        public decimal MontoPagado { get; set; }

        [StringLength(100)]
        [Display(Name = "Referencia de Pago (Ej. N° Operación)")]
        public string? Referencia { get; set; }

        // Navigation properties para la vista
        public string? DetallesDeuda { get; set; }
        public string? NombrePersona { get; set; }
    }
}
