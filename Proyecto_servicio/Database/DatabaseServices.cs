using Microsoft.Data.SqlClient;
using Proyecto_servicio.Helpers;
using Proyecto_servicio.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
namespace Proyecto_servicio.DataBase
{
    public abstract class ConnectionToSQL
    {
        private readonly string connectionString =
           "Server=DESKTOP-N3GOVNS\\KCU_PRUEBA;Database=Moneki;Trusted_Connection=True;TrustServerCertificate=True;";
        //escritorio DESKTOP-38IFLSE\KCSQL
        //laptop DESKTOP-N3GOVNS\\KCU_PRUEBA

        protected SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }

    public class DatabaseService : ConnectionToSQL
    {
        /* ===================== USUARIOS ===================== */
        public async Task RegisterUserAsync(
            string nombre,
            string apellidoPaterno,
            string apellidoMaterno,
            string email,
            string password,
            string telefono,
            string direccion,
            DateTime fechaNacimiento,
            DateTime fechaRegistro,
            double latitud,
            double longitud
        )
        {
            string query = @"
        INSERT INTO Usuarios
        (
            Nombre,
            ApellidoPaterno,
            ApellidoMaterno,
            Email,
            PasswordHash,
            Telefono,
            Direccion,
            FechaNacimiento,
            FechaRegistro,
            Latitud,
            Longitud
        )
        VALUES
        (
            @Nombre,
            @ApellidoPaterno,
            @ApellidoMaterno,
            @Email,
            @PasswordHash,
            @Telefono,
            @Direccion,
            @FechaNacimiento,
            @FechaRegistro,
            @Latitud,
            @Longitud
        )";

            using (SqlConnection con = GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@Nombre", nombre);
                cmd.Parameters.AddWithValue("@ApellidoPaterno", apellidoPaterno);
                cmd.Parameters.AddWithValue("@ApellidoMaterno", apellidoMaterno);
                cmd.Parameters.AddWithValue("@Email", email);

                // 🔐 IMPORTANTE: aquí luego puedes meter hashing
                cmd.Parameters.AddWithValue("@PasswordHash", password);

                cmd.Parameters.AddWithValue("@Telefono", telefono ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Direccion", direccion ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@FechaNacimiento", fechaNacimiento);
                cmd.Parameters.AddWithValue("@FechaRegistro", fechaRegistro);
                cmd.Parameters.AddWithValue("@Latitud", latitud);
                cmd.Parameters.AddWithValue("@Longitud", longitud);

                await con.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }


        public async Task<bool> UserExistsAsync(string email)
        {
            const string query = "SELECT COUNT(*) FROM Usuarios WHERE Email = @email";

            using var con = GetConnection();
            using var cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@email", email);

            await con.OpenAsync();
            return (int)await cmd.ExecuteScalarAsync() > 0;
        }

        public async Task RegistrarUsuarioAsync(
            string nombre,
            string apellidoP,
            string apellidoM,
            string email,
            string passwordHash,
            string direccion,
            string telefono,
            DateTime fechaNacimiento)
        {
            const string query = @"
                INSERT INTO Usuarios
                (Nombre, ApellidoPaterno, ApellidoMaterno, Email, PasswordHash,
                 Direccion, Telefono, FechaNacimiento)
                VALUES
                (@Nombre, @ApP, @ApM, @Email, @PasswordHash,
                 @Direccion, @Telefono, @FechaNacimiento)";

            using var con = GetConnection();
            using var cmd = new SqlCommand(query, con);

            cmd.Parameters.AddWithValue("@Nombre", nombre);
            cmd.Parameters.AddWithValue("@ApP", apellidoP);
            cmd.Parameters.AddWithValue("@ApM", apellidoM);
            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
            cmd.Parameters.AddWithValue("@Direccion", direccion);
            cmd.Parameters.AddWithValue("@Telefono", telefono);
            cmd.Parameters.AddWithValue("@FechaNacimiento", fechaNacimiento);

            await con.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<Dictionary<string, object>> LoginUsuarioEmailAsync(string email, string passwordHash)
        {
            const string query = @"
                SELECT * FROM Usuarios
                WHERE Email = @Email AND PasswordHash = @PasswordHash";

            return await EjecutarLoginAsync(query,
                new SqlParameter("@Email", email),
                new SqlParameter("@PasswordHash", passwordHash));
        }
        public async Task<bool> CorreoExiteUsuariosAsync(string correo)
        {
            if (string.IsNullOrWhiteSpace(correo))
                return false;

            correo = correo.Trim().ToLower();

            string query = @"
        SELECT COUNT(*)
        FROM Usuarios
        WHERE LOWER(LTRIM(RTRIM(Email))) = @correo
    ";

            using SqlConnection con = GetConnection();
            using SqlCommand cmd = new SqlCommand(query, con);

            cmd.Parameters.AddWithValue("@correo", correo);

            await con.OpenAsync();
            int count = Convert.ToInt32(await cmd.ExecuteScalarAsync());

            return count > 0;
        }


        /* ===================== TRABAJADORES ===================== */

        public async Task<Dictionary<string, object>> LoginTrabajadorEmailAsync(string email, string password)
        {
            string query = @"
                SELECT * FROM Trabajadores
                WHERE Email = @e AND PasswordHash = @p";

            return await EjecutarLoginAsync(query,
                new SqlParameter("@e", email),
                new SqlParameter("@p", password));
        }

        /* ===================== ADMINISTRADORES ===================== */

        public async Task<Dictionary<string, object>?> LoginAdminEmailAsync(string email, string password)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            string query = @"
        SELECT 
            ID_Administrador,  -- 👈 ESTE ES EL ID REAL
            Email
        FROM Administradores
        WHERE Email = @email AND PasswordHash = @pass";

            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@email", email);
            cmd.Parameters.AddWithValue("@pass", password);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var admin = new Dictionary<string, object>();

                admin["ID_Administrador"] = reader["ID_Administrador"];
                admin["Email"] = reader["Email"];

                return admin;
            }

            return null;
        }


