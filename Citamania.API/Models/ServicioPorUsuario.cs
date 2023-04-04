using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Citamania.API.Models
{
    public class ServicioPorUsuario
    {
        [Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ServicioId { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Servicio { get; set; }

        [MaxLength(400)]
        public string Descripcion { get; set; }

        [Required]
        public decimal Precio { get; set; }

        public Usuario? Usuario { get; set; }
    }
}
