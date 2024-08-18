using System.ComponentModel.DataAnnotations;

namespace StoreBooksBlazorWASM.Data.ViewModels
{
    public class LibroViewModel
    {
        public int IdLibro { get; set; }

        [Required(ErrorMessage = "El título es obligatorio.")]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "El autor es obligatorio.")]
        public string Autor { get; set; }

        [Required(ErrorMessage = "La categoría es obligatoria.")]
        public string Categoria { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio.")]
        [Range(0, double.MaxValue, ErrorMessage = "El precio debe ser un valor positivo.")]
        public decimal Precio { get; set; }
        [Required]
        public int Cantidad { get; set; }

        [Required(ErrorMessage = "La imagen es obligatoria.")]
        public string Imagen { get; set; }

        [Required(ErrorMessage = "La fecha de publicación es obligatoria.")]
        public DateTime? FechaPublicacion { get; set; }

        public bool activo { get; set; } 
    }

}
