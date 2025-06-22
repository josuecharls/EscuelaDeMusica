using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace EscuelaDeMusica.API.Models;

public partial class EscuelaDeMusicaContext : DbContext
{
    public EscuelaDeMusicaContext()
    {
    }

    public EscuelaDeMusicaContext(DbContextOptions<EscuelaDeMusicaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Alumno> Alumnos { get; set; }

    public virtual DbSet<Escuela> Escuelas { get; set; }

    public virtual DbSet<Profesore> Profesores { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=EscuelaDeMusica;User Id=sa;Password=4pfzqo0yE@;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Alumno>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Alumnos__3214EC07D4A26F57");

            entity.HasIndex(e => e.Identificacion, "UQ__Alumnos__D6F931E5BDFFACD3").IsUnique();

            entity.Property(e => e.Apellido).HasMaxLength(100);
            entity.Property(e => e.Identificacion).HasMaxLength(20);
            entity.Property(e => e.Nombre).HasMaxLength(100);

            entity.HasOne(d => d.Escuela).WithMany(p => p.Alumnos)
                .HasForeignKey(d => d.EscuelaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Alumnos__Escuela__3F466844");

            entity.HasMany(d => d.Profesors).WithMany(p => p.Alumnos)
                .UsingEntity<Dictionary<string, object>>(
                    "AlumnoProfesor",
                    r => r.HasOne<Profesore>().WithMany()
                        .HasForeignKey("ProfesorId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__AlumnoPro__Profe__4316F928"),
                    l => l.HasOne<Alumno>().WithMany()
                        .HasForeignKey("AlumnoId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__AlumnoPro__Alumn__4222D4EF"),
                    j =>
                    {
                        j.HasKey("AlumnoId", "ProfesorId").HasName("PK__AlumnoPr__0479951F77728612");
                        j.ToTable("AlumnoProfesor");
                    });
        });

        modelBuilder.Entity<Escuela>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Escuelas__3214EC07207E5E24");

            entity.HasIndex(e => e.Codigo, "UQ__Escuelas__06370DACFDB85B83").IsUnique();

            entity.Property(e => e.Codigo).HasMaxLength(10);
            entity.Property(e => e.Descripcion).HasMaxLength(250);
            entity.Property(e => e.Nombre).HasMaxLength(100);
        });

        modelBuilder.Entity<Profesore>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Profesor__3214EC0739980112");

            entity.HasIndex(e => e.Identificacion, "UQ__Profesor__D6F931E54094111C").IsUnique();

            entity.Property(e => e.Apellido).HasMaxLength(100);
            entity.Property(e => e.Identificacion).HasMaxLength(20);
            entity.Property(e => e.Nombre).HasMaxLength(100);

            entity.HasOne(d => d.Escuela).WithMany(p => p.Profesores)
                .HasForeignKey(d => d.EscuelaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Profesore__Escue__3B75D760");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
