using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreBooksBlazorWASM.Data.Models
{
    [Table("Libros")]
    public  class Libro:BaseModel
    {
        [Key]
        public int IdLibro { get; set; }
        public string Titulo { get; set; }
        public string Autor { get; set; }        
        public string? Imagen { get; set; }

        [ForeignKey("Categoria")]
        public int IdCategoria { get; set; }
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
        public DateTime? FechaPublicacion { get; set; }
        public virtual Categoria Categoria { get; set; }
    }
}
