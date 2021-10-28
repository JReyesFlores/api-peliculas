﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Models
{
    public enum TipoClasificacion
    {
        Siete, Trece, Dieciseis, Dieciocho
    }

    public class Pelicula
    {
        [Key]
        public int Id { get; set; }
        [StringLength(150)]
        public string Nombre { get; set; }
        public string RutaImagen { get; set; }
        public string Descripcion { get; set; }
        public TipoClasificacion TipoClasificacion { get; set; }
        [StringLength(50)]
        public string Duracion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int categoriaId { get; set; }
        [ForeignKey("categoriaId")]
        public Categoria Categoria { get; set; }
        public DateTime? FechaModificacion { get; set; }
    }
}
