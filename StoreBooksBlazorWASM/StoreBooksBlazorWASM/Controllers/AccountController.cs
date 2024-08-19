using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SoreBooksBlazorWASM.Service;
using StoreBooksBlazorWASM.Client.Services;
using System.Threading.Tasks;

namespace StoreBooksBlazorWASM.Controllers
{
    [Route("api/account")]
    [ApiController]
    [Authorize] // Asegúrate de que el usuario esté autenticado
    public class AccountController : ControllerBase
    {
        private readonly UsuarioManagerV1 _usuarioManagerV1;

        public AccountController(UsuarioManagerV1 usuarioManagerV1)
        {
            _usuarioManagerV1 = usuarioManagerV1;
        }

        [Authorize]
        [HttpGet("current")]
        public async Task<ActionResult<string>> GetCurrentUser()
        {
            var userId = await _usuarioManagerV1.ObtenerIdUsuarioActualAsync();
            if (userId == null)
            {
                return NotFound();
            }

            return Ok(userId);
        }
    }
}
