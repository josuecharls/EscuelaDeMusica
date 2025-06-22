using System;
using System.Collections.Generic;

namespace EscuelaDeMusica.API.Models;

public partial class Alumno
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellido { get; set; } = null!;

    public DateOnly FechaNacimiento { get; set; }

    public string Identificacion { get; set; } = null!;

    public int EscuelaId { get; set; }

    public virtual Escuela Escuela { get; set; } = null!;

    public virtual ICollection<Profesore> Profesors { get; set; } = new List<Profesore>();
}
