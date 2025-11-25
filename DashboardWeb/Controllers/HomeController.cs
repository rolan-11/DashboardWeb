using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DashboardWeb.Datos;
using DashboardWeb.Models;
using Dapper;
using System.Data;

namespace DashboardWeb.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _contexto;

        public HomeController(ApplicationDbContext contexto)
        {
            _contexto = contexto;
        }

        // PANTALLA PRINCIPAL
        public async Task<IActionResult> Index()
        {
            using (var conexion = _contexto.ObtenerConexion())
            {
                // 1. Traer las Regiones (como antes)
                string sqlRegiones = "SELECT * FROM Regiones";
                var regiones = await conexion.QueryAsync<Region>(sqlRegiones);

                // 2. Traer el TOP 5 MEJORES VENDEDORES 
                string sqlTop = @"
                    SELECT TOP 5 
                        v.Nombre, 
                        s.Nombre as Sucursal, 
                        v.VentasActuales 
                    FROM Vendedores v 
                    INNER JOIN Sucursales s ON v.IdSucursal = s.IdSucursal 
                    ORDER BY v.VentasActuales DESC";

                var topVendedores = await conexion.QueryAsync<TopVendedor>(sqlTop);

                // 3. Empaquetar todo en el ViewModel
                var modelo = new DashboardViewModel
                {
                    Regiones = regiones.ToList(),
                    TopVendedores = topVendedores.ToList()
                };

                return View(modelo);
            }
        }

        // Pantallas Secundarias (Drill Down)
        public async Task<IActionResult> VerSucursales(int idRegion)
        {
            using (var conexion = _contexto.ObtenerConexion())
            {
                string sql = "SELECT * FROM Sucursales WHERE IdRegion = @id";
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

        public async Task<IActionResult> Salir()
        {
            return RedirectToAction("Salir", "Acceso");
        }
    }
}