        /* ===================== RECUPERACIÓN PASSWORD ===================== */
        public async Task<int> ActualizarPasswordPorCorreoAsync(string correo, string nuevaPassword)
        {
            string query = @"
        UPDATE Usuarios
        SET PasswordHash = @pw
        WHERE Email = @correo
    ";

            using SqlConnection con = GetConnection();
            using SqlCommand cmd = new SqlCommand(query, con);

            cmd.Parameters.AddWithValue("@pw", nuevaPassword);
            cmd.Parameters.AddWithValue("@correo", correo);

            await con.OpenAsync();
            return await cmd.ExecuteNonQueryAsync();
        }

        public async Task GuardarCodigoRecuperacionAsync(string correo, string codigo)
        {
            const string query = @"
                INSERT INTO RecuperacionPassword (Correo, Codigo)
                VALUES (@Correo, @Codigo)";

            await ExecuteAsync(query,
                new SqlParameter("@Correo", correo),
                new SqlParameter("@Codigo", codigo));
        }

        public async Task<bool> ValidarCodigoAsync(string correo, string codigo)
        {
            const string query = @"
                SELECT COUNT(*) FROM RecuperacionPassword
                WHERE Correo = @Correo AND Codigo = @Codigo AND Usado = 0";

            using var con = GetConnection();
            using var cmd = new SqlCommand(query, con);

            cmd.Parameters.AddWithValue("@Correo", correo);
            cmd.Parameters.AddWithValue("@Codigo", codigo);

            await con.OpenAsync();
            return (int)await cmd.ExecuteScalarAsync() > 0;
        }

        public async Task MarcarCodigoUsadoAsync(string correo, string codigo)
        {
            const string query = @"
                UPDATE RecuperacionPassword
                SET Usado = 1
                WHERE Correo = @Correo AND Codigo = @Codigo";

            await ExecuteAsync(query,
                new SqlParameter("@Correo", correo),
                new SqlParameter("@Codigo", codigo));
        }

        public async Task ActualizarPasswordUsuarioAsync(string correo, string nuevoPasswordHash)
        {
            const string query = @"
                UPDATE Usuarios
                SET PasswordHash = @PasswordHash
                WHERE Email = @Email";

            await ExecuteAsync(query,
                new SqlParameter("@PasswordHash", nuevoPasswordHash),
                new SqlParameter("@Email", correo));
        }

        /* ===================== HELPERS ===================== */

        private async Task<Dictionary<string, object>> EjecutarLoginAsync(
            string query, params SqlParameter[] parameters)
        {
            using var con = GetConnection();
            using var cmd = new SqlCommand(query, con);
            cmd.Parameters.AddRange(parameters);

            await con.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            var result = new Dictionary<string, object>();
            for (int i = 0; i < reader.FieldCount; i++)
                result[reader.GetName(i)] = reader.GetValue(i);

            return result;
        }

