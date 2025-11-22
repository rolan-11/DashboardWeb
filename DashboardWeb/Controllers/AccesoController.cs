using Microsoft.AspNetCore.Mvc;
using DashboardWeb.Datos;
using DashboardWeb.Models;
using Dapper;
using System.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace DashboardWeb.Controllers
{
    public class AccesoController : Controller
    {
        private readonly ApplicationDbContext _contexto;

        public AccesoController(ApplicationDbContext contexto)
        {
            _contexto = contexto;
        }

        // 1. GET: Muestra la pantalla de Login
        [HttpGet]
        public IActionResult Login()
        {
            // Si ya estás logueado, te manda directo al inicio
            if (User.Identity!.IsAuthenticated) return RedirectToAction("Index", "Home");
            return View();
        }

        // 2. POST: Recibe los datos del formulario cuando le das "Ingresar"
        [HttpPost]
        public async Task<IActionResult> Login(string usuario, string password)
        {
            try
            {
                // A. Encriptar la contraseña a Base64 (Requisito del profe)
                string passBase64 = "";
                if (!string.IsNullOrEmpty(password))
                {
                    byte[] bytes = System.Text.Encoding.UTF8.GetBytes(password);
                    passBase64 = System.Convert.ToBase64String(bytes);
                }

                // B. Preguntar a la Base de Datos si existe usando Dapper
                Usuario? usuarioEncontrado = null;

                using (var conexion = _contexto.ObtenerConexion())
                {
                    string sql = "SELECT * FROM Usuarios WHERE NombreUsuario = @n AND PasswordBase64 = @p";
                    // Aquí Dapper hace la magia segura
                    usuarioEncontrado = await conexion.QueryFirstOrDefaultAsync<Usuario>(sql, new { n = usuario, p = passBase64 });
                }

                // C. Verificar resultado
                if (usuarioEncontrado != null)
                {
                    // Crear la "Identidad" (La credencial del usuario)
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, usuarioEncontrado.NombreUsuario),
                        new Claim("IdUsuario", usuarioEncontrado.IdUsuario.ToString())
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    // Guardar la cookie de sesión y entrar
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.Error = "Usuario o contraseña incorrectos";
                    return View();
                }
            }
            catch (System.Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        // 3. Cerrar Sesión
        public async Task<IActionResult> Salir()
        {
            // Borra la cookie
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Acceso");
        }
    }
}