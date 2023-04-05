using Citamania.API.Models;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace Citamania.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly ILogger<UsuarioController> _logger;

        public UsuarioController(ILogger<UsuarioController> logger, IConfiguration config)
        {
            _logger = logger;
            _connectionString = config.GetConnectionString("CitamaniaDB");
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            string sql = "SELECT  * FROM Usuarios";
            //string connectionString = "Data Source=(local);Initial Catalog=TallerMecanico;Integrated Security=True;";
            using var connection = new SqlConnection(_connectionString);
            var usuarios = await connection.QueryAsync<Usuario>(sql);
            return Ok(usuarios.ToList());
        }

        [HttpGet]
        [Route("{usuarioId}")]
        public async Task<IActionResult> GetById(int usuarioId)
        {
            string sql = "SELECT  * FROM Usuarios WHERE usuarioId = @UsuarioId";
            //string connectionString = "Data Source=(local);Initial Catalog=TallerMecanico;Integrated Security=True;";
            using var connection = new SqlConnection(_connectionString);
            var usuario = await connection.QueryFirstOrDefaultAsync<Usuario>(sql, new { usuarioId });
            return Ok(usuario);
        }

        [HttpPost]
        public async Task<IActionResult> Add(Usuario usuario)
        {
            // cita con Jonathan, cliente crisitna, pdidio = cirsitna, fecha 2023-02-01 hr12:20pm
            // un inserte INSERT INTO Citas 
            //   ese inserte va a generart un ID decita CitaId
            // servicios
            // For each para recorrer la lista de serivcios
            // 1 limpienza
            // INSERT INTO citadetalles --- el detalle del a citaID
            // 2 sacar una muela
            // INSERT INTO citadetalles --- el detalle del a citaID
            // 3 Revision de corona
            // INSERT INTO citadetalles --- el detalle del a citaID
            string sql = @"INSERT INTO [dbo].[Usuarios]
                            ([Nombre]
                            ,[Email]
                            ,[HasPassword]
                            ,[SaltStrig]
                            ,[TokenDeRecuperacion]
                            ,[PrestaServicio])
                        VALUES
                            (@Nombre
                            ,@Email
                            ,@HasPassword
                            ,@SaltStrig
                            ,@TokenDeRecuperacion
                            ,@PrestaServicio);
                         SELECT @@IDENTITY";
            var parameters = new DynamicParameters();
            parameters.Add("Nombre", usuario.Nombre, DbType.String);
            parameters.Add("Email", usuario.Email, DbType.String);
            parameters.Add("HasPassword", usuario.HasPassword, DbType.String);
            parameters.Add("SaltStrig", usuario.SaltStrig, DbType.String);
            parameters.Add("TokenDeRecuperacion", usuario.TokenDeRecuperacion, DbType.String);
            parameters.Add("PrestaServicio", usuario.PrestaServicio, DbType.Boolean);
            //string connectionString = "Data Source=(local);Initial Catalog=TallerMecanico;Integrated Security=True;";
            using var connection = new SqlConnection(_connectionString);
            var idGenerado = await connection.QueryFirstOrDefaultAsync<int>(sql, param: parameters);

            
            /*Cita ejemplo = new Cita();
            // insert de la cita 
            var idCita = 10;
            foreach(var citaDetalle in ejemplo.Detalle)
            {
                parameters.Add("CodigoUnico", ejemplo.CodigoUnico, DbType.String);
                parameters.Add("FechaDeCita", ejemplo.FechaDeCita, DbType.DateTime);
                parameters.Add("ClienteId", ejemplo.ClienteId, DbType.Int64);
                parameters.Add("PrestadorDeServicioId", ejemplo.PrestadorDeServicioId, DbType.Int64);
                parameters.Add("SolicitanteId", ejemplo.SolicitanteId, DbType.Int64);
                parameters.Add("Estatus", ejemplo.Estatus, DbType.String);
                parameters.Add("UsuarioAprobacionId", ejemplo.UsuarioCancelacionId, DbType.Int64);
                parameters.Add("FechaAprobacion", ejemplo.FechaAprobacion, DbType.DateTime);
                parameters.Add("FechaCancelacion", ejemplo.FechaCancelacion, DbType.DateTime);
                parameters.Add("UsuarioCancelacionId", ejemplo.UsuarioCancelacionId, DbType.Int64);
                parameters.Add("MotivoDeCancelacion", ejemplo.MotivoDeCancelacion, DbType.String);
                parameters.Add("FechaCreacion", ejemplo.FechaCreacion, DbType.DateTime);
                parameters.Add("FechaModificacion", ejemplo.FechaModificacion, DbType.DateTime);
                parameters.Add("Notas", ejemplo.Notas, DbType.String);
            }*/

            //connection.ExecuteAsync(sql, parameters);

            /*Algo("10", "azul", "x", "150.30");
            Algo(precio: "150.30", edad: "10", color:"azul", valor:"x");
            Algo(color: "azul");*/

            return StatusCode(201, idGenerado);
        }

        //public void Algo(string edad = "", string color= "", string valor="", string precio="") { }

        [HttpDelete]
        public async Task<IActionResult> Delete(int usuarioId)
        {
            string sql = @"DELETE FROM Usuarios WHERE usuarioId = @UsuarioId";
            //string connectionString = "Data Source=(local);Initial Catalog=TallerMecanico;Integrated Security=True;";
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(sql, new { usuarioId });
            return Ok("El usuario a sido eliminado");
        }


        [HttpPut]
        public async Task<IActionResult> Put(Usuario usuario)
        {
            string sql = @"UPDATE [dbo].[Usuarios]
                           SET [Nombre] = @Nombre
                              ,[Email] = @Email
                              ,[HasPassword] = @HasPassword
                              ,[SaltStrig] = @SaltStrig
                              ,[TokenDeRecuperacion] = @TokenDeRecuperacion
                              ,[PrestaServicio] = @PrestaServicio
                        WHERE usuarioId = @UsuarioId";
            var parameters = new DynamicParameters();
            parameters.Add("UsuarioId", usuario.UsuarioId, DbType.Int32);
            parameters.Add("Nombre", usuario.Nombre, DbType.String);
            parameters.Add("Email", usuario.Email, DbType.String);
            parameters.Add("HasPassword", usuario.HasPassword, DbType.String);
            parameters.Add("SaltStrig", usuario.SaltStrig, DbType.String);
            parameters.Add("TokenDeRecuperacion", usuario.TokenDeRecuperacion, DbType.String);
            parameters.Add("PrestaServicio", usuario.PrestaServicio, DbType.Boolean);
            //string connectionString = "Data Source=(local);Initial Catalog=TallerMecanico;Integrated Security=True;";
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(sql, param: parameters);
            return Ok("El usuario a sido actualizado");
        }

    }
}