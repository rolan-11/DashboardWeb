namespace DashboardWeb.Models
{
    public class ReporteCategoriaConteo
    {
        public int CategoriaId { get; set; }
        public string CategoriaNombre { get; set; }  // Ej: "Zapatillas"
        public int CantidadDeProductos { get; set; } // Ej: 15 productos
    }
}