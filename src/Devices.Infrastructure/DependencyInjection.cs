using Devices.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Devices.Application.Interfaces;
using Devices.Infrastructure.Repositories;

namespace Devices.Infrastructure;

public static class DependencyInjection
{
	public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
	{
		services.AddDbContext<DevicesDbContext>(options =>
			options.UseNpgsql(connectionString, npgsql => npgsql.EnableRetryOnFailure()));
		services.AddScoped<IDeviceRepository, DeviceRepository>();

		return services;
	}
}