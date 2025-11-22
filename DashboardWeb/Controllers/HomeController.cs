using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DashboardWeb.Datos;
using DashboardWeb.Models;
using Dapper; 
using System.Data;

namespace DashboardWeb.Controllers
{
    [Authorize] // seguridad
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _contexto;

        // conexión a la base de datos
        public HomeController(ApplicationDbContext contexto)
        {
            _contexto = contexto;
        }

        // PANTALLA 1: LISTA DE REGIONES (Nivel 1 del Drill Down)
        public async Task<IActionResult> Index()
        {
            // 1. Conectamos
            using (var conexion = _contexto.ObtenerConexion())
            {
                // 2. Consulta SQL (Requisito 1)
                string sql = "SELECT * FROM Regiones";

                // 3. Traemos los datos
                var regiones = await conexion.QueryAsync<Region>(sql);

                // 4. Se los pasamos a la vista
                return View(regiones);
            }
        }

        public async Task<IActionResult> Salir()
        {
            return RedirectToAction("Salir", "Acceso");
        }

        public async Task<IActionResult> VerSucursales(int idRegion)
        {
            using (var conexion = _contexto.ObtenerConexion())
            {
                // Consulta SQL con filtro (WHERE)
                string sql = "SELECT * FROM Sucursales WHERE IdRegion = @id";

                // Traemos solo las sucursales de esa región
                var sucursales = await conexion.QueryAsync<Sucursal>(sql, new { id = idRegion });

                return View(sucursales);
            }
        }

        public async Task<IActionResult> VerVendedores(int idSucursal)
        {
            using (var conexion = _contexto.ObtenerConexion())
            {
                string sql = "SELECT * FROM Vendedores WHERE IdSucursal = @id";
                var vendedores = await conexion.QueryAsync<Vendedor>(sql, new { id = idSucursal });
                return View(vendedores);
            }
        }
    }


}