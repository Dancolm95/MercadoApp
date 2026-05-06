using System;
using System.ComponentModel.DataAnnotations;

namespace MercadoApp.Models
{
    public class Deuda
    {
        public int IdDeuda { get; set; }

        [Required(ErrorMessage = "El puesto es obligatorio")]
        [Display(Name = "Puesto")]
        public int IdPuesto { get; set; }

        [Required(ErrorMessage = "El tipo de servicio es obligatorio")]
        [StringLength(50)]
        [Display(Name = "Tipo de Servicio")]
        public string? TipoServicio { get; set; }

        [Required(ErrorMessage = "El monto es obligatorio")]
        [Range(0.01, 10000, ErrorMessage = "El monto debe ser mayor a 0")]
        public decimal Monto { get; set; }

        [Required]
        [Display(Name = "Fecha Generada")]
        [DataType(DataType.Date)]
        public DateTime FechaGenerada { get; set; }

        public bool Pagada { get; set; }
        
        // Navigation / DTO property for UI
        public string? NumeroPuesto { get; set; }
    }
}