        public async Task<int> ExecuteAsync(string query, params SqlParameter[] parameters)
        {
            using (SqlConnection con = GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddRange(parameters);
                await con.OpenAsync();
                return await cmd.ExecuteNonQueryAsync();
            }
        }
        public async Task<List<Dictionary<string, object>>> ObtenerModulosINEAsync()
        {
            string query = @"
        SELECT IdModulo, Nombre, Direccion, Latitud, Longitud
        FROM ModulosAtencion
        WHERE TipoModulo = 'INE'";

            var lista = new List<Dictionary<string, object>>();

            using SqlConnection con = GetConnection();
            using SqlCommand cmd = new SqlCommand(query, con);

            await con.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var item = new Dictionary<string, object>
                {
                    ["IdModulo"] = reader.GetInt32(0),
                    ["Nombre"] = reader.GetString(1),
                    ["Direccion"] = reader.GetString(2),
                    ["Latitud"] = reader.GetDouble(3),
                    ["Longitud"] = reader.GetDouble(4),
                    ["DistanciaKm"] = 0.0 // se calcula después
                };

                lista.Add(item);
            }

            return lista;
        }
        // ===================== Tramites =====================

        public async Task<INECompleto> ObtenerINECompleto(int idTramite)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            string sql = @"
SELECT 
    t.ID_Tramite,
    i.CURP,
    t.Estado,
    t.FechaCreacion,

