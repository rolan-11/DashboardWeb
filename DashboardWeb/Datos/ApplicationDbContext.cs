using Microsoft.Data.SqlClient;
using System.Data;

namespace DashboardWeb.Datos
{
    public class ApplicationDbContext
    {
        private readonly string _cadenaConexion;

        // El constructor: Lee la configuración de appsettings.json
        public ApplicationDbContext(IConfiguration configuracion)
        {
            _cadenaConexion = configuracion.GetConnectionString("CadenaSQL");
        }

        // Este método devuelve la conexión lista para usar
        public IDbConnection ObtenerConexion()
        {
            return new SqlConnection(_cadenaConexion);
        }
    }
}