using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreBooksBlazorWASM.Data.Models
{
    public class DetalleVentas
    {
        [Key]
        public int IdDetalleVentas { get; set; }

        [ForeignKey("Ventas")]
        public int IdVenta { get; set; }
        [ForeignKey("Libro")]
        public int IdLibro { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal TotalVenta { get; set; }

        public virtual Ventas Ventas { get; set; }
        public virtual Libro Libro { get; set; }
    }
}
