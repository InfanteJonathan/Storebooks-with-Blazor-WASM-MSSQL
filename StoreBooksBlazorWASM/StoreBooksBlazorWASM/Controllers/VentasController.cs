using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SoreBooksBlazorWASM.Service;
using StoreBooksBlazorWASM.Data.ViewModels;

namespace StoreBooksBlazorWASM.Controllers
{
    [Route("api/ventas")]
    [ApiController]
    public class VentasController : ControllerBase
    {
        private readonly VentasManagerV1 _managerV1;

        public VentasController(VentasManagerV1 managerV1)
        {
            _managerV1 = managerV1;
        }

        [HttpGet]
        [Route("detalles")]
        public async Task<ActionResult<List<VentaGeneral>>> DetalleGeneral()
        {
            try
            {
                var response = await _managerV1.DetalleGeneralVenta();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Route("agregarDetalle")]
        public async Task<ActionResult> CrearDetalleVenta([FromBody]LibroViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _managerV1.AgregarCarrito(model);
                    return Ok("Producto Agregado!");
                }
                return BadRequest("Error al agregar el producto");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete]
        [Route("eliminarDetalle")]
        public async Task<ActionResult> EliminarDetalle(int id)
        {
            try
            {
                await _managerV1.EliminarProductoDetalle(id);
                return Ok("Producto Eliminado!");
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Route("completarVenta")]
        public async Task<ActionResult> CompletarVenta(VentaViewModel model)
        {
            try
            {
                await _managerV1.CompletarVenta(model);
                return Ok("Venta realizada con éxito!");
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
