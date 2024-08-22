using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using StoreBooksBlazorWASM.Data.ViewModels;
using System.Net.Http.Json;


namespace StoreBooksBlazorWASM.Client.Services
{
    public class UsuarioServicio
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authenticationStateProvider;


        public UsuarioServicio(HttpClient httpClient, AuthenticationStateProvider authenticationStateProvider)
        {
            _httpClient = httpClient;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<string> ObtenerUsuarioActualAsync()
        {
            var user = (await _authenticationStateProvider.GetAuthenticationStateAsync()).User;
            return user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        public async Task<List<UserViewModel>> ObtenerUsuariosAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<UserViewModel>>("api/account/usuarios");
        }


    }
}