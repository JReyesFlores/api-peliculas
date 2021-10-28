using ApiPeliculas.Data;
using ApiPeliculas.Helpers;
using ApiPeliculas.PeliculasMapper;
using ApiPeliculas.Repositorio;
using ApiPeliculas.Repositorio.IRepositorio;
using ApiUsuarios.Repositorio.IRepositorio;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;

namespace ApiPeliculas
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Establecemos la conexión
            services.AddDbContext<ApplicationDbContext>(
                options =>
                {
                    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
                }
                );
            //Agregando dependencias del token
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            //Inyectando las interfaces
            services.AddScoped<ICategoriaRepository, CategoriaRepository>();
            services.AddScoped<IPeliculaRepository, PeliculaRepository>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();

            //Agregando los mapeados
            services.AddAutoMapper(typeof(PeliculasMappers));

            //Habilitar la documentación de nuestra API
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("APICategorias", new OpenApiInfo()
                {
                    Title = "API Categorias",
                    Version = "1",
                    Description = "Backend Categorias",
                    Contact = new OpenApiContact()
                    {
                        Email = "jhon.seyer17@gmail.com",
                        Name = "Jhon Phileppe Reyes Flores",
                        Url = new Uri("https://www.openserver.com.pe")
                    },
                    License = new OpenApiLicense()
                    {
                        Name = "MIT License",
                        Url = new Uri("https://www.openserver.com.pe")
                    }
                });

                options.SwaggerDoc("APIPeliculas", new OpenApiInfo()
                {
                    Title = "API Películas",
                    Version = "1",
                    Description = "Backend Películas",
                    Contact = new OpenApiContact()
                    {
                        Email = "jhon.seyer17@gmail.com",
                        Name = "Jhon Phileppe Reyes Flores",
                        Url = new Uri("https://www.openserver.com.pe")
                    },
                    License = new OpenApiLicense()
                    {
                        Name = "MIT License",
                        Url = new Uri("https://www.openserver.com.pe")
                    }
                });

                options.SwaggerDoc("APIUsuarios", new OpenApiInfo()
                {
                    Title = "API Usuarios",
                    Version = "1",
                    Description = "Backend Usuarios",
                    Contact = new OpenApiContact()
                    {
                        Email = "jhon.seyer17@gmail.com",
                        Name = "Jhon Phileppe Reyes Flores",
                        Url = new Uri("https://www.openserver.com.pe")
                    },
                    License = new OpenApiLicense()
                    {
                        Name = "MIT License",
                        Url = new Uri("https://www.openserver.com.pe")
                    }
                });
                var archivoXMLComentarios = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var rutaApiComentarios = Path.Combine(AppContext.BaseDirectory, archivoXMLComentarios);
                options.IncludeXmlComments(archivoXMLComentarios);

                //Primero definir el esquema de seguridad
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Description = "Autenticación JWT (Bearer)",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {
                        new OpenApiSecurityScheme {
                            Reference = new OpenApiReference
                            {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                            }
                        }, new List<string>()
                    }
                });
            });

            services.AddControllers();
            //Agregando los cors
            services.AddCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(builder =>
                {
                    builder.Run(async context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        var error = context.Features.Get<IExceptionHandlerFeature>();

                        if(error != null)
                        {
                            context.Response.AddAplicationError(error.Error.Message);
                            await context.Response.WriteAsync(error.Error.Message);
                        }
                    });
                });
            }

            app.UseHttpsRedirection();

            //Habilita las rutas necesarias para la documentación.
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/APICategorias/swagger.json", "API Categorias");
                options.SwaggerEndpoint("/swagger/APIPeliculas/swagger.json", "API Películas");
                options.SwaggerEndpoint("/swagger/APIUsuarios/swagger.json", "API Usuarios");
                options.RoutePrefix = "";
            });

            app.UseRouting();

            //Habilitando la autenticación y la autorización
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            //Brindando soporte para los cors
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
        }
    }
}
