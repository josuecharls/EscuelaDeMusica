using EscuelaDeMusica.API.DTOs;
using EscuelaDeMusica.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EscuelaDeMusica.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EscuelasController : ControllerBase
    {
        private readonly EscuelaDeMusicaContext _context;

        public EscuelasController(EscuelaDeMusicaContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EscuelaDetalleDTO>>> ListarEscuelas()
        {
            try
            {
                var escuelas = await _context.Escuelas
                    .Select(e => new EscuelaDetalleDTO
                    {
                        Id = e.Id,
                        Codigo = e.Codigo,
                        Nombre = e.Nombre,
                        Descripcion = e.Descripcion
                    })
                    .ToListAsync();

                return escuelas;
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener las escuelas.", detalle = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EscuelaDetalleDTO>> EscuelaPorId(int id)
        {
            try
            {
                var escuela = await _context.Escuelas
                    .Include(e => e.Alumnos)
                    .Include(e => e.Profesores)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (escuela == null)
                    return NotFound();

                var dto = new EscuelaDetalleDTO
                {
                    Id = escuela.Id,
                    Codigo = escuela.Codigo,
                    Nombre = escuela.Nombre,
                    Descripcion = escuela.Descripcion,
                    Alumnos = escuela.Alumnos.Select(a => new AlumnoDTO
                    {
                        Id = a.Id,
                        Nombre = a.Nombre,
                        Apellido = a.Apellido,
                        Identificacion = a.Identificacion
                    }).ToList(),
                    Profesores = escuela.Profesores.Select(p => new ProfesorDTO
                    {
                        Id = p.Id,
                        Nombre = p.Nombre,
                        Apellido = p.Apellido,
                        Identificacion = p.Identificacion
                    }).ToList()
                };

                return dto;
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener la escuela.", detalle = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<Escuela>> InsertarEscuela(Escuela escuela)
        {
            try
            {
                if (await _context.Escuelas.AnyAsync(e => e.Codigo == escuela.Codigo))
                {
                    return Conflict("El codigo de esta Escuela ya existe");
                }
                _context.Escuelas.Add(escuela);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(EscuelaPorId), new { id = escuela.Id }, escuela);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al registrar la escuela.", detalle = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditarEscuela(int id, Escuela escuela)
        {
            try
            {
                var escuelaExistente = await _context.Escuelas.FindAsync(id);
                if (escuelaExistente == null) return NotFound();

                escuela.Id = id;

                if (await _context.Escuelas.AnyAsync(e => e.Id != id && e.Codigo == escuela.Codigo))
                    return Conflict("Ya existe otra escuela con ese código");

                _context.Entry(escuelaExistente).CurrentValues.SetValues(escuela);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al actualizar la escuela.", detalle = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarEscuela(int id)
        {
            try
            {
                var escuela = await _context.Escuelas.FindAsync(id);
                if (escuela == null)
                {
                    return NotFound();
                }

                _context.Escuelas.Remove(escuela);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al eliminar la escuela.", detalle = ex.Message });
            }
        }
    }
}