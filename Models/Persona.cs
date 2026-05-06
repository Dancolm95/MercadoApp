using System.ComponentModel.DataAnnotations;

namespace MercadoApp.Models
{
    public class Persona
    {
        public int IdPersona { get; set; }

        [Required(ErrorMessage = "Los nombres son obligatorios")]
        [StringLength(50)]
        public string? Nombres { get; set; }

        [Required(ErrorMessage = "Los apellidos son obligatorios")]
        [StringLength(50)]
        public string? Apellidos { get; set; }

        [Required(ErrorMessage = "El DNI es obligatorio")]
        [StringLength(8, MinimumLength = 8, ErrorMessage = "El DNI debe tener exactamente 8 caracteres")]
        public string? DNI { get; set; }

        [StringLength(20)]
        [Display(Name = "Teléfono")]
        public string? Telefono { get; set; }
    }
}