    i.ActaNacimiento,
    i.ComprobanteDomicilio,
    i.Identificacion

FROM Tramites t
INNER JOIN TramiteINE i ON i.ID_Tramite = t.ID_Tramite
WHERE t.ID_Tramite = @id";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", idTramite);

            using var rd = await cmd.ExecuteReaderAsync();

            if (!rd.Read())
                return null;

            return new INECompleto
            {
                IdTramite = rd.GetInt32(0),
                CURP = rd.IsDBNull(1) ? "" : rd.GetString(1),
                Estado = rd.IsDBNull(2) ? "" : rd.GetString(2),
                Fecha = rd.GetDateTime(3),

                ActaNacimiento = rd.IsDBNull(4) ? null : (byte[])rd[4],
                ComprobanteDomicilio = rd.IsDBNull(5) ? null : (byte[])rd[5],
                Identificacion = rd.IsDBNull(6) ? null : (byte[])rd[6]
            };
        }

        string DetectarTipo(byte[] bytes)
        {
            if (bytes.Length > 4 && bytes[0] == 0x25 && bytes[1] == 0x50)
                return "pdf";
            if (bytes[0] == 0xFF && bytes[1] == 0xD8)
                return "jpg";
            if (bytes[0] == 0x89 && bytes[1] == 0x50)
                return "png";

            return "bin"; // desconocido
        }
        

        public async Task<List<TramiteINEItem>> ObtenerMisTramitesINEAsync(int idUsuario)
        {
            List<TramiteINEItem> lista = new();

            using var conn = GetConnection();
            await conn.OpenAsync();

            var cmd = new SqlCommand(@"
        SELECT 
            t.ID_Tramite,
            i.CURP,
            t.Estado,
            t.FechaCreacion
        FROM Tramites t
        INNER JOIN TramiteINE i ON i.ID_Tramite = t.ID_Tramite
        WHERE t.ID_Usuario = @id", conn);

            cmd.Parameters.AddWithValue("@id", idUsuario);

            using var rd = await cmd.ExecuteReaderAsync();
            while (await rd.ReadAsync())
            {
                lista.Add(new TramiteINEItem
                {
                    IdTramite = rd.GetInt32(0),
                    CURP = rd.GetString(1),
                    Estado = rd.GetString(2),
                    FechaCreacion = rd.GetDateTime(3)
                });
            }

            return lista;
        }


        public async Task CrearTramiteINEAsync(TramiteINEModel model)
        {
            if (!UserSession.IsLoggedIn)
                throw new Exception("Sesión no válida");

            using SqlConnection conn = GetConnection();
            await conn.OpenAsync();

            using SqlTransaction transaction = conn.BeginTransaction();

            try
            {
                // 1️⃣ Crear trámite general
                string insertTramite = @"
                INSERT INTO Tramites (ID_Usuario, TipoTramite)
                OUTPUT INSERTED.ID_Tramite
                VALUES (@ID_Usuario, 'INE')";

                int idTramite;

                using (SqlCommand cmd = new SqlCommand(insertTramite, conn, transaction))
                {
                    cmd.Parameters.Add("@ID_Usuario", SqlDbType.Int)
                       .Value = UserSession.IdUsuario;

                    idTramite = (int)await cmd.ExecuteScalarAsync();
                }

                // 2️⃣ Crear registro INE
                string insertINE = @"
                INSERT INTO TramiteINE
                (ID_Tramite, CURP, ActaNacimiento, ComprobanteDomicilio, Identificacion)
                VALUES
                (@ID_Tramite, @CURP, @Acta, @Comprobante, @Identificacion)";

                using (SqlCommand cmd = new SqlCommand(insertINE, conn, transaction))
                {
                    cmd.Parameters.Add("@ID_Tramite", SqlDbType.Int).Value = idTramite;
                    cmd.Parameters.Add("@CURP", SqlDbType.VarChar, 18).Value = model.CURP;

                    cmd.Parameters.Add("@Acta", SqlDbType.VarBinary)
                       .Value = model.ActaNacimiento;

                    cmd.Parameters.Add("@Comprobante", SqlDbType.VarBinary)
                       .Value = model.ComprobanteDomicilio;

                    cmd.Parameters.Add("@Identificacion", SqlDbType.VarBinary)
                       .Value = model.Identificacion;

                    await cmd.ExecuteNonQueryAsync();
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }


        public async Task<List<TramiteModel>> GetTramitesUsuarioAsync()
        {
            var lista = new List<TramiteModel>();

            using var conn = GetConnection();
            await conn.OpenAsync();

            string query = @"
        SELECT 
            ID_Tramite,
            TipoTramite,
            Estado,
            FechaCreacion
        FROM Tramites
        WHERE ID_Usuario = @id
        ORDER BY FechaCreacion DESC";

            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", UserSession.IdUsuario);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(new TramiteModel
                {
                    ID_Tramite = reader.GetInt32(0),
                    TipoTramite = reader.GetString(1),
                    Estado = reader.GetString(2),
                    FechaCreacion = reader.GetDateTime(3)
                });
            }

            return lista;
        }


        public async Task<List<UsuarioItem>> ObtenerUsuariosAsync()
        {
            var lista = new List<UsuarioItem>();

            using var con = GetConnection();
            string q = @"SELECT ID_Usuario,
                        Nombre + ' ' + ApellidoPaterno AS NombreCompleto,
                        Email
                 FROM Usuarios";

            using var cmd = new SqlCommand(q, con);
            await con.OpenAsync();
            using var rd = await cmd.ExecuteReaderAsync();

            while (await rd.ReadAsync())
            {
                lista.Add(new UsuarioItem
                {
                    ID_Usuario = rd.GetInt32(0),
                    NombreCompleto = rd.GetString(1),
                    Email = rd.GetString(2)
                });
            }

            return lista;
        }
        public async Task CrearTramiteCompraventaAsync(
            string tipoBien,
            string vendedor,
            string comprador,
            decimal monto,
            byte[] contrato,
            byte[] idVendedor,
            byte[] idComprador)
        {
            using SqlConnection conn = GetConnection();
            await conn.OpenAsync();

            SqlTransaction tx = conn.BeginTransaction();

            try
            {
                // 1️⃣ Tramite general
                string qTramite = @"
                    INSERT INTO Tramites (ID_Usuario, TipoTramite)
                    OUTPUT INSERTED.ID_Tramite
                    VALUES (@ID_Usuario, 'COMPRAVENTA')";

                int idTramite;

                using (SqlCommand cmd = new SqlCommand(qTramite, conn, tx))
                {
                    cmd.Parameters.Add("@ID_Usuario", SqlDbType.Int)
                        .Value = UserSession.IdUsuario;

                    idTramite = (int)await cmd.ExecuteScalarAsync();
                }

                // 2️⃣ Compraventa
                string qCompra = @"
                    INSERT INTO TramiteCompraventa
                    (ID_Tramite, TipoBien, Vendedor, Comprador, Monto,
                     Contrato, IdentificacionVendedor, IdentificacionComprador)
                    VALUES
                    (@ID_Tramite, @TipoBien, @Vendedor, @Comprador, @Monto,
                     @Contrato, @IdVendedor, @IdComprador)";

                using (SqlCommand cmd = new SqlCommand(qCompra, conn, tx))
                {
                    cmd.Parameters.Add("@ID_Tramite", SqlDbType.Int).Value = idTramite;
                    cmd.Parameters.Add("@TipoBien", SqlDbType.VarChar).Value = tipoBien;
                    cmd.Parameters.Add("@Vendedor", SqlDbType.VarChar).Value = vendedor;
                    cmd.Parameters.Add("@Comprador", SqlDbType.VarChar).Value = comprador;
                    cmd.Parameters.Add("@Monto", SqlDbType.Decimal).Value = monto;
                    cmd.Parameters.Add("@Contrato", SqlDbType.VarBinary).Value = contrato;
                    cmd.Parameters.Add("@IdVendedor", SqlDbType.VarBinary).Value = idVendedor;
                    cmd.Parameters.Add("@IdComprador", SqlDbType.VarBinary).Value = idComprador;

                    await cmd.ExecuteNonQueryAsync();
                }

                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }
        public async Task<List<ContratoViewModel>> ObtenerMisContratosAsync(int idUsuario)
        {
            var lista = new List<ContratoViewModel>();

            using var conn = GetConnection();
            await conn.OpenAsync();

            var cmd = new SqlCommand(@"
        SELECT ID_Tramite, TipoTramite, Estado, FechaCreacion
        FROM Tramites
        WHERE ID_Usuario = @id
        AND TipoTramite IN ('COMPRAVENTA','TESTAMENTO','SUCESION')", conn);

            cmd.Parameters.AddWithValue("@id", idUsuario);

            var rd = await cmd.ExecuteReaderAsync();
            while (await rd.ReadAsync())
            {
                lista.Add(new ContratoViewModel
                {
                    ID_Tramite = rd.GetInt32(0),
                    TipoTramite = rd.GetString(1),
                    Estado = rd.GetString(2),
                    FechaCreacion = rd.GetDateTime(3)
                });
            }

            return lista;
        }
        public async Task<ContratoCompleto> ObtenerContratoCompleto(int idTramite)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            string sql = @"
SELECT 
    t.ID_Tramite,
    t.TipoTramite,
    t.Estado,
    t.Observaciones,
    t.FechaCreacion,

    -- INE
    i.CURP,
    i.ActaNacimiento,
    i.ComprobanteDomicilio,
    i.Identificacion,

    -- COMPRAVENTA
    c.Vendedor,
    c.Comprador,
    c.TipoBien,
    c.Monto,
    c.ContratoPDF,
    c.IdentificacionVendedor,
    c.IdentificacionComprador,

    -- TESTAMENTO
    te.EstadoCivil,
    te.TieneHijos,
    te.NumeroHijos,
    te.BienesDeclarados,

    -- SUCESION
    s.TipoSucesion,
    s.NombreFallecido,
    s.FechaDefuncion,
    s.NumeroHerederos

FROM Tramites t
LEFT JOIN TramiteINE i ON i.ID_Tramite = t.ID_Tramite
LEFT JOIN TramiteCompraventa c ON c.ID_Tramite = t.ID_Tramite
LEFT JOIN TramiteTestamento te ON te.ID_Tramite = t.ID_Tramite
LEFT JOIN TramiteSucesion s ON s.ID_Tramite = t.ID_Tramite
WHERE t.ID_Tramite = @id";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", idTramite);

            using var rd = await cmd.ExecuteReaderAsync();
            if (!rd.Read()) return null;

            var contrato = new ContratoCompleto
            {
                IdTramite = rd.GetInt32(0),
                TipoTramite = rd.GetString(1),
                Estado = rd.GetString(2),
                Observaciones = rd.IsDBNull(3) ? "" : rd.GetString(3),
                Fecha = rd.GetDateTime(4)
            };


            // ============ INE ============
            if (contrato.TipoTramite == "INE")
            {
                contrato.Contenido = $"CURP: {rd["CURP"]}";
                contrato.Documento1 = rd["ActaNacimiento"] as byte[];
                contrato.Documento2 = rd["ComprobanteDomicilio"] as byte[];
                contrato.Documento3 = rd["Identificacion"] as byte[];
            }

            // ============ COMPRAVENTA ============
            if (contrato.TipoTramite == "COMPRAVENTA")
            {
                contrato.Contenido =
                    $"Vendedor: {rd["Vendedor"]}\n" +
                    $"Comprador: {rd["Comprador"]}\n" +
                    $"Bien: {rd["TipoBien"]}\n" +
                    $"Monto: ${rd["Monto"]}";

                contrato.Documento1 = rd["ContratoPDF"] as byte[];
                contrato.Documento2 = rd["IdentificacionVendedor"] as byte[];
                contrato.Documento3 = rd["IdentificacionComprador"] as byte[];
            }

            // ============ TESTAMENTO ============
            if (contrato.TipoTramite == "TESTAMENTO")
            {
                contrato.Contenido =
                    $"Estado civil: {rd["EstadoCivil"]}\n" +
                    $"Tiene hijos: {rd["TieneHijos"]}\n" +
                    $"Número de hijos: {rd["NumeroHijos"]}\n" +
                    $"Bienes: {rd["BienesDeclarados"]}";
            }

            // ============ SUCESION ============
            if (contrato.TipoTramite == "SUCESION")
            {
                contrato.Contenido =
                    $"Tipo: {rd["TipoSucesion"]}\n" +
                    $"Fallecido: {rd["NombreFallecido"]}\n" +
                    $"Fecha defunción: {rd["FechaDefuncion"]}\n" +
                    $"Herederos: {rd["NumeroHerederos"]}";
            }

            return contrato;
        }

        // ===================== Funciones Admin =====================

        public async Task<List<TrabajadorItem>> ObtenerTrabajadoresAsync()
        {
            var lista = new List<TrabajadorItem>();

            using var con = GetConnection();
            string q = @"SELECT ID_Trabajador,
                        Nombre + ' ' + ApellidoPaterno AS NombreCompleto,
                        Email
                 FROM Trabajadores";

            using var cmd = new SqlCommand(q, con);
            await con.OpenAsync();
            using var rd = await cmd.ExecuteReaderAsync();

            while (await rd.ReadAsync())
            {
                lista.Add(new TrabajadorItem
                {
                    ID_Trabajador = rd.GetInt32(0),
                    NombreCompleto = rd.GetString(1),
                    Email = rd.GetString(2)
                });
            }

            return lista;
        }
        //eliminar usuarios
        public async Task EliminarUsuarioAsync(int id)
        {
            using var con = GetConnection();
            string q = "DELETE FROM Usuarios WHERE ID_Usuario = @id";
            using var cmd = new SqlCommand(q, con);
            cmd.Parameters.AddWithValue("@id", id);
            await con.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task EliminarTrabajadorAsync(int id)
        {
            using var con = GetConnection();
            string q = "DELETE FROM Trabajadores WHERE ID_Trabajador = @id";
            using var cmd = new SqlCommand(q, con);
            cmd.Parameters.AddWithValue("@id", id);
            await con.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }
        //agregar trabajador
        public async Task<bool> TrabajadorExisteAsync(string email)
        {
            string query = "SELECT COUNT(*) FROM Trabajadores WHERE Email = @e";

            using var con = GetConnection();
            using var cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@e", email);

            await con.OpenAsync();
            int count = Convert.ToInt32(await cmd.ExecuteScalarAsync());
            return count > 0;
        }
        public async Task InsertarTrabajadorAsync(
    string nombre,
    string apP,
    string apM,
    string email,
    string cargo,
    string departamento,
    string password
)
        {
            string query = @"
        INSERT INTO Trabajadores
        (Nombre, ApellidoPaterno, ApellidoMaterno, Email, Cargo, Departamento, PasswordHash)
        VALUES
        (@n, @ap, @am, @e, @c, @d, @p)";

            using var con = GetConnection();
            using var cmd = new SqlCommand(query, con);

            cmd.Parameters.AddWithValue("@n", nombre);
            cmd.Parameters.AddWithValue("@ap", apP);
            cmd.Parameters.AddWithValue("@am", apM);
            cmd.Parameters.AddWithValue("@e", email);
            cmd.Parameters.AddWithValue("@c", cargo);
            cmd.Parameters.AddWithValue("@d", departamento);
            cmd.Parameters.AddWithValue("@p", password); // luego puedes hashearla

            await con.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }


    }
    public class TramiteINEService : ConnectionToSQL
    {
        public async Task CrearTramiteINEAsync(
            string curp,
            byte[] actaNacimiento,
            byte[] comprobanteDomicilio,
            byte[] identificacion)
        {
            if (!UserSession.IsLoggedIn)
                throw new Exception("Sesión no válida");

            using SqlConnection conn = GetConnection();
            await conn.OpenAsync();

            SqlTransaction transaction = conn.BeginTransaction();

            try
            {
                // 1️⃣ Insertar trámite general
                string queryTramite = @"
                    INSERT INTO Tramites (ID_Usuario, TipoTramite)
                    OUTPUT INSERTED.ID_Tramite
                    VALUES (@ID_Usuario, 'INE')";

                int idTramite;

                using (SqlCommand cmd = new SqlCommand(queryTramite, conn, transaction))
                {
                    cmd.Parameters.Add("@ID_Usuario", SqlDbType.Int)
                        .Value = UserSession.IdUsuario;

                    idTramite = (int)await cmd.ExecuteScalarAsync();
                }

                // 2️⃣ Insertar datos INE
                string queryINE = @"
                    INSERT INTO TramiteINE
                    (ID_Tramite, CURP, ActaNacimiento, ComprobanteDomicilio, Identificacion)
                    VALUES
                    (@ID_Tramite, @CURP, @Acta, @Comprobante, @Identificacion)";

                using (SqlCommand cmd = new SqlCommand(queryINE, conn, transaction))
                {
                    cmd.Parameters.Add("@ID_Tramite", SqlDbType.Int).Value = idTramite;
                    cmd.Parameters.Add("@CURP", SqlDbType.VarChar, 18).Value = curp;

                    cmd.Parameters.Add("@Acta", SqlDbType.VarBinary).Value = actaNacimiento;
                    cmd.Parameters.Add("@Comprobante", SqlDbType.VarBinary).Value = comprobanteDomicilio;
                    cmd.Parameters.Add("@Identificacion", SqlDbType.VarBinary).Value = identificacion;

                    await cmd.ExecuteNonQueryAsync();
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
    public class TramiteCompraventaService : ConnectionToSQL
    {
        public async Task CrearTramiteCompraventaAsync(
            string tipoBien,
            string vendedor,
            string comprador,
            decimal monto,
            byte[] contratoPdf,
            byte[] contratoFirmado,
            byte[] idVendedor,
            byte[] idComprador)
        {
            using SqlConnection conn = GetConnection();
            await conn.OpenAsync();
            var tx = conn.BeginTransaction();

            try
            {
                int idTramite;

                using (var cmd = new SqlCommand(@"
                INSERT INTO Tramites (ID_Usuario, TipoTramite)
                OUTPUT INSERTED.ID_Tramite
                VALUES (@ID_Usuario, 'COMPRAVENTA')", conn, tx))
                {
                    cmd.Parameters.Add("@ID_Usuario", SqlDbType.Int).Value = UserSession.IdUsuario;
                    idTramite = (int)await cmd.ExecuteScalarAsync();
                }

                using (var cmd = new SqlCommand(@"
                INSERT INTO TramiteCompraventa
                (ID_Tramite, TipoBien, Vendedor, Comprador, Monto,
                 ContratoPDF, ContratoFirmado, IdentificacionVendedor, IdentificacionComprador)
                VALUES
                (@ID_Tramite, @TipoBien, @Vendedor, @Comprador, @Monto,
                 @ContratoPDF, @ContratoFirmado, @IdVendedor, @IdComprador)", conn, tx))
                {
                    cmd.Parameters.Add("@ID_Tramite", SqlDbType.Int).Value = idTramite;
                    cmd.Parameters.Add("@TipoBien", SqlDbType.VarChar).Value = tipoBien;
                    cmd.Parameters.Add("@Vendedor", SqlDbType.VarChar).Value = vendedor;
                    cmd.Parameters.Add("@Comprador", SqlDbType.VarChar).Value = comprador;
                    cmd.Parameters.Add("@Monto", SqlDbType.Decimal).Value = monto;
                    cmd.Parameters.Add("@ContratoPDF", SqlDbType.VarBinary).Value = contratoPdf;
                    cmd.Parameters.Add("@ContratoFirmado", SqlDbType.VarBinary).Value = (object)contratoFirmado ?? DBNull.Value;
                    cmd.Parameters.Add("@IdVendedor", SqlDbType.VarBinary).Value = idVendedor;
                    cmd.Parameters.Add("@IdComprador", SqlDbType.VarBinary).Value = idComprador;

                    await cmd.ExecuteNonQueryAsync();
                }

                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }
    }
}



