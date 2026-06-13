using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using Vendas_Dividas.Models;

namespace Vendas_Dividas.ContextDb;

public class AppDbContext : DbContext
{
	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
	{
	}

	public DbSet<Cliente> Clientes { get; set; }
	public DbSet<Divida> Dividas { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		// Apenas um cliente por CPF 
		modelBuilder.Entity<Cliente>()
			.HasIndex(c => c.Cpf)
			.IsUnique();

		// Filtro por texto
		modelBuilder.Entity<Cliente>()
			.HasIndex(c => c.NomeCompleto);
	}
}
