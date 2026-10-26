using ISIRUN.Models;
using Microsoft.EntityFrameworkCore;

namespace ISIRUN.Data;

public class IsirunContext(DbContextOptions<IsirunContext> options) : DbContext(options)
{
    public DbSet<Corredor> Corredores => Set<Corredor>();
    public DbSet<Equipo> Equipos => Set<Equipo>();
    public DbSet<Carrera> Carreras => Set<Carrera>();
    public DbSet<CategoriaEdad> CategoriasEdad => Set<CategoriaEdad>();
    public DbSet<Resultado> Resultados => Set<Resultado>();
    public DbSet<Organizacion> Organizaciones => Set<Organizacion>();
    public DbSet<OrgCarrera> OrgCarreras => Set<OrgCarrera>();
    public DbSet<Sponsor> Sponsors => Set<Sponsor>();
    public DbSet<Actividad> Actividades => Set<Actividad>();
    public DbSet<LikeActividad> LikesActividad => Set<LikeActividad>();
    public DbSet<Amistad> Amistades => Set<Amistad>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Resultado>().HasKey(r => new { r.IdCorredor, r.IdCarrera });
        b.Entity<OrgCarrera>().HasKey(o => new { o.IdCarrera, o.IdOrganizacion });
        b.Entity<Sponsor>().HasKey(s => new { s.IdCarrera, s.IdOrganizacion });
        b.Entity<LikeActividad>().HasKey(l => new { l.IdActividad, l.IdCorredor });
        b.Entity<Amistad>().HasKey(a => new { a.IdCorredor, a.IdAmigo });

        b.Entity<Corredor>().HasIndex(c => c.Email).IsUnique();
        b.Entity<Organizacion>().HasIndex(o => o.Email).IsUnique();

        // Evita dos caminos de borrado en cascada desde Carrera hasta Resultado.
        b.Entity<Resultado>().HasOne(r => r.Categoria).WithMany(c => c.Resultados)
            .HasForeignKey(r => r.IdCategoria).OnDelete(DeleteBehavior.NoAction);

        // El corredor ya llega a los likes mediante sus actividades.
        b.Entity<LikeActividad>().HasOne(l => l.Corredor).WithMany()
            .HasForeignKey(l => l.IdCorredor).OnDelete(DeleteBehavior.NoAction);

        // Ambas relaciones apuntan a Corredor, pero representan personas distintas.
        b.Entity<Amistad>().HasOne(a => a.Corredor).WithMany(c => c.Amigos)
            .HasForeignKey(a => a.IdCorredor).OnDelete(DeleteBehavior.Cascade);
        b.Entity<Amistad>().HasOne(a => a.Amigo).WithMany()
            .HasForeignKey(a => a.IdAmigo).OnDelete(DeleteBehavior.NoAction);

        b.Entity<Corredor>().HasOne(c => c.Equipo).WithMany(e => e.Corredores)
            .HasForeignKey(c => c.IdEquipo).OnDelete(DeleteBehavior.SetNull);

        b.Entity<CategoriaEdad>().ToTable(t => t.HasCheckConstraint(
            "CK_CategoriasEdad_Rango", "[EdadMinima] >= 0 AND [EdadMinima] <= [EdadMaxima]"));
        b.Entity<Amistad>().ToTable(t => t.HasCheckConstraint(
            "CK_Amistades_PersonasDistintas", "[IdCorredor] <> [IdAmigo]"));
    }
}
