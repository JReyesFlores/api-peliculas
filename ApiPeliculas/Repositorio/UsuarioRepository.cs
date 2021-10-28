using ApiPeliculas.Data;
using ApiPeliculas.Models;
using ApiUsuarios.Repositorio.IRepositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;

namespace ApiPeliculas.Repositorio
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly ApplicationDbContext _db;
        public UsuarioRepository(ApplicationDbContext db)
        {
            this._db = db;
        }
        public bool BorrarUsuario(Usuario Usuario)
        {
            this._db.Usuario.Remove(Usuario);
            return this.Guardar();
        }

        public bool ExisteUsuario(string usuario) => this._db.Usuario.Any(x => x.UsuarioA == usuario);

        public bool ExisteUsuario(int UsuarioId) => this._db.Usuario.Any(x => x.Id == UsuarioId);

        public Usuario GetUsuario(int UsuarioId) => this._db.Usuario.SingleOrDefault(x => x.Id == UsuarioId);

        public ICollection<Usuario> GetUsuarios() => this._db.Usuario.ToList();

        public bool Guardar()
        {
            return (this._db.SaveChanges() != -1);
        }

        public Usuario Login(string Usuario, string password)
        {
            var user = this._db.Usuario.FirstOrDefault(x => x.UsuarioA == Usuario);
            if (user == null) return null;

            if (!VerificarPasswordHash(password, user.PasswordHash, user.PasswordSalt)) return null;
            return user;
        }

        private bool VerificarPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var hashComputado = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < hashComputado.Length; i++)
                {
                    if (hashComputado[i] != passwordHash[i]) return false;
                }
            }
            return true;
        }

        public Usuario Registro(Usuario Usuario, string password)
        {
            byte[] passwordHash, passwordSalt;

            CrearPasswordHash(password, out passwordHash,out passwordSalt);
            Usuario.PasswordHash = passwordHash;
            Usuario.PasswordSalt = passwordSalt;

            this._db.Add(Usuario);
            this.Guardar();
            return Usuario;
        }

        private void CrearPasswordHash(string password, out byte[] passwordHash,out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            } 
        }
    }
}
