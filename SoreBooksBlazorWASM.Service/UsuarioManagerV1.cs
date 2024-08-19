using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SoreBooksBlazorWASM.Service
{
    public class UsuarioManagerV1
    {
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public UsuarioManagerV1(AuthenticationStateProvider authenticationStateProvider)
        {
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<string> ObtenerIdUsuarioActualAsync()
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            return user?.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        }
    }


}
