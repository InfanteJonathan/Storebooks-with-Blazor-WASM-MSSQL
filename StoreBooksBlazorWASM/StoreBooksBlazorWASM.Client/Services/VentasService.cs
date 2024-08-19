using Microsoft.AspNetCore.Identity;
using StoreBooksBlazorWASM.Data;
using StoreBooksBlazorWASM.Data.ViewModels;
using System.Net.Http.Json;

namespace StoreBooksBlazorWASM.Client.Services
{
    public class VentasService
    {
        private HttpClient _httpClient;
        private readonly UsuarioServicio _usuarioServicio;

        public VentasService(HttpClient httpClient, UsuarioServicio usuarioServicio)
        {
            _httpClient = httpClient;
            _usuarioServicio = usuarioServicio;
        }


        public async Task<List<VentaGeneral>> detalleGeneral(string id)
        {
            try
            {
                //var usuarioActualId = await _usuarioServicio.ObtenerUsuarioActualAsync();

                var response = await _httpClient.GetAsync($"api/ventas/detalles/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<VentaGeneral>>();
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception(error);
                }
            }
            catch (HttpRequestException ex)
            {
                throw new HttpRequestException("Error al obtener las ventas, " + ex.Message);
            }
        }


        public async Task<MensajeOperacion> CrearDetalleVenta(LibroViewModel model,string id)
        {
            try
            {

                var response = await _httpClient.PostAsJsonAsync($"api/ventas/agregarDetalle/{id}", model);


                if(response.IsSuccessStatusCode)
                {
                    var mensajeExito = await response.Content.ReadAsStringAsync();
                    return new MensajeOperacion { Exito = true, Mensaje = mensajeExito };
                }
                else
                {
                    var mensajeError = await response.Content.ReadAsStringAsync();
                    return new MensajeOperacion { Exito = false, Mensaje = mensajeError };
                }
            }
            catch(HttpRequestException ex)
            {
                throw new HttpRequestException(ex.Message);
            }
        }

        public async Task<VentaViewModel> ObtenerVenta(string userid)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/ventas/obtenerVenta/{userid}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<VentaViewModel>();
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception(error);
                }
            }
            catch(HttpRequestException ex)
            {
                throw new HttpRequestException(ex.Message);
            }
        }


        public async Task<MensajeOperacion> EliminarDetalleVenta(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/ventas/eliminarDetalle/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var mensajeExito = await response.Content.ReadAsStringAsync();
                    return new MensajeOperacion { Exito = true, Mensaje = mensajeExito };
                }
                else
                {
                    var mensajeError = await response.Content.ReadAsStringAsync();
                    return new MensajeOperacion { Exito = true, Mensaje = mensajeError };
                }
            }
            catch (HttpRequestException ex)
            {
                throw new HttpRequestException(ex.Message);
            }
        }

        public async Task<MensajeOperacion> CompletarVenta(VentaViewModel model,string userid)
        {
            try
            {                
                var response = await _httpClient.PostAsJsonAsync($"api/ventas/completarVenta/{userid}", model);

                if (response.IsSuccessStatusCode)
                {
                    var mensajeExito = await response.Content.ReadAsStringAsync();
                    return new MensajeOperacion { Exito = true, Mensaje = mensajeExito };
                }
                else
                {
                    var mensajeError = await response.Content.ReadAsStringAsync();
                    return new MensajeOperacion { Exito = false, Mensaje = mensajeError };
                }
            }
            catch (HttpRequestException ex)
            {
                throw new HttpRequestException(ex.Message);
            }
        }
    }
}
