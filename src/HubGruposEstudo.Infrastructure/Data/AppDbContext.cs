using HubGruposEstudo.Domain.Entities;
using HubGruposEstudo.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HubGruposEstudo.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Habilidade> Habilidades { get; set; }
    public DbSet<Trilha> Trilhas { get; set; }
    public DbSet<Etapa> Etapas { get; set; }
    public DbSet<Inscricao> Inscricoes { get; set; }
    public DbSet<Troca> Trocas { get; set; }
    public DbSet<Conquista> Conquistas { get; set; }
    public DbSet<ConquistaUsuario> ConquistaUsuarios { get; set; }
    public DbSet<UsuarioHabilidade> UsuarioHabilidades { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("usuarios");
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Nome).IsRequired().HasMaxLength(100);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(150);
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.SenhaHash).IsRequired();
            entity.Property(u => u.Perfil).IsRequired().HasConversion<string>();
            entity.Property(u => u.Bio).HasMaxLength(500);
            entity.Property(u => u.Nivel).HasDefaultValue(1);
            entity.Property(u => u.Xp).HasDefaultValue(0);
            entity.Property(u => u.CriadoEm).IsRequired();
        });

        modelBuilder.Entity<Habilidade>(entity =>
        {
            entity.ToTable("habilidades");
            entity.HasKey(h => h.Id);
            entity.Property(h => h.Nome).IsRequired().HasMaxLength(100);
            entity.HasIndex(h => h.Nome).IsUnique();
            entity.Property(h => h.Categoria).IsRequired().HasMaxLength(50);
        });

        modelBuilder.Entity<Trilha>(entity =>
        {
            entity.ToTable("trilhas");
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Titulo).IsRequired().HasMaxLength(200);
            entity.Property(t => t.Descricao).IsRequired().HasMaxLength(2000);
            entity.Property(t => t.NivelDificuldade).IsRequired().HasConversion<string>();
            entity.Property(t => t.CriadoEm).IsRequired();

            entity.HasOne(t => t.Criador)
                  .WithMany(u => u.TrilhasCriadas)
                  .HasForeignKey(t => t.CriadorId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(t => t.Habilidade)
                  .WithMany()
                  .HasForeignKey(t => t.HabilidadeId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(t => t.Etapas)
                  .WithOne(e => e.Trilha)
                  .HasForeignKey(e => e.TrilhaId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Etapa>(entity =>
        {
            entity.ToTable("etapas");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Titulo).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Descricao).IsRequired().HasMaxLength(2000);
            entity.Property(e => e.Tipo).IsRequired().HasConversion<string>();
            entity.Property(e => e.DuracaoEstimadaMin).IsRequired();
        });

        modelBuilder.Entity<Inscricao>(entity =>
        {
            entity.ToTable("inscricoes");
            entity.HasKey(i => i.Id);
            entity.Property(i => i.EtapaAtual).HasDefaultValue(1);
            entity.Property(i => i.Concluida).HasDefaultValue(false);
            entity.Property(i => i.DataInicio).IsRequired();

            entity.HasOne(i => i.Usuario)
                  .WithMany(u => u.Inscricoes)
                  .HasForeignKey(i => i.UsuarioId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(i => i.Trilha)
                  .WithMany()
                  .HasForeignKey(i => i.TrilhaId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(i => new { i.UsuarioId, i.TrilhaId }).IsUnique();
        });

        modelBuilder.Entity<Troca>(entity =>
        {
            entity.ToTable("trocas");
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Status).IsRequired().HasConversion<string>();
            entity.Property(t => t.CriadoEm).IsRequired();

            entity.HasOne(t => t.UsuarioA)
                  .WithMany()
                  .HasForeignKey(t => t.UsuarioAId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(t => t.UsuarioB)
                  .WithMany()
                  .HasForeignKey(t => t.UsuarioBId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(t => t.HabilidadeA)
                  .WithMany()
                  .HasForeignKey(t => t.HabilidadeAId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(t => t.HabilidadeB)
                  .WithMany()
                  .HasForeignKey(t => t.HabilidadeBId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(t => t.TrilhaA)
                  .WithMany()
                  .HasForeignKey(t => t.TrilhaAId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(t => t.TrilhaB)
                  .WithMany()
                  .HasForeignKey(t => t.TrilhaBId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Conquista>(entity =>
        {
            entity.ToTable("conquistas");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Nome).IsRequired().HasMaxLength(100);
            entity.Property(c => c.Descricao).IsRequired().HasMaxLength(500);
            entity.Property(c => c.Icone).IsRequired().HasMaxLength(200);
            entity.Property(c => c.Criterio).IsRequired().HasMaxLength(200);
        });

        modelBuilder.Entity<ConquistaUsuario>(entity =>
        {
            entity.ToTable("conquistas_usuarios");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.DataObtida).IsRequired();

            entity.HasOne(c => c.Usuario)
                  .WithMany(u => u.Conquistas)
                  .HasForeignKey(c => c.UsuarioId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(c => c.Conquista)
                  .WithMany()
                  .HasForeignKey(c => c.ConquistaId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<UsuarioHabilidade>(entity =>
        {
            entity.ToTable("usuarios_habilidades");
            entity.HasKey(uh => new { uh.UsuarioId, uh.HabilidadeId });

            entity.HasOne(uh => uh.Usuario)
                  .WithMany(u => u.Habilidades)
                  .HasForeignKey(uh => uh.UsuarioId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(uh => uh.Habilidade)
                  .WithMany()
                  .HasForeignKey(uh => uh.HabilidadeId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
