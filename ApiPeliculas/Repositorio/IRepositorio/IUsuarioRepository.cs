using ApiPeliculas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiUsuarios.Repositorio.IRepositorio
{
    public interface IUsuarioRepository
    {
        ICollection<Usuario> GetUsuarios();
        Usuario GetUsuario(int UsuarioId);
        bool ExisteUsuario(string usuario);
        bool ExisteUsuario(int UsuarioId);
        Usuario Registro(Usuario Usuario, string password);
        Usuario Login(string Usuario, string password);
        bool BorrarUsuario(Usuario Usuario);
        bool Guardar();
    }
}
