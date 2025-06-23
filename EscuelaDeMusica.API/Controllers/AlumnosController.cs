using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EscuelaDeMusica.API.Models;
using EscuelaDeMusica.API.DTOs;

namespace EscuelaDeMusica.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AlumnosController : ControllerBase
{
    private readonly EscuelaDeMusicaContext _context;

    public AlumnosController(EscuelaDeMusicaContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Alumno>>> ListarAlumnos()
    {
        var alumnos = await _context.Alumnos
                .FromSqlRaw("EXEC usp_ListarAlumnos")
                .AsNoTracking()
                .ToListAsync();


        foreach (var alumno in alumnos)
        {
            alumno.Escuela = await _context.Escuelas.FindAsync(alumno.EscuelaId);
        }

        return alumnos;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AlumnoDetalleDTO>> AlumnoPorID(int id)
    {
        var alumno = await _context.Alumnos
            .Include(a => a.Escuela)
            .Include(a => a.Profesors)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (alumno == null) return NotFound();

        var dto = new AlumnoDetalleDTO
        {
            Id = alumno.Id,
            Nombre = alumno.Nombre,
            Apellido = alumno.Apellido,
            Identificacion = alumno.Identificacion,
            FechaNacimiento = alumno.FechaNacimiento.ToDateTime(TimeOnly.MinValue),
            Escuela = alumno.Escuela?.Nombre ?? "",
            Profesores = alumno.Profesors.Select(p => $"{p.Nombre} {p.Apellido}").ToList()
        };

        return dto;
    }

    [HttpPost]
    public async Task<ActionResult<Alumno>> InsertarAlumno(Alumno alumno)
    {
        if (await _context.Alumnos.AnyAsync(a => a.Identificacion == alumno.Identificacion))
            return Conflict("La identificación ya existe");
        
        try
        {
            // Ejecutar el SP de inserción
            await _context.Database.ExecuteSqlInterpolatedAsync($@"
            EXEC usp_InsertarAlumno 
                @Nombre={alumno.Nombre}, 
                @Apellido={alumno.Apellido}, 
                @FechaNacimiento={alumno.FechaNacimiento.ToDateTime(TimeOnly.MinValue)}, 
                @Identificacion={alumno.Identificacion}, 
                @EscuelaId={alumno.EscuelaId}
        ");
        }
        catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("FOREIGN KEY") == true)
        {
            return BadRequest("No se puede insertar el alumno porque la escuela indicada no existe.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error inesperado: {ex.Message}");
        }

        var creado = await _context.Alumnos.FirstOrDefaultAsync(a => a.Identificacion == alumno.Identificacion);
        return CreatedAtAction(nameof(AlumnoPorID), new { id = creado!.Id }, creado);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> EditarAlumno(int id, Alumno alumno)
    {
        if (!await _context.Alumnos.AnyAsync(a => a.Id == id))
            return NotFound();

        if (await _context.Alumnos.AnyAsync(a => a.Id != id && a.Identificacion == alumno.Identificacion))
            return Conflict("Ya existe otro alumno con esa identificación");

        await _context.Database.ExecuteSqlInterpolatedAsync($"EXEC usp_ActualizarAlumno @Id={id}, @Nombre={alumno.Nombre}, @Apellido={alumno.Apellido}, @FechaNacimiento={alumno.FechaNacimiento.ToDateTime(TimeOnly.MinValue)}, @Identificacion={alumno.Identificacion}, @EscuelaId={alumno.EscuelaId}");
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> EliminarAlumno(int id)
    {
        if (!await _context.Alumnos.AnyAsync(a => a.Id == id))
            return NotFound();

        await _context.Database.ExecuteSqlInterpolatedAsync($"EXEC usp_EliminarAlumno @Id={id}");
        return NoContent();
    }
}