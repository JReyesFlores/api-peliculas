using ApiPeliculas.Models;
using ApiPeliculas.Models.DTOs;
using ApiPeliculas.Repositorio.IRepositorio;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "APICategorias")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class CategoriasController : ControllerBase
    {
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly IMapper _mapper;
        public CategoriasController(ICategoriaRepository categoriaRepository, IMapper mapper)
        {
            this._categoriaRepository = categoriaRepository;
            this._mapper = mapper;
        }

        /// <summary>
        /// Obtener todas las categorias.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(List<CategoriaDTO>))]
        [ProducesResponseType(400)]
        public IActionResult GetCategorias()
        {
            //throw new Exception("Error generado");
            var listaCategorias = this._categoriaRepository.GetCategorias();
            var listaCategoriasDTO = new List<CategoriaDTO>();
            foreach (var item in listaCategorias)
            {
                listaCategoriasDTO.Add(this._mapper.Map<CategoriaDTO>(item));
            }

            return Ok(listaCategoriasDTO);
        }

        /// <summary>
        /// Obtener una categoria individual.
        /// </summary>
        /// <param name="categoriaId">Este es el Id de la categoría.</param>
        /// <returns></returns>
        [HttpGet("{categoriaId:int}", Name = "GetCategoria")]
        [AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(List<CategoriaDTO>))]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public IActionResult GetCategoria(int categoriaId)
        {
            var itemCategoria = this._categoriaRepository.GetCategoria(categoriaId);
            if (itemCategoria == null) return NotFound();

            var itemCategoriaDTO = this._mapper.Map<CategoriaDTO>(itemCategoria);
            return Ok(itemCategoriaDTO);
        }

        /// <summary>
        /// Crear una nueva categoría
        /// </summary>
        /// <param name="categoriaDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(List<CategoriaDTO>))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public IActionResult CrearCategoria([FromBody] CategoriaDTO categoriaDTO)
        {
            if (categoriaDTO == null) return BadRequest(ModelState);
            if (this._categoriaRepository.ExisteCategoria(categoriaDTO.Nombre))
            {
                ModelState.AddModelError("", "La categoría ya existe.");
                return StatusCode(404, ModelState);
            }

            var categoria = this._mapper.Map<Categoria>(categoriaDTO);
            if (!this._categoriaRepository.CrearCategoria(categoria))
            {
                ModelState.AddModelError("", $"Algo salio mal guardando el registro {categoriaDTO.Nombre}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetCategoria", new { categoriaId = categoria.Id }, categoria);
        }

        /// <summary>
        /// Actualizar una categoría existente.
        /// </summary>
        /// <param name="categoriaId"></param>
        /// <param name="categoriaDTO"></param>
        /// <returns></returns>
        [HttpPatch("{categoriaId:int}", Name = "ActualizarCategoria")]
        [ProducesResponseType(201, Type = typeof(List<CategoriaDTO>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] 
        public IActionResult ActualizarCategoria(int categoriaId, [FromBody] CategoriaDTO categoriaDTO)
        {
            if (categoriaDTO == null || categoriaId != categoriaDTO.Id) return BadRequest(ModelState);
            var categoria = this._mapper.Map<Categoria>(categoriaDTO);

            if (!this._categoriaRepository.ActualizarCategoria(categoria))
            {
                ModelState.AddModelError("", $"Algo salio mal actualizando el registro {categoriaDTO.Nombre}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        /// <summary>
        /// Borrar una categoría existente.
        /// </summary>
        /// <param name="categoriaId"></param>
        /// <returns></returns>
        [HttpDelete("{categoriaId:int}", Name = "EliminarCategoria")]
        [ProducesResponseType(201, Type = typeof(List<CategoriaDTO>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public IActionResult EliminarCategoria(int categoriaId)
        {
            if (!this._categoriaRepository.ExisteCategoria(categoriaId)) return NotFound();

            var categoria = this._categoriaRepository.GetCategoria(categoriaId);
            if (!this._categoriaRepository.BorrarCategoria(categoria))
            {
                ModelState.AddModelError("", $"Algo salio mal borrando el registro {categoria.Nombre}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
