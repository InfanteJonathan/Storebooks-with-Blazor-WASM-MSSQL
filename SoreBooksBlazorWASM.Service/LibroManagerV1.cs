using Microsoft.EntityFrameworkCore;
using StoreBooksBlazorWASM.Data;
using StoreBooksBlazorWASM.Data.Models;
using StoreBooksBlazorWASM.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreBooksBlazorWASM.Service
{
    public class LibroManagerV1
    {
        private readonly ApplicationDbContext _context;

        public LibroManagerV1(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<List<LibroViewModel>> GetAll()
        {
            var lista = await  _context.Libros
                    .Where(l => l.Activo)
                    .Select(libro => new LibroViewModel
                    {
                        IdLibro = libro.IdLibro,
                        Titulo = libro.Titulo,
                        Autor = libro.Autor,
                        Precio = libro.Precio,
                        Categoria = libro.Categoria.NombreCategoria,
                        Imagen = libro.Imagen,
                        FechaPublicacion = libro.FechaPublicacion,
                    })
                    .OrderByDescending(l => l.IdLibro)
                    .Take(20)
                    .ToListAsync();

            return lista;
        }

        public async Task<LibroViewModel> GetById(int id)
        {
            var libro = await  _context.Libros.Where(x => x.Activo && x.IdLibro == id).FirstOrDefaultAsync();

            var buscarCategoria = await  _context.Categorias
                .Where(c => c.IdCategoria == libro.IdCategoria)
                .Select(c => c.NombreCategoria)
                .FirstOrDefaultAsync();


            var model = new LibroViewModel()
            {
                IdLibro = libro.IdLibro,
                Titulo = libro.Titulo,
                Autor = libro.Autor,
                Precio = libro.Precio,
                Categoria = buscarCategoria,
                Imagen = libro.Imagen,
                FechaPublicacion = libro.FechaPublicacion,
                activo = libro.Activo
            };

            return model;
        }

        public async Task<List<LibroViewModel>> BusquedaLibros(string? nombre)
        {
            var busqueda =  await _context.Libros
                    .Where(libro => libro.Activo && (libro.Titulo.Contains(nombre) ||
                                    libro.Autor.Contains(nombre) ||
                                    libro.Categoria.NombreCategoria.Contains(nombre)))
                    .OrderByDescending(libro => libro.IdLibro)
                    .Select(libro => new LibroViewModel
                    {
                        IdLibro = libro.IdLibro,
                        Titulo = libro.Titulo,
                        Autor = libro.Autor,
                        Precio = libro.Precio,
                        Categoria = libro.Categoria.NombreCategoria,
                        Imagen = libro.Imagen,
                        FechaPublicacion = libro.FechaPublicacion,
                    })
                    .ToListAsync();

            return busqueda;
        }

        public async Task CrearLibro(LibroViewModel model)
        {
            try
            {
                var buscarId = await _context.Categorias
                                .Where(c => c.NombreCategoria.Equals(model.Categoria))
                                .Select(x => x.IdCategoria)
                                .FirstOrDefaultAsync();

                if (buscarId <= 0) throw new Exception("No se pudo encontrar la categoria");

                Libro newLibro = new Libro();

                newLibro.Titulo = model.Titulo;
                newLibro.Autor = model.Autor;
                newLibro.Precio = model.Precio;
                newLibro.IdCategoria = buscarId;
                newLibro.FechaPublicacion = model.FechaPublicacion;
                newLibro.Imagen = model.Imagen;
                newLibro.Activo = true;
                newLibro.FechaCreacion = DateTime.Now;


                _context.Libros.Add(newLibro);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("No se pudo completar el registro, intentelo nuevamente");
            }
            

        }

        public async Task Editar(int id,LibroViewModel model)
        {
            var libro = await  _context.Libros.Where(x => x.IdLibro == id).FirstOrDefaultAsync();

            var buscarIdCategoria =  await _context.Categorias
                .Where(c => c.Activo && c.NombreCategoria.Equals(model.Categoria))
                .Select(c => c.IdCategoria)
                .FirstOrDefaultAsync();


            libro.Titulo = model.Titulo;
            libro.Autor = model.Autor;
            libro.Precio = model.Precio;
            libro.IdCategoria = buscarIdCategoria;
            libro.FechaPublicacion = model.FechaPublicacion;
            libro.Imagen = model.Imagen;
            libro.Activo = model.activo;
            libro.FechaEdicion = DateTime.Now;

           await _context.SaveChangesAsync();
        }

        public async Task ELiminar(int id)
        {
            var libro = await  _context.Libros.FirstOrDefaultAsync(l => l.IdLibro == id);

            libro.Activo = false;
            await _context.SaveChangesAsync();

        }
    }
}
