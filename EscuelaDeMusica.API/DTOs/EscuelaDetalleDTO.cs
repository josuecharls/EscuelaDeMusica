namespace EscuelaDeMusica.API.DTOs
{
    public class EscuelaDetalleDTO
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
        public List<AlumnoDTO> Alumnos { get; set; } = new();
        public List<ProfesorDTO> Profesores { get; set; } = new();
    }

    public class AlumnoDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string Identificacion { get; set; } = null!;
    }

    public class ProfesorDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string Identificacion { get; set; } = null!;
    }
}