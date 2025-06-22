namespace EscuelaDeMusica.API.DTOs
{
    public class AlumnoConEscuelaDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string Escuela { get; set; } = null!;
    }
}
