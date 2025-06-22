using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EscuelaDeMusica.API.Models;

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
        public async Task<ActionResult<IEnumerable<Escuela>>> GetEscuelas()
        {
            return await _context.Escuelas.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Escuela>> GetEscuela(int id)
        {
            var escuela = await _context.Escuelas.FindAsync(id);
            if (escuela == null)
            {
                return NotFound();
            }
            return escuela;
        }

        [HttpPost]
        public async Task<ActionResult<Escuela>> CreateEscuela(Escuela escuela)
        {
            if (await _context.Escuelas.AnyAsync(e => e.Codigo == escuela.Codigo))
            {
                return Conflict("El codigo ya existe");
            }
            _context.Escuelas.Add(escuela);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetEscuela), new { id = escuela.Id }, escuela);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEscuela(int id, Escuela escuela)
        {
            var escuelaExistente = await _context.Escuelas.FindAsync(id);
            if (escuelaExistente == null) return NotFound();

            // Asignar el ID al objeto recibido (por seguridad)
            escuela.Id = id;

            // Validar código único
            if (await _context.Escuelas.AnyAsync(e => e.Id != id && e.Codigo == escuela.Codigo))
                return Conflict("Ya existe otra escuela con ese código.");

            _context.Entry(escuelaExistente).CurrentValues.SetValues(escuela);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEscuela(int id)
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
    }
}