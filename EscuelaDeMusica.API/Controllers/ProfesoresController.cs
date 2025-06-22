using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EscuelaDeMusica.API.Models;

namespace EscuelaDeMusica.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfesoresController : ControllerBase
    {
        private readonly EscuelaDeMusicaContext _context;

        public ProfesoresController(EscuelaDeMusicaContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Profesore>>> GetProfesores()
        {
            return await _context.Profesores.Include(p => p.Escuela).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Profesore>> GetProfesor(int id)
        {
            var profesor = await _context.Profesores
                .Include(p => p.Escuela)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (profesor == null)
                return NotFound(new { mensaje = "Profesor no encontrado." });

            return profesor;
        }

        [HttpPost]
        public async Task<ActionResult<Profesore>> PostProfesor(Profesore profesor)
        {
            try
            {
                if (!await _context.Escuelas.AnyAsync(e => e.Id == profesor.EscuelaId))
                    return BadRequest(new { mensaje = "La escuela asignada no existe." });

                if (await _context.Profesores.AnyAsync(p => p.Identificacion == profesor.Identificacion))
                    return Conflict(new { mensaje = "El número de identificación ya existe." });

                _context.Profesores.Add(profesor);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetProfesor), new { id = profesor.Id }, profesor);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al registrar el profesor.", detalle = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProfesor(int id, Profesore profesor)
        {
            if (id != profesor.Id)
                return BadRequest(new { mensaje = "El ID enviado no coincide con el recurso." });

            try
            {
                if (!await _context.Escuelas.AnyAsync(e => e.Id == profesor.EscuelaId))
                    return BadRequest(new { mensaje = "La escuela asignada no existe." });

                if (await _context.Profesores.AnyAsync(p => p.Id != id && p.Identificacion == profesor.Identificacion))
                    return Conflict(new { mensaje = "Ya existe otro profesor con esa identificación." });

                _context.Entry(profesor).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Profesores.Any(p => p.Id == id))
                    return NotFound(new { mensaje = "Profesor no encontrado." });

                throw;
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al actualizar el profesor.", detalle = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProfesor(int id)
        {
            var profesor = await _context.Profesores.FindAsync(id);
            if (profesor == null)
                return NotFound(new { mensaje = "Profesor no encontrado." });

            _context.Profesores.Remove(profesor);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}