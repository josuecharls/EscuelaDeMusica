namespace EscuelaDeMusica.API.DTOs
{
    public class ProfesorDetalleDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string Identificacion { get; set; } = null!;
        public int EscuelaId { get; set; }
        public string EscuelaNombre { get; set; } = null!;
        public List<AlumnosProfDto> Alumnos { get; set; } = new();
    }

    public class AlumnosProfDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
    }
}
