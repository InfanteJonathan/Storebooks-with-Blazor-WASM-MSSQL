using StoreBooksBlazorWASM.Data.ViewModels;
using System.Net.Http.Json;

namespace StoreBooksBlazorWASM.Client.Services
{
    public class VentasService
    {
        private HttpClient _httpClient;

        public VentasService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        public async Task<List<VentaGeneral>> detalleGeneral()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/ventas/detalles");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<VentaGeneral>>();
                }
                else
                {
                    var error  = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException(error);
                }
            }
            catch(HttpRequestException ex)
            {
                throw new HttpRequestException("Erro al obtener las ventas, "+ex.Message);
            }
        }

        public async Task<MensajeOperacion> CrearDetalleVenta(LibroViewModel model)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/ventas/agregarDetalle", model);

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


        public async Task<MensajeOperacion> EliminarDetalleVenta(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/ventas/eliminarDetalle{id}");

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

        public async Task<MensajeOperacion> CompletarVenta(VentaViewModel model)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/ventas/completarVenta", model);

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
    }
}
