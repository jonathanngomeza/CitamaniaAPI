using System.ComponentModel.DataAnnotations;

namespace Citamania.API.Models
{ 
    public class UsuarioLogueado
    {
        public string Email { get; set; }        
        public string Nombre { get; set; }
        public string Token { get; set; } 
        public DateTime FechaDeExpiracion { get; set; }
    }
}
