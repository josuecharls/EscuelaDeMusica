using EscuelaDeMusica.API.DTOs;
using EscuelaDeMusica.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

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
        public async Task<ActionResult<IEnumerable<Profesore>>> ListarProfesores()
        {
            var profesores = await _context.Profesores
                .FromSqlRaw("EXEC usp_ListarProfesores")
                .AsNoTracking()
                .ToListAsync();

            foreach (var profe in profesores)
            {
                profe.Escuela = await _context.Escuelas.FindAsync(profe.EscuelaId);
            }

            return profesores;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProfesorDetalleDTO>> ProfesorPorID(int id)
        {
            var profesor = await _context.Profesores
                .Include(p => p.Escuela)
                .Include(p => p.Alumnos)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (profesor == null)
                return NotFound();

            var dto = new ProfesorDetalleDTO
            {
                Id = profesor.Id,
                Nombre = profesor.Nombre,
                Apellido = profesor.Apellido,
                Identificacion = profesor.Identificacion,
                EscuelaId = profesor.EscuelaId,
                EscuelaNombre = profesor.Escuela?.Nombre ?? "",
                Alumnos = profesor.Alumnos.Select(p => new AlumnosProfDto
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Apellido = p.Apellido
                }).ToList(),
            };
            return dto;
        }

        [HttpPost]
        public async Task<ActionResult<Profesore>> InsertarProfesor(Profesore profesor)
        {
            try
            {
                if (!await _context.Escuelas.AnyAsync(e => e.Id == profesor.EscuelaId))
                    return BadRequest(new { mensaje = "La escuela asignada no existe." });

                if (await _context.Profesores.AnyAsync(p => p.Identificacion == profesor.Identificacion))
                    return Conflict(new { mensaje = "El número de identificación ya existe." });

                // Usando un procedimiento almacenado para insertar el profesor
                await _context.Database.ExecuteSqlInterpolatedAsync($"EXEC usp_InsertarProfesor @Nombre={profesor.Nombre}, @Apellido={profesor.Apellido}, @Identificacion={profesor.Identificacion}, @EscuelaId={profesor.EscuelaId}");

                var creado = await _context.Profesores.FirstOrDefaultAsync(p => p.Identificacion == profesor.Identificacion);

                return CreatedAtAction(nameof(InsertarProfesor), new { id = creado!.Id }, creado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al registrar el profesor.", detalle = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditarProfesor(int id, Profesore profesor)
        {
            if (id != profesor.Id)
                return BadRequest(new { mensaje = "El ID enviado no coincide con el recurso." });

            try
            {
                if (!await _context.Escuelas.AnyAsync(e => e.Id == profesor.EscuelaId))
                    return BadRequest(new { mensaje = "La escuela asignada no existe." });

                if (await _context.Profesores.AnyAsync(p => p.Id != id && p.Identificacion == profesor.Identificacion))
                    return Conflict(new { mensaje = "Ya existe otro profesor con esa identificación." });
                // Usando un procedimiento almacenado para actualizar el profesor
                await _context.Database.ExecuteSqlInterpolatedAsync($"EXEC usp_ActualizarProfesor @Id={id}, @Nombre={profesor.Nombre}, @Apellido={profesor.Apellido}, @Identificacion={profesor.Identificacion}, @EscuelaId={profesor.EscuelaId}");

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
        public async Task<IActionResult> EliminarProfesor(int id)
        {
            if (!await _context.Profesores.AnyAsync(p => p.Id == id))
                return NotFound(new { mensaje = "Profesor no encontrado." });

            await _context.Database.ExecuteSqlInterpolatedAsync($"EXEC usp_EliminarProfesor @Id={id}");

            return NoContent();
        }

        [HttpPost("{id}/alumnos")]
        public async Task<IActionResult> AsignarAlumnos(int id, List<int> alumnoIds)
        {
            var profesor = await _context.Profesores.Include(p => p.Alumnos).FirstOrDefaultAsync(p => p.Id == id);
            if (profesor == null)
                return NotFound(new { mensaje = "Profesor no encontrado." });

            var alumnos = await _context.Alumnos
                .Where(a => alumnoIds.Contains(a.Id))
                .ToListAsync();

            if (alumnos.Count != alumnoIds.Count)
                return BadRequest(new { mensaje = "Uno o más alumnos no existen." });

            if (alumnos.Any(a => a.EscuelaId != profesor.EscuelaId))
                return BadRequest(new { mensaje = "Todos los alumnos deben pertenecer a la misma escuela que el profesor." });

            foreach (var alumno in alumnos)
            {
                if (!profesor.Alumnos.Any(a => a.Id == alumno.Id))
                    profesor.Alumnos.Add(alumno);
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("{id}/alumnos")]
        public async Task<ActionResult<IEnumerable<AlumnoConEscuelaDTO>>> GetAlumnosPorProfesor(int id)
        {
            var resultado = new List<AlumnoConEscuelaDTO>();
            using var connection = _context.Database.GetDbConnection();
            await connection.OpenAsync();
            using var command = connection.CreateCommand();
            command.CommandText = "usp_AlumnosPorProfesor";
            command.CommandType = System.Data.CommandType.StoredProcedure;
            var param = command.CreateParameter();
            param.ParameterName = "@ProfesorId";
            param.Value = id;
            command.Parameters.Add(param);
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                resultado.Add(new AlumnoConEscuelaDTO
                {
                    Id = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    Apellido = reader.GetString(2),
                    Escuela = reader.GetString(3)
                });
            }
            return resultado;
        }

        [HttpGet("{id}/escuela-alumnos")]
        public async Task<ActionResult<IEnumerable<EscuelaConAlumnoDTO>>> GetEscuelaYAlumnos(int id)
        {
            var resultado = new List<EscuelaConAlumnoDTO>();
            using var connection = _context.Database.GetDbConnection();
            await connection.OpenAsync();
            using var command = connection.CreateCommand();
            command.CommandText = "usp_EscuelaYAlumnosPorProfesor";
            command.CommandType = System.Data.CommandType.StoredProcedure;
            var param = command.CreateParameter();
            param.ParameterName = "@ProfesorId";
            param.Value = id;
            command.Parameters.Add(param);
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                resultado.Add(new EscuelaConAlumnoDTO
                {
                    Id = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    Descripcion = reader.GetString(2),
                    AlumnoId = reader.GetInt32(3),
                    AlumnoNombre = reader.GetString(4)
                });
            }
            return resultado;
        }
    }
}