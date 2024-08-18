using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreBooksBlazorWASM.Data.Models
{
    public class Ventas
    {
        [Key]
        public int IdVenta { get; set; }
        public  string IdUsuario {  get; set; }
        public  DateTime FechaVenta { get; set; }
        public decimal? Total { get; set; }
        public string? Estado { get; set; }
        public string? MetodoPago { get; set; }
        public string? DireccionEnvio { get; set; }
        public virtual ICollection<DetalleVentas> DetalleVentas { get; set; }

    }
}
