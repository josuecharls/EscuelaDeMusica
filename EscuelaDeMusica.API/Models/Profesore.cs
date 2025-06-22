using System;
using System.Collections.Generic;

namespace EscuelaDeMusica.API.Models;

public partial class Profesore
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellido { get; set; } = null!;

    public string Identificacion { get; set; } = null!;

    public int EscuelaId { get; set; }

    public virtual Escuela Escuela { get; set; } = null!;

    public virtual ICollection<Alumno> Alumnos { get; set; } = new List<Alumno>();
}
