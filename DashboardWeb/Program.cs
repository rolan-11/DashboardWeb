using DashboardWeb.Datos;
using Microsoft.AspNetCore.Authentication.Cookies; // <--- Necesario para el login

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<ApplicationDbContext>();

// 2. CONFIGURAR LA SEGURIDAD (COOKIES)
// Esto le dice al programa: "Usa cookies para recordar al usuario"
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(option => {
        option.LoginPath = "/Acceso/Login"; // Si no estás logueado, te manda aquí
        option.ExpireTimeSpan = TimeSpan.FromMinutes(20); // La sesión dura 20 mins
    });

var app = builder.Build();

// Configuracion de HTTP 
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // (Nota: En .NET 8/9 a veces se usa MapStaticAssets, pero UseStaticFiles es el estándar clásico)

app.UseRouting();

// 3. ACTIVAR LA SEGURIDAD (El orden importa: Antes de Authorization)
app.UseAuthentication();
app.UseAuthorization();

// app.MapStaticAssets(); // Esta línea a veces da problemas con plantillas viejas, la comentamos por seguridad
// Si tu versión de .NET la requiere obligatoriamente, descoméntala.

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();