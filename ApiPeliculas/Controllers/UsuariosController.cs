using ApiPeliculas.Models;
using ApiPeliculas.Models.DTOs;
using ApiUsuarios.Repositorio.IRepositorio;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ApiPeliculas.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiExplorerSettings(GroupName = "APIUsuarios")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UsuariosController(IUsuarioRepository usuarioRepository,
                                  IMapper mapper,
                                  IConfiguration configuration)
        {
            this._usuarioRepository = usuarioRepository;
            this._mapper = mapper;
            this._configuration = configuration;
        }

        [HttpGet]
        public IActionResult GetUsuarios()
        {
            var listaUsuarios = this._usuarioRepository.GetUsuarios();
            var listaUsuariosDTO = new List<UsuarioDTO>();
            foreach (var item in listaUsuarios)
            {
                listaUsuariosDTO.Add(this._mapper.Map<UsuarioDTO>(item));
            }

            return Ok(listaUsuariosDTO);
        }

        [HttpGet("{usuarioId:int}", Name = "GetUsuario")]
        [AllowAnonymous]
        public IActionResult GetUsuario(int usuarioId)
        {
            var usuario = this._usuarioRepository.GetUsuario(usuarioId);
            if (usuario == null) return NotFound();

            var userDTO = this._mapper.Map<UsuarioDTO>(usuario);
            return Ok(userDTO);
        }

        [HttpPost("registro")]
        public IActionResult Registro(UsuarioAuthDTO usuarioAuthDto)
        {
            usuarioAuthDto.Usuario = usuarioAuthDto.Usuario.ToLower();
            if (this._usuarioRepository.ExisteUsuario(usuarioAuthDto.Usuario)) return BadRequest("El usuario ya existe");

            var usuarioCrear = new Usuario
            {
                UsuarioA = usuarioAuthDto.Usuario
            };

            var usuarioCreado = this._usuarioRepository.Registro(usuarioCrear, usuarioAuthDto.Password);
            return Ok(usuarioCreado);
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public IActionResult Login(UsuarioLoginDTO usuarioLoginDTO)
        {
            //try
            //{
            //throw new Exception("Error generado");
            var usuarioDesdeRepo = this._usuarioRepository.Login(usuarioLoginDTO.Usuario, usuarioLoginDTO.Password);
            if (usuarioDesdeRepo == null) return Unauthorized();

            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, usuarioDesdeRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, usuarioDesdeRepo.UsuarioA.ToString())
            };

            //Generacion Token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this._configuration.GetSection("AppSettings:Token").Value));
            var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credenciales
            };

            var tokenhandler = new JwtSecurityTokenHandler();
            var token = tokenhandler.CreateToken(tokenDescriptor);

            return Ok(new
            {
                token = tokenhandler.WriteToken(token)
            });
            //}
            //catch (Exception)
            //{
            //    return StatusCode(500, "Error generado!");
            //}

        }
    }
}
