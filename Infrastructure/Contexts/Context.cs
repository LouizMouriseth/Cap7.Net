using System.Linq.Expressions;
using Core;
using Core.SeedWork;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Contexts;

public class Context : DbContext
{
    public Context(DbContextOptions<Context> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<User>(e =>
        {
            e.Property(p => p.Username)
                .HasMaxLength(160)
                .IsRequired();

            e.HasIndex(i => i.Email)
                .IsUnique();
            e.Property(p => p.Email)
                .HasMaxLength(320)
                .IsRequired();

            e.Property(p => p.Password)
                .HasMaxLength(255)
                .IsRequired();

            e.Property(p => p.Role)
                .IsRequired();
        });

        mb.Entity<Empresa>(e =>
        {
            e.Property(p => p.Nome)
                .HasMaxLength(120)
                .IsRequired();
            
            e.Property(p => p.Cnpj)
                .HasMaxLength(14)
                .IsRequired();
            e.HasIndex(e => e.Cnpj)
                .IsUnique();
            
            e.Property(p => p.Segmento)
                .HasMaxLength(120)
                .IsRequired();
        });

        mb.Entity<Unidade>(e =>
        {
            e.Property(p => p.Nome)
                .HasMaxLength(255)
                .IsRequired();
            
            e.Property(p => p.Endereco)
                .HasMaxLength(255)
                .IsRequired();
            
            e.Property(p => p.Estado)
                .HasMaxLength(255)
                .IsRequired();
            
            e.Property(p => p.Area)
                .IsRequired();
            
            e.Property(p => p.InicioOperacao)
                .IsRequired();

            e.HasOne(e => e.Empresa)
                .WithMany(e => e.Unidades)
                .HasForeignKey(e => e.IdEmpresa);
        });

        mb.Entity<Consumo>(e =>
        {
            e.Property(p => p.DataReferencia)
                .IsRequired();
            
            e.Property(p => p.ConsumoTotal)
                .IsRequired();
            
            e.Property(p => p.TipoFonte)
                .HasMaxLength(100)
                .IsRequired();
            
            e.Property(p => p.ERenovavel)
                .IsRequired();

            e.HasOne(e => e.Unidade)
                .WithMany(e => e.Consumos)
                .HasForeignKey(e => e.IdUnidade);
        });

        mb.Entity<Acao>(e =>
        {
            e.Property(p => p.Descricao)
                .HasMaxLength(255)
                .IsRequired();
            
            e.Property(p => p.Categoria)
                .HasMaxLength(255)
                .IsRequired();
        });

        mb.Entity<UnidadeAcao>(e =>
        {
            e.Property(p => p.DataImplantacao)
                .IsRequired();

            e.HasOne(e => e.Unidade)
                .WithMany(e => e.UnidadesAcoes)
                .HasForeignKey(e => e.IdUnidade);

            e.HasOne(e => e.Acao)
                .WithMany(e => e.UnidadesAcoes)
                .HasForeignKey(e => e.IdAcao);
        });

        SetFilterUniqueWhenDeletedAtIsNotNull(mb);
        FilterDeletedAtNullValues(mb);
        base.OnModelCreating(mb);
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Acao> Acoes { get; set; }
    public DbSet<Consumo> Consumo { get; set; }
    public DbSet<Empresa> Empresas { get; set; }
    public DbSet<Unidade> Unidades { get; set; }
    public DbSet<UnidadeAcao> UnidadesAcoes { get; set; }

    private void SetFilterUniqueWhenDeletedAtIsNotNull(ModelBuilder mb)
    {
        foreach (var type in mb.Model.GetEntityTypes())
            if (typeof(GenericModel).IsAssignableFrom(type.ClrType))
                foreach (var index in type.GetIndexes())
                    if (index.IsUnique)
                        index.SetFilter("DeletedAt IS NULL");
    }

    public override int SaveChanges()
    {
        UpdateModifiedAtAndCreatedAt();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateModifiedAtAndCreatedAt();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateModifiedAtAndCreatedAt()
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is GenericModel);

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                ((GenericModel)entry.Entity).SetCreatedAt();
                ((GenericModel)entry.Entity).SetUpdatedAt();
            }
            else if (entry.State == EntityState.Modified)
                ((GenericModel)entry.Entity).SetUpdatedAt();
        }
    }

    private void FilterDeletedAtNullValues(ModelBuilder mb)
    {
        foreach (var type in mb.Model.GetEntityTypes())
        {
            var clrType = type.ClrType;

            if (typeof(GenericModel).IsAssignableFrom(clrType))
            {
                var param = Expression.Parameter(clrType, "e");
                
                var property = Expression.Property(param, nameof(GenericModel.DeletedAt));
                var nullConstant = Expression.Constant(null, typeof(DateTime?));
                var body = Expression.Equal(property, nullConstant);

                var lambda = Expression.Lambda(body, param);

                mb.Entity(clrType).HasQueryFilter(lambda);
            }
        }
    }
}