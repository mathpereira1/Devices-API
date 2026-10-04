using Devices.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Devices.Infrastructure.Persistence.Configurations;

public class DeviceConfiguration : IEntityTypeConfiguration<Device>
{
	public void Configure(EntityTypeBuilder<Device> builder)
	{
		builder.ToTable("devices");

		builder.HasKey(d => d.Id);
		builder.Property(d => d.Id)
			.ValueGeneratedNever();

		builder.Property(d => d.Name)
			.IsRequired()
			.HasMaxLength(100);

		builder.Property(d => d.Brand)
			.IsRequired()
			.HasMaxLength(100);

		builder.Property(d => d.State)
			.IsRequired()
			.HasConversion<string>()
			.HasMaxLength(20);

		builder.Property(d => d.CreatedAt)
			.IsRequired();

		builder.HasIndex(d => d.Brand);
		builder.HasIndex(d => d.State);
	}
}
