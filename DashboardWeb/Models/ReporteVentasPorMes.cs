namespace DashboardWeb.Models
{
    public class ReporteVentasPorMes
    {
        public int MesNumero { get; set; }          // Ej: 11
        public string MesNombre { get; set; }       // Ej: "Noviembre"
        public int TotalTransacciones { get; set; } // Cuántas ventas hubo
        public decimal TotalRecaudado { get; set; } // Cuánto dinero entró
    }
}