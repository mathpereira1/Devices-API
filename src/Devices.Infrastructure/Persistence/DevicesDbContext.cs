using Devices.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Devices.Infrastructure.Persistence;
public class DevicesDbContext : DbContext
{
	public DevicesDbContext(DbContextOptions<DevicesDbContext> options) : base(options) { }

	public DbSet<Device> Devices { get; set; } = null!;

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.ApplyConfigurationsFromAssembly(typeof(DevicesDbContext).Assembly);
	}
}