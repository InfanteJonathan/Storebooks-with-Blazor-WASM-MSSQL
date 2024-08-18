using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreBooksBlazorWASM.Data.ViewModels
{
    public class VentaViewModel
    {
        public int IdVenta { get; set; }
        public required string IdUsuario { get; set; }
        public DateTime FechaVenta { get; set; }
        public decimal? Total { get; set; }
        public string? Estado { get; set; }
        public string? MetodoPago { get; set; }
        public string? DireccionEnvio { get; set; }
    }

    public class DetalleVentaViewModel
    {
        public int IdDetalleVentas { get; set; }
        public int IdVenta { get; set; }
        public int IdLibro { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal TotalVenta { get; set; }
    }

    public class VentaGeneral
    {
        public int IdDetalleVentas { get; set; }
        public int IdVenta { get; set; }
        public int IdLibro { get; set; }
        public string ImgLibro { get; set; }
        public string Titulo { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal? TotalVenta { get;  set; }
    }
}
