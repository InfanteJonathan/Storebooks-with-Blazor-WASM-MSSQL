using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Net.Http.Json;
using System.Security.Claims;

namespace StoreBooksBlazorWASM.Client.Services
{
    public class UsuarioServicio
    {
        private readonly HttpClient _httpClient;

        public UsuarioServicio(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> ObtenerUsuarioActualAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<string>("api/account/current");
            return response;
        }
    }
}