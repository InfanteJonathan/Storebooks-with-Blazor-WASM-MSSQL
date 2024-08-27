using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StoreBooksBlazorWASM.Data;
using StoreBooksBlazorWASM.Data.Models;
using StoreBooksBlazorWASM.Data.ViewModels;
using StoreBooksBlazorWASM.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoreBooksBlazorWASM.Service
{
    public class VentasManagerV1
    {
        private readonly ApplicationDbContext _context;
        private LibroManagerV1 _libroManagerV1;

        public VentasManagerV1( ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CompletarVenta(VentaViewModel model, string id)
        {
            try
            {
                var buscarVentaActiva = await _context.Ventas
                    .Where(u => u.IdUsuario == id && u.Estado == "En Proceso")
                    .FirstOrDefaultAsync();


                if (string.IsNullOrEmpty(model.MetodoPago) ||
                    string.IsNullOrWhiteSpace(model.DireccionEnvio)
                    )
                {
                    throw new Exception("Error al registrar la venta, intentelo nuevamente");
                }


                buscarVentaActiva.Estado = "Finalizada";
                buscarVentaActiva.MetodoPago = model.MetodoPago;
                buscarVentaActiva.DireccionEnvio = model.DireccionEnvio;

                await _context.SaveChangesAsync();
            }
            catch(Exception ex) 
            {
                throw new Exception(ex.Message);
            }
        }


        public async Task<VentaViewModel> ObtenerVenta(string userid)
        {
            try
            {
                var buscarVenta = await _context.Ventas
                    .Where(v => v.IdUsuario.Equals(userid) && v.Estado.Equals("En Proceso"))
                    .Select(x => new VentaViewModel
                    {
                        IdVenta = x.IdVenta,
                        IdUsuario = x.IdUsuario,
                        Estado = x.Estado,
                        FechaVenta = x.FechaVenta,
                        MetodoPago = x.MetodoPago,
                        DireccionEnvio=x.DireccionEnvio,
                        Total = x.Total
                    }).FirstOrDefaultAsync();

                return buscarVenta;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task AgregarCarrito(LibroViewModel model, string id)
        {
            try
            {
                var buscarVentaActiva = await _context.Ventas
                    .FirstOrDefaultAsync(u => u.IdUsuario == id && u.Estado == "En Proceso");

                if (buscarVentaActiva == null)
                {
                    buscarVentaActiva = new Ventas
                    {
                        IdUsuario = id,
                        FechaVenta = DateTime.Now,
                        Estado = "En Proceso",
                        Total = 0 // Inicializamos Total en 0
                    };

                    _context.Ventas.Add(buscarVentaActiva);
                    await _context.SaveChangesAsync(); // Guardamos para obtener el IdVenta
                }

                var detalleExistente = await _context.DetalleVentas
                    .FirstOrDefaultAsync(dv => dv.IdVenta == buscarVentaActiva.IdVenta && dv.IdLibro == model.IdLibro);

                if (detalleExistente != null)
                {
                    detalleExistente.Cantidad += 1;
                    detalleExistente.TotalVenta = detalleExistente.Cantidad * detalleExistente.PrecioUnitario;
                }
                else
                {
                    detalleExistente = new DetalleVentas
                    {
                        IdVenta = buscarVentaActiva.IdVenta,
                        IdLibro = model.IdLibro,
                        Cantidad = 1,
                        PrecioUnitario = model.Precio,
                        TotalVenta = model.Precio
                    };

                    _context.DetalleVentas.Add(detalleExistente);
                }                              

                buscarVentaActiva.Total += detalleExistente.TotalVenta;
                await _context.SaveChangesAsync();


                await AdministrarInventarioLibros(detalleExistente.IdLibro, "eliminar");
            }
            catch (Exception ex)
            {
                // Puedes agregar logging aquí si lo deseas
                throw new Exception("Ocurrió un error al agregar el libro al carrito.", ex);
            }
        }


        public async Task<List<VentaGeneral>> DetalleGeneralVenta(string id)
        {
            try
            {
                var listaVentaGeneral = new List<VentaGeneral>();

                // Buscar la venta activa junto con sus detalles
                var buscarVentaActiva = await _context.Ventas
                    .Include(v => v.DetalleVentas)
                    .FirstOrDefaultAsync(u => u.IdUsuario == id && u.Estado == "En Proceso");

                if (buscarVentaActiva == null || !buscarVentaActiva.DetalleVentas.Any())
                {
                    throw new HttpRequestException("No tiene artículos añadidos o la venta no está en proceso.");
                }

                // Obtener los IDs de los libros de los detalles de venta
                var idsLibros = buscarVentaActiva.DetalleVentas
                    .Select(dv => dv.IdLibro)
                    .Distinct()
                    .ToList();

                // Obtener los libros en una sola consulta
                var libros = await _context.Libros
                    .Where(l => idsLibros.Contains(l.IdLibro))
                    .ToDictionaryAsync(l => l.IdLibro);

                // Construir la lista de detalles de venta
                foreach (var item in buscarVentaActiva.DetalleVentas)
                {
                    if (libros.TryGetValue(item.IdLibro, out var buscarLibro))
                    {
                        listaVentaGeneral.Add(new VentaGeneral
                        {
                            IdDetalleVentas = item.IdDetalleVentas,
                            IdLibro = item.IdLibro,
                            IdVenta = item.IdVenta,
                            ImgLibro = buscarLibro.Imagen,
                            Titulo = buscarLibro.Titulo,
                            Cantidad = item.Cantidad,
                            PrecioUnitario = item.PrecioUnitario,
                            TotalVenta = item.TotalVenta
                        });
                    }
                }

                // Actualizar el total de la venta
                buscarVentaActiva.Total = buscarVentaActiva.DetalleVentas.Sum(dv => dv.TotalVenta);

                await _context.SaveChangesAsync();

                return listaVentaGeneral;
            }
            catch (Exception ex)
            {
                // Manejar la excepción adecuadamente, por ejemplo, registrando el error
                throw new HttpRequestException("Error al obtener los detalles de la venta: " + ex.Message);
            }
        }




        public async Task EliminarProductoDetalle(int idDetalleVenta)
        {
            try
            {
                var buscarDetalle = await _context.DetalleVentas
                    .Include(v => v.Ventas)
                    .FirstOrDefaultAsync(dv => dv.IdDetalleVentas == idDetalleVenta);


                buscarDetalle.Ventas.Total -= buscarDetalle.PrecioUnitario;


                if (buscarDetalle.Cantidad == 1)
                {
                    _context.DetalleVentas.Remove(buscarDetalle);

                    var contarDetalles = await _context.DetalleVentas
                        .Where(dv => dv.IdVenta == buscarDetalle.Ventas.IdVenta)
                        .CountAsync();

                    if (contarDetalles == 1 )
                    {
                        _context.Ventas.Remove(buscarDetalle.Ventas);
                    }

                }
                else
                {

                    buscarDetalle.Cantidad -= 1;
                    buscarDetalle.TotalVenta -= buscarDetalle.PrecioUnitario;

                }

                await AdministrarInventarioLibros(buscarDetalle.IdLibro,"agregar");

                await _context.SaveChangesAsync();                


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public async Task AdministrarInventarioLibros(int id, string nombre)
        {
            try
            {
                var libro = await _context.Libros.FirstOrDefaultAsync(l => l.IdLibro == id);

                if (nombre.Equals("eliminar"))
                {
                    libro.Cantidad -= 1;
                }
                else
                {
                    libro.Cantidad += 1;
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
