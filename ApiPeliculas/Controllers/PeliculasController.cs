using ApiPeliculas.Models;
using ApiPeliculas.Models.DTOs;
using ApiPeliculas.Repositorio.IRepositorio;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiExplorerSettings(GroupName = "APIPeliculas")]
    [ApiController]
    public class PeliculasController : ControllerBase
    {
        private readonly IPeliculaRepository _peliculaRepository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public PeliculasController(IPeliculaRepository peliculaRepository,
                                   IMapper mapper,
                                   IWebHostEnvironment webHostEnvironment)
        {
            this._peliculaRepository = peliculaRepository;
            this._mapper = mapper;
            this._webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetPeliculas()
        {
            var listaPeliculas = this._peliculaRepository.GetPeliculas();
            var listaPeliculasDTO = new List<PeliculaDTO>();
            foreach (var item in listaPeliculas)
            {
                listaPeliculasDTO.Add(this._mapper.Map<PeliculaDTO>(item));
            }

            return Ok(listaPeliculasDTO);
        }

        [HttpGet("{peliculaId:int}", Name = "GetPelicula")]
        [AllowAnonymous]
        public IActionResult GetPelicula(int peliculaId)
        {
            var itemPeliculas = this._peliculaRepository.GetPelicula(peliculaId);
            if (itemPeliculas == null) return NotFound();

            var itemPeliculasDTO = this._mapper.Map<PeliculaDTO>(itemPeliculas);
            return Ok(itemPeliculasDTO);
        }

        [HttpPost]
        public IActionResult CrearPelicula([FromForm] PeliculaCreateDTO peliculaCreateDTO)
        {
            if (peliculaCreateDTO == null) return BadRequest(ModelState);
            if (this._peliculaRepository.ExistePelicula(peliculaCreateDTO.Nombre))
            {
                ModelState.AddModelError("", "La pelicula ya existe.");
                return StatusCode(404, ModelState);
            }

            /* Subida de archivos */
            var archivo = peliculaCreateDTO.Foto;
            string rutaPrincipal = this._webHostEnvironment.WebRootPath;
            var archivos = HttpContext.Request.Form.Files;
            if (archivo.Length > 0)
            {
                var nombreFoto = Guid.NewGuid().ToString();
                var subida = Path.Combine(rutaPrincipal, @"fotos");
                var extension = Path.GetExtension(archivos[0].FileName);
                using (var fileStream = new FileStream(Path.Combine(subida, nombreFoto + extension), FileMode.Create))
                {
                    archivos[0].CopyTo(fileStream);
                }
                peliculaCreateDTO.RutaImagen = @"\fotos\" + nombreFoto + extension;
            }
            /* Fin subida de archivos */

            var pelicula = this._mapper.Map<Pelicula>(peliculaCreateDTO);
            if (!this._peliculaRepository.CrearPelicula(pelicula))
            {
                ModelState.AddModelError("", $"Algo salio mal guardando el registro {peliculaCreateDTO.Nombre}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetPelicula", new { peliculaId = pelicula.Id }, pelicula);
        }

        [HttpGet("GetPeliculasEnCategoria/{categoriaId:int}", Name = "GetPeliculasEnCategoria")]
        [AllowAnonymous]
        public IActionResult GetPeliculasEnCategoria(int categoriaId)
        {
            var listaPelicula = this._peliculaRepository.GetPeliculasPorCategoria(categoriaId);
            if (listaPelicula == null) return NotFound();

            var itemPelicula = new List<PeliculaDTO>();
            foreach (var item in listaPelicula)
            {
                itemPelicula.Add(this._mapper.Map<PeliculaDTO>(item));
            }
            return Ok(itemPelicula);
        }

        [HttpGet("Buscar/{nombre}", Name = "Buscar")]
        [AllowAnonymous]
        public IActionResult Buscar(string nombre)
        {
            try
            {
                var resultado = this._peliculaRepository.BuscarPelicula(nombre);
                if (resultado.Any()) return Ok(resultado);

                return NotFound();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error recuperando datos de la aplicación.");
            }
        }

        [HttpPatch("{peliculaId:int}", Name = "ActualizarPelicula")]
        public IActionResult ActualizarPelicula(int peliculaId, [FromBody] PeliculaUpdateDTO peliculaUpdateDTO)
        {
            if (peliculaUpdateDTO == null || peliculaId != peliculaUpdateDTO.Id) return BadRequest(ModelState);
            var pelicula = this._mapper.Map<Pelicula>(peliculaUpdateDTO);

            if (!this._peliculaRepository.ActualizarPelicula(pelicula))
            {
                ModelState.AddModelError("", $"Algo salio mal actualizando el registro {peliculaUpdateDTO.Nombre}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{peliculaId:int}", Name = "EliminarPelicula")]
        public IActionResult EliminarPelicula(int peliculaId)
        {
            if (!this._peliculaRepository.ExistePelicula(peliculaId)) return NotFound();

            var pelicula = this._peliculaRepository.GetPelicula(peliculaId);
            if (!this._peliculaRepository.BorrarPelicula(pelicula))
            {
                ModelState.AddModelError("", $"Algo salio mal borrando el registro {pelicula.Nombre}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
