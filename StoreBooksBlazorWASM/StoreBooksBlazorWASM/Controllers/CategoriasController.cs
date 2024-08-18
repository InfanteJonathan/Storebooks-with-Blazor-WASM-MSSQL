using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreBooksBlazorWASM.Data;
using StoreBooksBlazorWASM.Data.ViewModels;


namespace StoreBooksBlazorWASM.Controllers
{
    [Route("api/categorias")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private ApplicationDbContext _dataContext;

        public CategoriasController(ApplicationDbContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet]
        public async Task<ActionResult<List<CategoriaViewModel>>> Index()
        {
            try
            {
                var lista = await _dataContext.Categorias
                    .Select(cat => new CategoriaViewModel
                    {
                        IdCategoria = cat.IdCategoria,
                        NombreCategoria = cat.NombreCategoria,                        
                    }).ToListAsync();

                return Ok(lista) ?? throw new Exception("Error al obtener la lista de Categorias");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
