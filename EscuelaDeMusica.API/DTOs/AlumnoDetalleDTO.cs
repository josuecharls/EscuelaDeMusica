namespace EscuelaDeMusica.API.DTOs
{
    public class AlumnoDetalleDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string Identificacion { get; set; } = null!;
        public DateTime FechaNacimiento { get; set; }
        public string Escuela { get; set; } = null!;
        public List<string> Profesores { get; set; } = new();
    }
}
