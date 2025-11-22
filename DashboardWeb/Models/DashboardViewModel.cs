namespace DashboardWeb.Models
{
    // Clase para representar una fila de la tabla de campeones
    public class TopVendedor
    {
        public string Nombre { get; set; }
        public string Sucursal { get; set; }
        public decimal VentasActuales { get; set; }
    }

    // Clase contenedora que enviaremos a la Vista
    public class DashboardViewModel
    {
        public List<Region> Regiones { get; set; }
        public List<TopVendedor> TopVendedores { get; set; }
    }
}