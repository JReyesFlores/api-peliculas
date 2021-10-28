using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }
        [StringLength(50)]
        public string UsuarioA { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public DateTime? FechaRegistro { get; set; }
    }
}
