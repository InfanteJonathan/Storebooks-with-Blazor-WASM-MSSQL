using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using StoreBooksBlazorWASM.Data.ViewModels;
using StoreBooksBlazorWASM.Service;


namespace StoreBooksBlazorWASM.Controllers
{
    [Route("api/libros")]
    [ApiController]
    public class LibrosController : ControllerBase
    {
        private LibroManagerV1 _managerV1;
        public LibrosController(LibroManagerV1 managerv1)
        {
            _managerV1 = managerv1;
        }

        [HttpGet]
        [Route("lista")]
        public async Task<ActionResult<List<LibroViewModel>>> Index()
        {
            try
            {
                var lista = await _managerV1.GetAll();
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }


        [HttpGet]
        [Route("buscar/{texto?}")]
        public async Task<ActionResult<List<LibroViewModel>>> BuscarLibro(string? texto)
        {
            try
            {
                var busqueda = await _managerV1.BusquedaLibros(texto);

                return Ok(busqueda);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet]
        [Route("detalle/{id}")]
        public async Task<ActionResult<LibroViewModel>> Detalle(int id)
        {
            try
            {
                var model = await _managerV1.GetById(id);

                return Ok(model);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }


        }

        [HttpPost]
        [Route("registrar")]
        public async Task<ActionResult> Registrar([FromBody] LibroViewModel libro)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _managerV1.CrearLibro(libro);

                    return Ok("Libro creado exitosamente");
                }

                return BadRequest("Error en el registro de datos");

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }


        [HttpPut]
        [Route("editar/{id}")]
        public async Task<ActionResult> Editar(int id, [FromBody] LibroViewModel model)
        {
            try
            {
                await _managerV1.Editar(id, model);

                return Ok("Se registro correctamente");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete]
        [Route("eliminar/{id}")]
        public async Task<ActionResult> Eliminar(int id)
        {
            await _managerV1.ELiminar(id);
            return Ok("Se elimino Correctamente");
        }



    }
}
