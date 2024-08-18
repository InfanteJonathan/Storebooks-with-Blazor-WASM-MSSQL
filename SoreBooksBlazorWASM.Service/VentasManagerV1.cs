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
        private readonly AuthenticationStateProvider _authProvider;
        private readonly UserManager<IdentityUser> _userManager;

        public VentasManagerV1(ApplicationDbContext context, AuthenticationStateProvider authProvider, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _authProvider = authProvider;
            _userManager = userManager;
        }

        public async Task CompletarVenta(VentaViewModel model)
        {
            try
            {
                var authState = await _authProvider.GetAuthenticationStateAsync();
                var user = authState.User;
                var usuarioActual = await _userManager.GetUserAsync(user);

                var buscarVentaActiva = await _context.Ventas
                    .Where(u => u.IdUsuario == usuarioActual.Id && u.Estado == "En Proceso")
                    .FirstOrDefaultAsync();


                if (buscarVentaActiva == null)
                {
                    throw new Exception("Carrito de Compras Vacio");
                }

                var TotalVenta = await _context.DetalleVentas
                    .Where(dv => dv.IdVenta == buscarVentaActiva.IdVenta)
                    .SumAsync(d => d.Cantidad);

                buscarVentaActiva.Estado = "Finalizada";
                buscarVentaActiva.MetodoPago = model.MetodoPago;
                buscarVentaActiva.DireccionEnvio = model.DireccionEnvio;
                buscarVentaActiva.Total = TotalVenta;

                await _context.SaveChangesAsync();
            }
            catch(Exception ex) 
            {
                throw;
            }
        }

        public async Task AgregarCarrito(LibroViewModel model)
        {
            var authState = await _authProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            var usuarioActual = await _userManager.GetUserAsync(user);

            var buscarVentaActiva = await _context.Ventas
                .Where(u => u.IdUsuario == usuarioActual.Id && u.Estado == "En Proceso")
                .FirstOrDefaultAsync();

            if (buscarVentaActiva == null)
            {
                var nuevaVenta = new Ventas
                {
                    IdUsuario = usuarioActual.Id,
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

            buscarVentaActiva.Total += detalleExistente.TotalVenta; 

            await _context.SaveChangesAsync();
        }

        public async Task<List<VentaGeneral>> DetalleGeneralVenta()
        {
            try
            {
                var authState = await _authProvider.GetAuthenticationStateAsync();
                var user = authState.User;
                var usuarioActual = await _userManager.GetUserAsync(user);

                List<VentaGeneral> listaVentaGeneral = new List<VentaGeneral>();

                var buscarVentaActiva = await _context.Ventas
                    .Where(u => u.IdUsuario == usuarioActual.Id && u.Estado == "En Proceso")
                    .FirstOrDefaultAsync();


                if (buscarVentaActiva == null)
                {
                    throw new Exception("No tiene articulos añadidos");
                }

                var buscarDetalleVenta = await _context.DetalleVentas
                    .Where(dv => dv.IdVenta == buscarVentaActiva.IdVenta)
                    .ToListAsync();

                foreach (var item in buscarDetalleVenta)
                {
                    var nuevoDetalle = new VentaGeneral
                    {
                        IdDetalleVentas = item.IdDetalleVentas,
                        IdLibro = item.IdLibro,
                        IdVenta = item.IdVenta,
                        ImgLibro = item.Libro.Imagen,
                        Titulo = item.Libro.Titulo,
                        Cantidad = item.Cantidad,
                        PrecioUnitario = item.PrecioUnitario,
                        TotalVenta = item.Ventas.Total,
                    };

                    listaVentaGeneral.Add(nuevoDetalle);
                }

                return listaVentaGeneral;

            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public async Task EliminarProductoDetalle(int idDetalleVenta)
        {
            try
            {
                var buscarDetalle = await _context.DetalleVentas
                    .Where(dv => dv.IdDetalleVentas == idDetalleVenta)
                    .FirstOrDefaultAsync();

                var buscarVenta = await _context.Ventas
                    .Where(v => v.IdVenta == buscarDetalle.IdVenta)
                    .FirstOrDefaultAsync();

                if (buscarDetalle.Cantidad == 1)
                {
                    _context.DetalleVentas.Remove(buscarDetalle);
                    _context.Ventas.Remove(buscarVenta);
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
                throw;
            }
        }

    }
}
