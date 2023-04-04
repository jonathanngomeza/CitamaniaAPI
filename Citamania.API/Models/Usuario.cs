using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Citamania.API.Models
{ 
    public class Usuario
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UsuarioId { get; set; }

        [MaxLength(250)]
        [Required]
        public string Nombre { get; set; }

        [MaxLength(250)]
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [MaxLength(400)]
        [Required]
        public string HasPassword { get; set; }

        [MaxLength(400)]
        [Required]
        public string SaltStrig { get; set; }

        [MaxLength(100)]
        public string? TokenDeRecuperacion { get; set; }

        [Required]
        public bool PrestaServicio { get; set; }

    }
}
