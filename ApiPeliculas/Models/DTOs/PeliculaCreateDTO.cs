using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Models.DTOs
{
    public class PeliculaCreateDTO
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Nombre { get; set; }
        //[Required(ErrorMessage = "La RutaImagen es obligatoria.")]
        public string RutaImagen { get; set; }
        public IFormFile Foto { get; set; }
        [Required(ErrorMessage = "La Descripcion es obligatoria.")]
        public string Descripcion { get; set; }
        [Required(ErrorMessage = "La Duracion es obligatoria.")]
        public string Duracion { get; set; }
        public TipoClasificacion TipoClasificacion { get; set; }
        public int categoriaId { get; set; } 
    }
}
