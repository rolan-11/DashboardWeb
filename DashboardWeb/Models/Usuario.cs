namespace DashboardWeb.Models
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public string PasswordBase64 { get; set; }
    }
}