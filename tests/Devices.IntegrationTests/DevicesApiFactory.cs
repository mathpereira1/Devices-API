using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.PostgreSql;

namespace Devices.IntegrationTests;

public class DevicesApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
	private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17").Build();

	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		builder.UseEnvironment("Testing");
		builder.UseSetting("ConnectionStrings:DevicesDb", _postgres.GetConnectionString());
	}

	public async Task InitializeAsync()
	{
		await _postgres.StartAsync();
	}

	async Task IAsyncLifetime.DisposeAsync()
	{
		await _postgres.DisposeAsync();
	}
}
