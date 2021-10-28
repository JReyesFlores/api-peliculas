using ApiPeliculas.Data;
using ApiPeliculas.Models;
using ApiPeliculas.Repositorio.IRepositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Repositorio
{
    public class PeliculaRepository : IPeliculaRepository
    {
        private readonly ApplicationDbContext _db;
        public PeliculaRepository(ApplicationDbContext db)
        {
            this._db = db;
        }

        public bool ActualizarPelicula(Pelicula Pelicula)
        {
            this._db.Pelicula.Update(Pelicula);
            return Guardar();
        }

        public bool BorrarPelicula(Pelicula Pelicula)
        {
            this._db.Pelicula.Remove(Pelicula);
            return Guardar();
        }

        public ICollection<Pelicula> BuscarPelicula(string nombre)
        {
            //var listaPeliculas = this._db.Pelicula.Where(x => x.Nombre.ToLower() == nombre.ToLower()).ToList();
            //return listaPeliculas;
            IQueryable<Pelicula> query = this._db.Pelicula;
            if (!string.IsNullOrEmpty(nombre))
            {
                query = query.Where(x => x.Nombre.ToLower() == nombre.ToLower());
            }
            return query.ToList();
        }

        public bool CrearPelicula(Pelicula Pelicula)
        {
            this._db.Pelicula.Add(Pelicula);
            return Guardar();
        }

        public bool ExistePelicula(string nombre)
        {
            return this._db.Pelicula.Any(x => x.Nombre.ToLower() == nombre.ToLower());
        }

        public bool ExistePelicula(int peliculaId)
        {
            return this._db.Pelicula.Any(x => x.Id == peliculaId);
        }

        public Pelicula GetPelicula(int PeliculaId)
        {
            return this._db.Pelicula.SingleOrDefault(x => x.Id == PeliculaId);
        }

        public ICollection<Pelicula> GetPeliculas()
        {
            return this._db.Pelicula.ToList();
        }

        public ICollection<Pelicula> GetPeliculasPorCategoria(int categoriaId)
        {
            var listaPeliculas = this._db.Pelicula.Where(x => x.categoriaId == categoriaId).ToList();
            return listaPeliculas;
        }

        public bool Guardar()
        {
            return (this._db.SaveChanges() != -1);
        }
    }
}
