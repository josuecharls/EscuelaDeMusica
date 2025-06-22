using System;
using System.Collections.Generic;

namespace EscuelaDeMusica.API.Models;

public partial class Escuela
{
    public int Id { get; set; }

    public string Codigo { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public virtual ICollection<Alumno> Alumnos { get; set; } = new List<Alumno>();

    public virtual ICollection<Profesore> Profesores { get; set; } = new List<Profesore>();
}
