using System.ComponentModel.DataAnnotations;

namespace Citamania.API.Models
{
    public class CitaDetalle
    {
        [Key]
        [Required]
        public int CitaId { get; set; }

        [Key]
        [Required]
        public int ServicioId { get; set; }

        [Required]
        public int Cantidad { get; set; }

        [Required]
        public decimal Precio { get; set; }

        public Cita Cita { get; set; }
        public ServicioPorUsuario Servicio { get; set; }
    }
}
