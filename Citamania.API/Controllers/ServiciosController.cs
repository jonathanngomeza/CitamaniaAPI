using Citamania.API.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace Citamania.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ServiciosController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly ILogger<ServiciosController> _logger;

        public ServiciosController(ILogger<ServiciosController> logger, IConfiguration config)
        {
            _logger = logger;
            _connectionString = config.GetConnectionString("CitamaniaDB");
        }

        //Servicios/Usuario/{usuarioId}
        [HttpGet("Usuario/{usuarioId}")]
        public async Task<IActionResult> Get(int usuarioId)
        {
            string sql = "SELECT  * FROM ServiciosPorUsuario WHERE usuarioId = @UsuarioId";
            var parameters = new DynamicParameters();
            parameters.Add("UsuarioId", usuarioId, DbType.Int32);
            //string connectionString = "Data Source=(local);Initial Catalog=TallerMecanico;Integrated Security=True;";
            using var connection = new SqlConnection(_connectionString);
            var servicios = await connection.QueryAsync<ServicioPorUsuario>(sql, param: parameters) ;
            return Ok(servicios.ToList());
        }

        [HttpGet]
        [Route("{servicioId}")]
        public async Task<IActionResult> GetById(int servicioId)
        {
            string sql = "SELECT  * FROM ServiciosPorUsuario WHERE servicioId = @ServicioId";
            //string connectionString = "Data Source=(local);Initial Catalog=TallerMecanico;Integrated Security=True;";
            using var connection = new SqlConnection(_connectionString);
            var servicio = await connection.QueryFirstOrDefaultAsync<ServicioPorUsuario>(sql, new { servicioId });
            return Ok(servicio);
        }

        [HttpPost]
        public async Task<IActionResult> Add(ServicioPorUsuario servicio)
        {
            string sql = @"INSERT INTO [dbo].[ServiciosPorUsuario]
                            ([UsuarioId]
                            ,[Servicio]
                            ,[Descripcion]
                            ,[Precio])
                        VALUES
                            (@UsuarioId
                            ,@Servicio
                            ,@Descripcion
                            ,@Precio);
                         SELECT @@IDENTITY";
            var parameters = new DynamicParameters();
            parameters.Add("UsuarioId", servicio.UsuarioId, DbType.Int32);
            parameters.Add("Servicio", servicio.Servicio, DbType.String);
            parameters.Add("Descripcion", servicio.Descripcion, DbType.String);
            parameters.Add("Precio", servicio.Precio, DbType.Decimal);
            using var connection = new SqlConnection(_connectionString);
            var idGenerado = await connection.QueryFirstOrDefaultAsync<int>(sql, param: parameters);
            return StatusCode(201, idGenerado);
        }

        //public void Algo(string edad = "", string color= "", string valor="", string precio="") { }

        [HttpDelete("{servicioId}")]
        public async Task<IActionResult> Delete(int servicioId)
        {
            string sql = @"DELETE FROM ServiciosPorUsuario WHERE servicioId = @ServicioId";
            //string connectionString = "Data Source=(local);Initial Catalog=TallerMecanico;Integrated Security=True;";
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(sql, new { servicioId });
            return Ok("El servicio a sido eliminado"); ;
        }

        //Servicios
        [HttpPut]
        public async Task<IActionResult> Put(ServicioPorUsuario servicio)
        {
            string sql = @"UPDATE [dbo].[ServiciosPorUsuario]
                               SET [UsuarioId] = @UsuarioId
                                  ,[Servicio] = @Servicio
                                  ,[Descripcion] = @Descripcion
                                  ,[Precio] = @Precio
                             WHERE ServicioId = @ServicioId";
            var parameters = new DynamicParameters();
            parameters.Add("ServicioId", servicio.Servicio, DbType.Int64);
            parameters.Add("UsuarioId", servicio.Servicio, DbType.Int64);
            parameters.Add("Servicio", servicio.Servicio, DbType.String);
            parameters.Add("Email", servicio.Descripcion, DbType.String);
            parameters.Add("HasPassword", servicio.Precio, DbType.Decimal);
            //string connectionString = "Data Source=(local);Initial Catalog=TallerMecanico;Integrated Security=True;";
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(sql, param: parameters);
            return Ok("El servicio a sido actualizado");
        }
    }
}