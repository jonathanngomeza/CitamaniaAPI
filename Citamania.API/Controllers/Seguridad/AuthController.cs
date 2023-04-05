using Citamania.API.Models;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Citamania.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly ILogger<AuthController> _logger;
        private readonly IConfiguration _config;

        public AuthController(ILogger<AuthController> logger, IConfiguration config)
        {
            _logger = logger;
            _connectionString = config.GetConnectionString("CitamaniaDB");
            _config = config;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(Login login)
        {
            var salt = GenerarSalt();
            var usuarioLogueado = await IniciarSesion(login);
            if(usuarioLogueado is null)
            {
                return BadRequest("Usuario o contraseña incorrectos");
            }
            else
            {
                return Ok(usuarioLogueado);
            }
        }

        private async Task<UsuarioLogueado?> IniciarSesion(Login login)
        {
            UsuarioLogueado? usuario = null;

            //if(string.IsNullOrEmpty(login.Usuario) || string.IsNullOrEmpty(login.Password))
            //{
            //    return null;
            //}
            var usuarioDb = await ObtenerUsuarioPorEmail(login.Email);
            var nuevoHash = ConvertirAHash(usuarioDb?.SaltStrig ?? "", login.Password);
            var esValido = usuarioDb?.HasPassword == nuevoHash;

            if(usuarioDb != null && esValido)
            {
                var issuer = _config["Jwt:Issuer"];
                var audience = _config["Jwt:Aduience"];
                var key = _config["Jwt:Key"];
                var minExp = _config.GetValue<int?>("Jwt:MinExp") ?? 30;

                var manejardorDeSeguridadToken = new JwtSecurityTokenHandler();
                var keyBytes = System.Text.Encoding.ASCII.GetBytes(key);

                var fechaExpiracion = DateTime.UtcNow.AddMinutes(minExp);

                var claims = new List<Claim>();
                claims.Add(new Claim("UsuarioId", usuarioDb.UsuarioId.ToString()));
                claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
                claims.Add(new Claim(JwtRegisteredClaimNames.Sub, usuarioDb.Email.ToString()));
                claims.Add(new Claim(JwtRegisteredClaimNames.Email, usuarioDb.Email.ToString()));
                claims.Add(new Claim(ClaimTypes.Role, "User"));
                //claims.Add(new Claim(ClaimTypes.Role, "RoleEjemplo"));

                var claimIdentity = new ClaimsIdentity(claims);
                var claveDeSeguridad = new SymmetricSecurityKey(keyBytes);
                var tokenDescirptor = new SecurityTokenDescriptor
                {
                    Subject = claimIdentity,
                    Expires = fechaExpiracion,
                    Audience = audience,
                    Issuer = issuer,
                    SigningCredentials = new SigningCredentials(claveDeSeguridad, SecurityAlgorithms.HmacSha512Signature)
                };
                var token = manejardorDeSeguridadToken.CreateToken(tokenDescirptor);
                var jwtToken = manejardorDeSeguridadToken.WriteToken(token);

                usuario = new UsuarioLogueado
                {
                    Email = usuarioDb.Email,
                    Nombre = usuarioDb.Nombre,
                    Token = jwtToken,
                    FechaDeExpiracion = fechaExpiracion
                };

            }

            return usuario;
        }

        private string ConvertirAHash(string saltStrig, string password)
        {
            byte[] saltByte = Convert.FromBase64String(saltStrig);

            var resultado = KeyDerivation.Pbkdf2(password: password,
                salt: saltByte,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8);

            string hashed = Convert.ToBase64String(resultado);
            return hashed; 
        }

        private string GenerarSalt()
        {
            byte[] salt = new byte[128 / 8];

            using(var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            return Convert.ToBase64String(salt);
        }


        private async Task<Usuario?> ObtenerUsuarioPorEmail(string email)
        {
            string sql = "SELECT * FROM Usuarios WHERE Email = @Email";
            //string connectionString = "Data Source=(local);Initial Catalog=TallerMecanico;Integrated Security=True;";
            using var connection = new SqlConnection(_connectionString);
            var usuario = await connection.QueryFirstOrDefaultAsync<Usuario?>(sql, new { email });
            return usuario;
        }

    }
}