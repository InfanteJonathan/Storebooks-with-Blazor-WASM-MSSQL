using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SoreBooksBlazorWASM.Service;
using StoreBooksBlazorWASM.Data;
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
        [Route("detalles/{id}")]
        public async Task<ActionResult<List<VentaGeneral>>> DetalleGeneral(string id)
        {
            try
            {
                var response = await _managerV1.DetalleGeneralVenta(id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("obtenerVenta/{userid}")]
        public async Task<ActionResult<VentaViewModel>> ObtenerVenta(string userid)
        {
            try
            {
                var response = await _managerV1.ObtenerVenta(userid);
                return Ok(response);    
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPost]
        [Route("agregarDetalle/{id}")]
        public async Task<ActionResult> CrearDetalleVenta([FromBody]LibroViewModel model,string id)
        {
            try
            {
                await _managerV1.AgregarCarrito(model, id);
                return Ok("Producto Agregado!");
                
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpDelete]
        [Route("eliminarDetalle/{id}")]
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
        [Route("completarVenta/{userid}")]
        public async Task<ActionResult> CompletarVenta(VentaViewModel model, string userid)
        {
            try
            {
                await _managerV1.CompletarVenta(model, userid);
                return Ok("Venta realizada con éxito!");
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
