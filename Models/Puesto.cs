using System.ComponentModel.DataAnnotations;

namespace MercadoApp.Models
{
    public class Puesto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El número de puesto es obligatorio")]
        [StringLength(20)]
        [Display(Name = "N° Puesto")]
        public string NumeroPuesto { get; set; }

        [Required(ErrorMessage = "La sección es obligatoria")]
        [StringLength(5)]
        [Display(Name = "Sección")]
        public string Seccion { get; set; }

        [Required(ErrorMessage = "El área es obligatoria")]
        [Range(1, 1000, ErrorMessage = "El área debe estar entre 1 y 1000 M2")]
        [Display(Name = "Área M2")]
        public decimal AreaM2 { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio")]
        [StringLength(20)]
        public string Estado { get; set; }

        [Required(ErrorMessage = "El monto es obligatorio")]
        [Range(0, 100000, ErrorMessage = "El monto debe ser válido")]
        [Display(Name = "Monto Mensual")]
        public decimal MontoMensual { get; set; }

        [Display(Name = "Dueño")]
        public int? IdPersona { get; set; }
    }
}
