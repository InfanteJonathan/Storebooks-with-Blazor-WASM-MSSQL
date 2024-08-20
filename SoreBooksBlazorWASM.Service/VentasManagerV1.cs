using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StoreBooksBlazorWASM.Data;
using StoreBooksBlazorWASM.Data.Models;
using StoreBooksBlazorWASM.Data.ViewModels;
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
                    .Where(u => u.IdUsuario == id && u.Estado == "En Proceso")
                    .FirstOrDefaultAsync();

                if (buscarVentaActiva == null)
                {
                    var nuevaVenta = new Ventas
                    {
                        IdUsuario = id,
                        FechaVenta = DateTime.Now,
                        Estado = "En Proceso"
                    };

                    _context.Ventas.Add(nuevaVenta);
                    await _context.SaveChangesAsync();

                    buscarVentaActiva = nuevaVenta;
                }

                var detalleExistente = await _context.DetalleVentas
                    .Where(dv => dv.IdVenta == buscarVentaActiva.IdVenta && dv.IdLibro == model.IdLibro)
                    .FirstOrDefaultAsync();

                if (detalleExistente != null)
                {
                    detalleExistente.Cantidad += 1;
                    detalleExistente.TotalVenta = detalleExistente.Cantidad * detalleExistente.PrecioUnitario;

                }
                else
                {
                    var nuevoDetalle = new DetalleVentas
                    {
                        IdVenta = buscarVentaActiva.IdVenta,
                        IdLibro = model.IdLibro,
                        Cantidad = 1,
                        PrecioUnitario = model.Precio,
                        TotalVenta = model.Precio
                    };

                    detalleExistente = nuevoDetalle;

                    _context.DetalleVentas.Add(nuevoDetalle);
                }

                var sumaTotalDetalles = await _context.DetalleVentas
                    .Where(dv => dv.IdVenta == buscarVentaActiva.IdVenta && dv.IdLibro == model.IdLibro)
                    .SumAsync(dv => dv.TotalVenta);

                buscarVentaActiva.Total += sumaTotalDetalles;

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<VentaGeneral>> DetalleGeneralVenta(string id)
        {
            try
            {
                List<VentaGeneral> listaVentaGeneral = new List<VentaGeneral>();

                // Buscar la venta activa
                var buscarVentaActiva = await _context.Ventas
                    .Where(u => u.IdUsuario == id && u.Estado == "En Proceso")
                    .FirstOrDefaultAsync();

                // Si no se encuentra la venta activa, lanzar una excepción
                if (buscarVentaActiva == null)
                {
                    throw new Exception("No tiene artículos añadidos o la venta no está en proceso.");
                }

                // Obtener detalles de venta
                var buscarDetalleVenta = await _context.DetalleVentas
                    .Where(dv => dv.IdVenta == buscarVentaActiva.IdVenta)
                    .ToListAsync();

                if (buscarDetalleVenta == null)
                {
                    throw new Exception("No tiene artículos añadidos");
                }

                // Obtener los libros en una sola consulta para evitar múltiples consultas
                var idsLibros = buscarDetalleVenta.Select(dv => dv.IdLibro).Distinct().ToList();

                var libros = await _context.Libros
                    .Where(l => idsLibros.Contains(l.IdLibro))
                    .ToDictionaryAsync(l => l.IdLibro);

                foreach (var item in buscarDetalleVenta)
                {
                    // Buscar el libro correspondiente usando el diccionario
                    if (libros.TryGetValue(item.IdLibro, out var buscarLibro))
                    {
                        var nuevoDetalle = new VentaGeneral
                        {
                            IdDetalleVentas = item.IdDetalleVentas,
                            IdLibro = item.IdLibro,
                            IdVenta = item.IdVenta,
                            ImgLibro = buscarLibro.Imagen,
                            Titulo = buscarLibro.Titulo,
                            Cantidad = item.Cantidad,
                            PrecioUnitario = item.PrecioUnitario,
                            TotalVenta = item.TotalVenta
                        };

                        listaVentaGeneral.Add(nuevoDetalle);
                    }
                    else
                    {
                        // Manejar el caso en que no se encuentra el libro
                        // Por ejemplo, puedes registrar un error o agregar un mensaje de advertencia
                    }
                }

                var sumaTotalVenta = buscarDetalleVenta.Sum(dv => dv.TotalVenta);
                buscarVentaActiva.Total = sumaTotalVenta;

                await _context.SaveChangesAsync();

                return listaVentaGeneral;
            }
            catch (Exception ex)
            {
                // Manejar la excepción adecuadamente, por ejemplo, registrando el error
                throw new HttpRequestException("Error al obtener los detalles de la venta, " + ex.Message);
            }
        }



        public async Task EliminarProductoDetalle(int idDetalleVenta)
        {
            try
            {
                var buscarDetalle = await _context.DetalleVentas
                    .FirstOrDefaultAsync(dv => dv.IdDetalleVentas == idDetalleVenta);

                var buscarVenta = await _context.Ventas
                    .FirstOrDefaultAsync(v => v.IdVenta == buscarDetalle.IdVenta);

                if (buscarDetalle.Cantidad == 1)
                {
                    _context.DetalleVentas.Remove(buscarDetalle);

                    var contarDetalles = await _context.DetalleVentas
                        .Where(dv => dv.IdVenta == buscarVenta.IdVenta)
                        .CountAsync();

                    if (contarDetalles <=1 )
                    {
                        _context.Ventas.Remove(buscarVenta);
                    }

                }
                else if(buscarDetalle.Cantidad >1)
                {

                    buscarDetalle.Cantidad = buscarDetalle.Cantidad - 1;
                    buscarDetalle.TotalVenta = buscarDetalle.TotalVenta - buscarDetalle.PrecioUnitario;


                    buscarVenta.Total -= buscarDetalle.TotalVenta; 

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
