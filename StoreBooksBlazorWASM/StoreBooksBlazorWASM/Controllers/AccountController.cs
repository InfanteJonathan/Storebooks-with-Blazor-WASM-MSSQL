using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoreBooksBlazorWASM.Service;
using StoreBooksBlazorWASM.Client.Services;
using StoreBooksBlazorWASM.Data;
using StoreBooksBlazorWASM.Data.ViewModels;
using System.Threading.Tasks;

namespace StoreBooksBlazorWASM.Controllers
{
    [Route("api/account")]
    [ApiController]
    /*[Authorize]*/ // Asegúrate de que el usuario esté autenticado
    public class AccountController : ControllerBase
    {
        private readonly UsuarioManagerV1 _usuarioManagerV1;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AspNetRoleManager<ApplicationUser> role;



        public AccountController(UsuarioManagerV1 usuarioManagerV1, UserManager<ApplicationUser> userManager)
        {
            _usuarioManagerV1 = usuarioManagerV1;
            _userManager = userManager;
        }

        [Authorize]
        [HttpGet("current")]
        public async Task<ActionResult<string>> GetCurrentUser()
        {
            var userId = await _usuarioManagerV1.GetUserIdAsync();
            if (userId == null)
            {
                return NotFound();
            }

            return Ok(userId);
        }

        [HttpGet("usuarios")]
        public async Task<IActionResult> ListaUsuarios()
        {
            try
            {                
                var usuarios = _userManager.Users
                    .Select(u =>  new UserViewModel
                    {
                        Id = u.Id,
                        Name = u.UserName,
                        Email = u.Email,
                        Password = u.PasswordHash
                    }).ToList();

                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocurrió un error al obtener los usuarios", ex.Message });
            }
        }





        [HttpPost]
        public async Task<ActionResult> RegisterUserByAdmin(UserViewModel user,string role)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser appuser = new ApplicationUser
                {
                    UserName = user.Name,
                    Email = user.Email
                };

                IdentityResult result = await _userManager.CreateAsync(appuser, user.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(appuser, role);
                }


                return BadRequest(result.Errors);
            }

            return BadRequest();
        }
    }
}
