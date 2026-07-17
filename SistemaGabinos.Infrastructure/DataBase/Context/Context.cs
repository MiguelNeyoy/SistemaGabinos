using Microsoft.EntityFrameworkCore;
using SistemaGabinos.Domain.Entities;

namespace SistemaGabinos.Infrastructure.DataBase.Context;

public class SistemaGabinosDBContext : DbContext
{
    public DbSet<Alumno> Alumnos => Set<Alumno>();
    public DbSet<Curso> Cursos => Set<Curso>();
    public DbSet<Inscripcion> Inscripciones => Set<Inscripcion>();
    public DbSet<Deuda> Deudas => Set<Deuda>();
    public DbSet<Pago> Pagos => Set<Pago>();
    public DbSet<Recibo> Recibos => Set<Recibo>();

    public SistemaGabinosDBContext(DbContextOptions<SistemaGabinosDBContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Alumno>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.NombreCompleto).IsRequired();
            entity.Property(a => a.CURP).IsRequired().HasMaxLength(18);
            entity.HasIndex(a => a.CURP).IsUnique();
            entity.Property(a => a.Telefono).IsRequired();
            entity.Property(a => a.FechaRegistro).IsRequired();
            entity.Property(a => a.Estado).IsRequired().HasConversion<string>();
        });

        modelBuilder.Entity<Curso>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Nombre).IsRequired();
            entity.Property(c => c.PrecioLibro).IsRequired().HasColumnType("decimal(18,2)");
            entity.HasData(
                new Curso(1, "Book 1", 350m),
                new Curso(2, "Book 2", 350m),
                new Curso(3, "Book 3", 350m),
                new Curso(4, "Book 4", 350m),
                new Curso(5, "Book 5", 350m),
                new Curso(6, "Book 6", 350m));
        });

        modelBuilder.Entity<Inscripcion>(entity =>
        {
            entity.HasKey(i => i.Id);
            entity.Property(i => i.FechaInscripcion).IsRequired();
            entity.Property(i => i.Estado).IsRequired().HasConversion<string>();
            entity.HasIndex(i => new { i.AlumnoId, i.CursoId }).IsUnique();
        });

        modelBuilder.Entity<Deuda>(entity =>
        {
            entity.HasKey(d => d.Id);
            entity.Property(d => d.MontoTotal).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(d => d.MontoPagado).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(d => d.Concepto).IsRequired().HasConversion<string>();
            entity.Property(d => d.FechaCreacion).IsRequired();
            entity.Ignore(d => d.EstaPagada);
        });

        modelBuilder.Entity<Pago>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Monto).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(p => p.Fecha).IsRequired();
            entity.Property(p => p.Concepto).IsRequired().HasConversion<string>();
            entity.Property(p => p.MetodoPago).IsRequired().HasConversion<string>();
            entity.Property(p => p.EstaCancelado).IsRequired();
        });

        modelBuilder.Entity<Recibo>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Folio).IsRequired();
            entity.HasIndex(r => r.Folio).IsUnique();
            entity.Property(r => r.Detalle).IsRequired();
            entity.Property(r => r.Monto).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(r => r.FechaEmision).IsRequired();
        });
    }
}
