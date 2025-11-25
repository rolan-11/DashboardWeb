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

        //pantalla de Login
        [HttpGet]
        public IActionResult Login()
        {
            // Si ya estás logueadote manda directo al inicio
            if (User.Identity!.IsAuthenticated) return RedirectToAction("Index", "Home");
            return View();
        }

        // 2. POST: Recibe los datos del formulario cuando le das "Ingresar"
        [HttpPost]
        public async Task<IActionResult> Login(string usuario, string password)
        {
            try
            {
                //contraseña a Base64
                string passBase64 = "";
                if (!string.IsNullOrEmpty(password))
                {
                    byte[] bytes = System.Text.Encoding.UTF8.GetBytes(password);
                    passBase64 = System.Convert.ToBase64String(bytes);
                }

                //Preguntar a la Base de Datos si existe usando Dapper
                Usuario? usuarioEncontrado = null;

                using (var conexion = _contexto.ObtenerConexion())
                {
                    string sql = "SELECT * FROM Usuarios WHERE NombreUsuario = @n AND PasswordBase64 = @p";
                    usuarioEncontrado = await conexion.QueryFirstOrDefaultAsync<Usuario>(sql, new { n = usuario, p = passBase64 });
                }

                // C. Verificar resultado
                if (usuarioEncontrado != null)
                {
                    // 1. Crear la "Identidad" (La credencial del usuario)
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, usuarioEncontrado.NombreUsuario),
                        new Claim("IdUsuario", usuarioEncontrado.IdUsuario.ToString())
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    // 2. Guardar la cookie de sesión y entrar
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                    // 3. ✅ Mensaje de Éxito
                    TempData["MensajeExito"] = "Bienvenido al sistema, " + usuarioEncontrado.NombreUsuario;

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // ❌ Mensaje de Error 
                    TempData["MensajeError"] = "Usuario o clave incorrectos. Intenta de nuevo.";
                    return View();
                }
            }
            catch (System.Exception ex)
            {
                // Error técnico inesperado
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