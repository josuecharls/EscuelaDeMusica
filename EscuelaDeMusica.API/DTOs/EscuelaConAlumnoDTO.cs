namespace EscuelaDeMusica.API.DTOs
{
    public class EscuelaConAlumnoDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
        public int AlumnoId { get; set; }
        public string AlumnoNombre { get; set; } = null!;
    }
}
