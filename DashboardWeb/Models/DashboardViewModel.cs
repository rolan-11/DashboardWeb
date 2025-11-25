namespace DashboardWeb.Models
{
    //tabla de campeones
    public class TopVendedor
    {
        public string Nombre { get; set; }
        public string Sucursal { get; set; }
        public decimal VentasActuales { get; set; }
    }

    // Clase contenedora 
    public class DashboardViewModel
    {
        public List<Region> Regiones { get; set; }
        public List<TopVendedor> TopVendedores { get; set; }
    }
}