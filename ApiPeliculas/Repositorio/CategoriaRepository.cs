using ApiPeliculas.Data;
using ApiPeliculas.Models;
using ApiPeliculas.Repositorio.IRepositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Repositorio
{
    public class CategoriaRepository : ICategoriaRepository
    {
        private readonly ApplicationDbContext _bd;

        public CategoriaRepository(ApplicationDbContext bd)
        {
            this._bd = bd;
        }
        public bool ActualizarCategoria(Categoria categoria)
        {
            _bd.Categoria.Update(categoria);
            return Guardar();
        }

        public bool BorrarCategoria(Categoria categoria)
        {
            _bd.Categoria.Remove(categoria);
            return Guardar();
        }

        public bool CrearCategoria(Categoria categoria)
        {
            _bd.Categoria.Add(categoria);
            return Guardar();
        }

        public bool ExisteCategoria(string nombre)
        {
            bool valor = _bd.Categoria.Any(x => x.Nombre.ToLower() == nombre.ToLower());
            return valor;
        }

        public bool ExisteCategoria(int categoriaId)
        {
            bool valor = _bd.Categoria.Any(x => x.Id == categoriaId);
            return valor;
        }

        public Categoria GetCategoria(int categoriaId)
        {
            return _bd.Categoria.SingleOrDefault(x => x.Id == categoriaId);
        }

        public ICollection<Categoria> GetCategorias()
        {
            return _bd.Categoria.OrderBy(c => c.Nombre).ToList();
        }

        public bool Guardar()
        {
            return (_bd.SaveChanges() != -1);
        }
    }
}
