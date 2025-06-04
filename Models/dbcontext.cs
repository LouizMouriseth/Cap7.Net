using Microsoft.EntityFrameworkCore;
using MeuProjetoApi.Models;

public class MeuDbContext : DbContext
{
	public MeuDbContext(DbContextOptions<MeuDbContext> options) : base(options) { }

	public DbSet<Produto> Produtos { get; set; }
}
