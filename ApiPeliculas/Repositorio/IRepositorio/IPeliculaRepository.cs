using ApiPeliculas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Repositorio.IRepositorio
{
    public interface IPeliculaRepository
    {
        ICollection<Pelicula> GetPeliculas();
        ICollection<Pelicula> GetPeliculasPorCategoria(int categoriaId);
        Pelicula GetPelicula(int PeliculaId);
        bool ExistePelicula(string nombre);
        ICollection<Pelicula> BuscarPelicula(string nombre);
        bool ExistePelicula(int PeliculaId);
        bool CrearPelicula(Pelicula Pelicula);
        bool ActualizarPelicula(Pelicula Pelicula);
        bool BorrarPelicula(Pelicula Pelicula);
        bool Guardar();
    }
}
