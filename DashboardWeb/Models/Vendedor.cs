namespace DashboardWeb.Models
{
    public class Vendedor
    {
        public int IdVendedor { get; set; }
        public string Nombre { get; set; }
        public int IdSucursal { get; set; }
        public decimal VentasActuales { get; set; }
        public decimal MetaVentas { get; set; }

        // Lógica del semáforo (Calculada automáticamente)
        public string ColorSemaforo
        {
            get
            {
                if (VentasActuales >= MetaVentas) return "success"; //  color Verde
                if (VentasActuales >= (MetaVentas * 0.8m)) return "warning"; //  color Amarillo
                return "danger"; // Rojo
            }
        }
    }
}