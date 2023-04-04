using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Citamania.API.Models
{
    public class Cita
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CitaId { get; set; }

        [Required]
        public Guid CodigoUnico { get; set; }

        [Required]
        public DateTime FechaDeCita { get; set; }

        [Required]
        public int ClienteId { get; set; }

        [Required]
        public int PrestadorDeServicioId { get; set; }

        [Required]
        public int SolicitanteId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Estatus { get; set; }
        public int? UsuarioAprovacion { get; set; }
        public DateTime? FechaAprobacion { get; set; }
        public DateTime? FechaCancelacion { get; set; }
        public int? UsuarioCancelacionId { get; set; }
        public string? MotivoDeCancelacion { get; set; }

        [Required]
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }

        [MaxLength(500)]
        public string? Notas { get; set; }

        public Usuario Cliente { get; set; }
        public Usuario PrestadorDeServicio { get; set; }
        public Usuario SolicitadaPor { get; set; }
        public Usuario? AprovadaPor { get; set; }
        public Usuario? CanceladaPor { get; set; }
        [Required]
        public List<CitaDetalle> Detalle { get; set; } //este va a ser el campo que me ayude con el oreach para saber cuantas veces tiene eque entrar en el insert
    }
}
