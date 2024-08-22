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

        public async Task<string?> GetUserIdAsync()
        {
            var user = (await _authenticationStateProvider.GetAuthenticationStateAsync()).User;
            return user.FindFirst(u => u.Type.Contains("nameidentifier"))?.Value;
        }
    }


}
