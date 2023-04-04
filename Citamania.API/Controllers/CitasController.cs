using Citamania.API.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace Citamania.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CitasController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly ILogger<CitasController> _logger;

        public CitasController(ILogger<CitasController> logger, IConfiguration config)
        {
            _logger = logger;
            _connectionString = config.GetConnectionString("CitamaniaDB");
        }

        //Servicios/Usuario/{usuarioId}
        // TODO ESte endpoing no funcioan debe ser por Clietne y por Prestador de servicios
        // Cliente/{clienteId}
        // PrestadorDeServicios/{prestadorDeServiocId}
        [HttpGet("Cliente/{clienteId}")]
        public async Task<IActionResult> Get(int clienteId)
        {
            string sql = "SELECT  * FROM Citas WHERE clienteId = @ClienteId";
            //string connectionString = "Data Source=(local);Initial Catalog=TallerMecanico;Integrated Security=True;";
            using var connection = new SqlConnection(_connectionString);
            var citas = await connection.QueryAsync<Cita>(sql, new { clienteId });
            return Ok(citas.ToList());
        }

        [HttpGet]
        [Route("Usuario/{usuarioId}/PorFecha")]
        public async Task<IActionResult> GetByUsuarioPorFecha(int usuarioId, [FromQuery]DateTime fechaInicial, [FromQuery]DateTime fechaFinal)
        {
            string sql = @"SELECT  * FROM Citas
                           WHERE usuarioId = @UsuarioId 
                                 AND FechaDeCita BETWEEN @FechaInicial AND @FechaFinal";
            var parameters = new DynamicParameters();
            parameters.Add("UsuarioId", usuarioId, DbType.Int32);
            parameters.Add("FechaInicial", fechaInicial, DbType.DateTime);
            parameters.Add("FechaFinal", fechaFinal, DbType.DateTime);
            //string connectionString = "Data Source=(local);Initial Catalog=TallerMecanico;Integrated Security=True;";
            using var connection = new SqlConnection(_connectionString);
            var citas = await connection.QueryAsync<Cita>(sql, param: parameters);
            return Ok(citas);
        }
        [HttpGet]
        [Route("Citas/{citaId}")]
        public async Task<IActionResult> GetCitaId(int citaId)
        {
            string sql = "SELECT  * FROM Citas WHERE citaId = @CitaId";
            //string connectionString = "Data Source=(local);Initial Catalog=TallerMecanico;Integrated Security=True;";
            using var connection = new SqlConnection(_connectionString);
            var cita = await connection.QueryFirstOrDefaultAsync<Cita>(sql, new { citaId });
            return Ok(cita);
        }
        [HttpPost]
        public async Task<IActionResult> Add(Cita cita)
        {
            string sql = @"INSERT INTO [dbo].[Citas]
                            (
                             [FechaDeCita]
                            ,[ClienteId]
                            ,[PrestadorDeServicioId]
                            ,[SolicitanteId]
                            ,[Estatus]
                            ,[UsuarioAprobacionId]
                            ,[FechaAprobacion]
                            ,[FechaCancelacion]
                            ,[UsuarioCancelacionId]
                            ,[MotivoDeCancelacion]
                            ,[FechaCreacion]
                            ,[FechaModificacion]
                            ,[Notas])
                        VALUES
                            (
                             @FechaDeCita
                            ,@ClienteId
                            ,@PrestadorDeServicioId
                            ,@SolicitanteId
                            ,@Estatus
                            ,@UsuarioAprobacionId
                            ,@FechaAprobacion
                            ,@FechaCancelacion
                            ,@UsuarioCancelacionId
                            ,@MotivoDeCancelacion
                            ,@FechaCreacion
                            ,@FechaModificacion
                            ,@Notas)
                         SELECT @@IDENTITY";
            var parameters = new DynamicParameters();
            //parameters.Add("CodigoUnico", cita.CodigoUnico, DbType.String);
            parameters.Add("FechaDeCita", cita.FechaDeCita, DbType.DateTime);
            parameters.Add("ClienteId", cita.ClienteId, DbType.Int32);
            parameters.Add("PrestadorDeServicioId", cita.PrestadorDeServicioId, DbType.Int32);
            parameters.Add("SolicitanteId", cita.SolicitanteId, DbType.Int32);
            parameters.Add("Estatus", cita.Estatus, DbType.String);
            parameters.Add("UsuarioAprobacionId", cita.UsuarioAprovacion, DbType.Int32);
            parameters.Add("FechaAprobacion", cita.FechaAprobacion, DbType.DateTime);
            parameters.Add("FechaCancelacion", cita.CodigoUnico, DbType.DateTime);
            parameters.Add("UsuarioCancelacionId", cita.CodigoUnico, DbType.Int32);
            parameters.Add("MotivoDeCancelacion", cita.CodigoUnico, DbType.String);
            parameters.Add("FechaCreacion", cita.CodigoUnico, DbType.DateTime);
            parameters.Add("FechaModificacion", cita.CodigoUnico, DbType.DateTime);
            parameters.Add("Notas", cita.CodigoUnico, DbType.String);
            using var connection = new SqlConnection(_connectionString);
            var idGenerado = await connection.QueryFirstOrDefaultAsync<int>(sql, param: parameters);

            foreach(var detalle in cita.Detalle) 
            {
                sql = @"INSERT INTO[dbo].[CitasDetalles]
                                    ([CitaId]
                                   ,[ServicioId]
                                   ,[Cantidad]
                                   ,[Precio])
                             VALUES
                                   (@CitaId
                                   , @ServicioId
                                   , @Cantidad
                                   , @Precio)";
                parameters = new DynamicParameters();
                parameters.Add("CitaId", idGenerado, DbType.Int32);
                parameters.Add("ServicioId", detalle.ServicioId, DbType.Int32);
                parameters.Add("Cantidad", detalle.Cantidad, DbType.Int32);
                parameters.Add("Precio", detalle.Precio, DbType.Decimal);
                //string connectionString = "Data Source=(local);Initial Catalog=TallerMecanico;Integrated Security=True;";
                await connection.ExecuteAsync(sql, param: parameters);
            }
            return StatusCode(201, idGenerado);
        }

        [HttpDelete("{citaId}")]
        public async Task<IActionResult> Delete(int citaId)
        {
            string sql = @"DELETE FROM Citas WHERE citaId = @CitaId";
            //string connectionString = "Data Source=(local);Initial Catalog=TallerMecanico;Integrated Security=True;";
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(sql, new { citaId });
            return Ok("La cita a sido eliminada");
        }

        //Servicios
        [HttpPatch("{citaId}/Cancelar")]
        public async Task<IActionResult> PatchCancelar(int citaId, string motivoDeCancelacion)
        {
            int usuarioId = 1;
            string sql = @"UPDATE [dbo].[Citas]
                               SET
                                   [Estatus] = 'Cancelada'
                                  ,[FechaCancelacion] = GETDATE()
                                  ,[UsuarioCancelacionId] = @UsuarioCancelacionId
                                  ,[MotivoDeCancelacion] = @MotivoDeCancelacion
                                  ,[FechaModificacion] = GETDATE()
                             WHERE Cita =@CitaId";
            var parameters = new DynamicParameters();
            parameters.Add("CitaId", citaId, DbType.Int32);
            parameters.Add("UsuarioCancelacionId", usuarioId, DbType.Int32);
            parameters.Add("MotivoDeCancelacion", motivoDeCancelacion, DbType.String);
            //string connectionString = "Data Source=(local);Initial Catalog=TallerMecanico;Integrated Security=True;";
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(sql, param: parameters);
            return Ok("La cita a sido cancelada");
        }

        [HttpPatch("{citaId}/Aprobar")]
        public async Task<IActionResult> PatchAprobar(int citaId)
        {
            int usuarioId = 1;
            string sql = @"UPDATE [dbo].[Citas]
                               SET 
                                   [Estatus] = 'Aprobada'
                                  ,[UsuarioAprobacionId] = @UsuarioAprobacionId
                                  ,[FechaAprobacion] = GETDATE()
                                  ,[FechaModificacion] = GETDATE()
                             WHERE Cita =@CitaId";
            var parameters = new DynamicParameters();
            parameters.Add("CitaId", citaId, DbType.Int32);
            parameters.Add("UsuarioAprobacionId", usuarioId, DbType.Int32);
            //string connectionString = "Data Source=(local);Initial Catalog=TallerMecanico;Integrated Security=True;";
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(sql, param: parameters);
            return Ok("La cita a sido aprobada");
        }

        [HttpPut]
        public async Task<IActionResult> Put(Cita cita)
        {
            string sql = @"UPDATE [dbo].[Citas]
                               SET [CodigoUnico] = @CodigoUnico
                                  ,[FechaDeCita] = @FechaDeCita
                                  ,[ClienteId] = @ClienteId
                                  ,[PrestadorDeServicioId] = @PrestadorDeServicioId
                                  ,[SolicitanteId] = @SolicitanteId
                                  ,[Estatus] = @Estatus
                                  ,[UsuarioAprobacionId] = @UsuarioAprobacionId
                                  ,[FechaAprobacion] = @FechaAprobacion
                                  ,[FechaCancelacion] = @FechaCancelacion
                                  ,[UsuarioCancelacionId] = @UsuarioCancelacionId
                                  ,[MotivoDeCancelacion] = @MotivoDeCancelacion
                                  ,[FechaCreacion] = @FechaCreacion
                                  ,[FechaModificacion] = @FechaModificacion
                                  ,[Notas] = @Notas
                             WHERE Cita =@CitaId";
            var parameters = new DynamicParameters();
            parameters.Add("CitaId", cita.CitaId, DbType.Int32);
            parameters.Add("CodigoUnico", cita.CodigoUnico, DbType.String);
            parameters.Add("FechaDeCita", cita.FechaDeCita, DbType.DateTime);
            parameters.Add("ClienteId", cita.ClienteId, DbType.Int32);
            parameters.Add("PrestadorDeServicioId", cita.PrestadorDeServicioId, DbType.Int32);
            parameters.Add("SolicitanteId", cita.SolicitanteId, DbType.Int32);
            parameters.Add("Estatus", cita.Estatus, DbType.String);
            parameters.Add("UsuarioAprobacionId", cita.UsuarioAprovacion, DbType.Int32);
            parameters.Add("FechaAprobacion", cita.FechaAprobacion, DbType.DateTime);
            parameters.Add("FechaCancelacion", cita.FechaCancelacion, DbType.DateTime);
            parameters.Add("UsuarioCancelacionId", cita.UsuarioCancelacionId, DbType.Int32);
            parameters.Add("MotivoDeCancelacion", cita.MotivoDeCancelacion, DbType.String);
            parameters.Add("FechaCreacion", cita.FechaCreacion, DbType.DateTime);
            parameters.Add("FechaModificacion", cita.FechaModificacion, DbType.DateTime);
            parameters.Add("Notas", cita.Notas, DbType.String);
            //string connectionString = "Data Source=(local);Initial Catalog=TallerMecanico;Integrated Security=True;";
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(sql, param: parameters);
            return Ok("La cita a sido actualizada");
        }
    }
}