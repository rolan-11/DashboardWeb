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
                // 1. Traer las Regiones
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

        // Pantallas Secundarias (Drill Down Geográfico)
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


       

        // REPORTE DE VENTAS 

        // NIVEL 1: Resumen de Meses
        public async Task<IActionResult> ReporteVentasPrincipal()
        {
            using (var conexion = _contexto.ObtenerConexion())
            {
                // Agrupa ventas por Mes y suma totales
                string sql = @"
                    SELECT 
                        MONTH(Fecha) as MesNumero,
                        DATENAME(MONTH, Fecha) as MesNombre,
                        COUNT(*) as TotalTransacciones,
                        SUM(Total) as TotalRecaudado
                    FROM Ventas
                    WHERE YEAR(Fecha) = YEAR(GETDATE())
                    GROUP BY MONTH(Fecha), DATENAME(MONTH, Fecha)
                    ORDER BY MesNumero DESC";

                var reporte = await conexion.QueryAsync<ReporteVentasPorMes>(sql);
                return View(reporte);
            }
        }

        // NIVEL 2: Detalle de Tickets por Mes
        public async Task<IActionResult> DetalleVentasMes(int mes)
        {
            using (var conexion = _contexto.ObtenerConexion())
            {
                string sql = "SELECT * FROM Ventas WHERE MONTH(Fecha) = @m AND YEAR(Fecha) = YEAR(GETDATE())";
                var lista = await conexion.QueryAsync<Venta>(sql, new { m = mes });

                ViewBag.Mes = mes;
                return View(lista);
            }
        }

        //REPORTE DE PRODUCTOS (POR CATEGORÍA) ---

        // NIVEL 1: Resumen de Categorías
        public async Task<IActionResult> ReporteProductosPrincipal()
        {
            using (var conexion = _contexto.ObtenerConexion())
            {
                // Cuenta cuántos productos hay por categoría
                string sql = @"
                    SELECT 
                        c.IdCategoria as CategoriaId,
                        c.Nombre as CategoriaNombre,
                        COUNT(p.IdProducto) as CantidadDeProductos
                    FROM Categorias c
                    LEFT JOIN Productos p ON c.IdCategoria = p.IdCategoria
                    GROUP BY c.IdCategoria, c.Nombre";

                var reporte = await conexion.QueryAsync<ReporteCategoriaConteo>(sql);
                return View(reporte);
            }
        }

        // Lista de Productos en esa Categoría
        public async Task<IActionResult> DetalleProductosCategoria(int idCat)
        {
            using (var conexion = _contexto.ObtenerConexion())
            {
                string sql = "SELECT * FROM Productos WHERE IdCategoria = @id";
                var lista = await conexion.QueryAsync<Producto>(sql, new { id = idCat });
                return View(lista);
            }
        }
    }
}