using System.Net.Http.Json;
using StoreBooksBlazorWASM.Data.ViewModels;

namespace StoreBooksBlazorWASM.Client.Services
{

    public class LibroService
    {
        private readonly HttpClient _httpClient;

        public LibroService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<LibroViewModel>> ListaLibros()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/libros/lista");

                response.EnsureSuccessStatusCode(); // Throws an exception if the HTTP response status is an error

                return await response.Content.ReadFromJsonAsync<List<LibroViewModel>>();
            }
            catch (HttpRequestException ex)
            {
                // Log the exception (ex) here
                throw; // Rethrow the exception to be handled elsewhere
            }
        }


        public async Task<LibroViewModel> obtenerLibro(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/libros/detalle/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<LibroViewModel>();
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Error al obtener datos del libro: {response.StatusCode} - {errorMessage}");
                }
            }
            catch (HttpRequestException ex)
            {
                throw new HttpRequestException($"{ex.Message}");
            }
        }

        public async Task<List<LibroViewModel>> Buscar(string texto)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/libros/buscar/{texto}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<LibroViewModel>>();
                }
                else
                {
                    var messageError = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"{response.StatusCode} - {messageError}");
                }
            }
            catch (HttpRequestException ex)
            {
                throw new HttpRequestException(ex.Message);
            }
        }

        public async Task<MensajeOperacion> Crear(LibroViewModel model)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"api/libros/registrar", model);

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
                return new MensajeOperacion { Exito = true, Mensaje = ex.Message };
            }
        }

        public async Task<MensajeOperacion> Editar(int id, LibroViewModel model)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/libros/editar/{id}", model);

                if (response.IsSuccessStatusCode)
                {
                    var mensajeExito = await response.Content.ReadAsStringAsync();
                    return new MensajeOperacion { Exito = true, Mensaje = mensajeExito };
                }
                else
                {
                    var mensajeError = await response.Content.ReadAsStringAsync();
                    return new MensajeOperacion {Exito = false,Mensaje = mensajeError };
                }

            }
            catch (HttpRequestException ex)
            {

                return new MensajeOperacion { Exito = false, Mensaje = ex.Message };
            }
        }

        public async Task<MensajeOperacion> Eliminar(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/libros/eliminar/{id}");

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
            catch(HttpRequestException ex)
            {
                return new MensajeOperacion {Exito=false, Mensaje = ex.Message};
            }
        }






    }
}
