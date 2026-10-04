using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Devices.Application.Dtos;
using Devices.Domain.Enums;

namespace Devices.IntegrationTests;

public class DevicesControllerTests : IClassFixture<DevicesApiFactory>
{
	private readonly HttpClient _client;
	private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web)
	{
		Converters = { new JsonStringEnumConverter() }
	};

	public DevicesControllerTests(DevicesApiFactory factory)
	{
		_client = factory.CreateClient();
	}

	[Fact]
	public async Task CreateDevice_ShouldReturnCreatedWithLocation()
	{
		// Act
		var response = await _client.PostAsJsonAsync("/api/devices", new { name = "Phone", brand = "Apple", state = "Available" });

		// Assert
		Assert.Equal(HttpStatusCode.Created, response.StatusCode);
		var device = await ReadDeviceAsync(response);
		Assert.Equal("Phone", device.Name);
		Assert.Equal("Apple", device.Brand);
		Assert.Equal(DeviceState.Available, device.State);
		Assert.EndsWith($"/api/devices/{device.Id}", response.Headers.Location!.ToString());
	}

	[Fact]
	public async Task CreateDevice_ShouldDefaultToAvailable_WhenStateIsNotSent()
	{
		// Act
		var response = await _client.PostAsJsonAsync("/api/devices", new { name = "Phone", brand = "Apple" });

		// Assert
		Assert.Equal(HttpStatusCode.Created, response.StatusCode);
		var device = await ReadDeviceAsync(response);
		Assert.Equal(DeviceState.Available, device.State);
	}

	[Fact]
	public async Task CreateDevice_ShouldReturnBadRequest_WhenNameIsEmpty()
	{
		// Act
		var response = await _client.PostAsJsonAsync("/api/devices", new { name = "", brand = "Apple", state = "Available" });

		// Assert
		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
	}

	[Fact]
	public async Task CreateDevice_ShouldReturnBadRequest_WhenStateIsInvalid()
	{
		// Act
		var response = await _client.PostAsJsonAsync("/api/devices", new { name = "Phone", brand = "Apple", state = "Banana" });

		// Assert
		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
	}

	[Fact]
	public async Task GetDevice_ShouldReturnNotFound_WhenDeviceDoesNotExist()
	{
		// Act
		var response = await _client.GetAsync($"/api/devices/{Guid.NewGuid()}");

		// Assert
		Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
	}

	[Fact]
	public async Task GetDevices_ShouldReturnOnlyMatchingDevices_WhenFilteredByBrandAndState()
	{
		// Arrange
		var brand = UniqueBrand();
		var match = await CreateDeviceAsync(brand, "InUse");
		await CreateDeviceAsync(brand, "Available");
		await CreateDeviceAsync(UniqueBrand(), "InUse");

		// Act
		var devices = await _client.GetFromJsonAsync<List<DeviceResponse>>($"/api/devices?brand={brand}&state=InUse", _jsonOptions);

		// Assert
		var device = Assert.Single(devices!);
		Assert.Equal(match.Id, device.Id);
	}

	[Fact]
	public async Task PatchDevice_ShouldChangeOnlyTheState()
	{
		// Arrange
		var created = await CreateDeviceAsync(UniqueBrand(), "Available");

		// Act
		var response = await _client.PatchAsJsonAsync($"/api/devices/{created.Id}", new { state = "InUse" });

		// Assert
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var device = await ReadDeviceAsync(response);
		Assert.Equal(DeviceState.InUse, device.State);
		Assert.Equal(created.Name, device.Name);
		Assert.Equal(created.Brand, device.Brand);
	}

	[Fact]
	public async Task PatchDevice_ShouldReturnConflictAndKeepName_WhenRenamingDeviceInUse()
	{
		// Arrange
		var created = await CreateDeviceAsync(UniqueBrand(), "InUse");

		// Act
		var response = await _client.PatchAsJsonAsync($"/api/devices/{created.Id}", new { name = "New name" });

		// Assert
		Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
		var device = await GetDeviceAsync(created.Id);
		Assert.Equal(created.Name, device.Name);
	}

	[Fact]
	public async Task PutDevice_ShouldReturnOk_WhenDeviceInUseKeepsSameNameAndBrand()
	{
		// Arrange
		var created = await CreateDeviceAsync(UniqueBrand(), "InUse");

		// Act
		var response = await _client.PutAsJsonAsync($"/api/devices/{created.Id}", new { name = created.Name, brand = created.Brand, state = "InUse" });

		// Assert
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
	}

	[Fact]
	public async Task PutDevice_ShouldNotChangeCreationTime()
	{
		// Arrange
		var created = await CreateDeviceAsync(UniqueBrand(), "Available");
		var before = await GetDeviceAsync(created.Id);

		// Act
		await _client.PutAsJsonAsync($"/api/devices/{created.Id}", new { name = "Renamed", brand = "Other brand", state = "Inactive" });

		// Assert
		var after = await GetDeviceAsync(created.Id);
		Assert.Equal("Renamed", after.Name);
		Assert.Equal(before.CreatedAt, after.CreatedAt);
	}

	[Fact]
	public async Task DeleteDevice_ShouldReturnConflict_WhenDeviceIsInUse()
	{
		// Arrange
		var created = await CreateDeviceAsync(UniqueBrand(), "InUse");

		// Act
		var response = await _client.DeleteAsync($"/api/devices/{created.Id}");

		// Assert
		Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
	}

	[Fact]
	public async Task DeleteDevice_ShouldRemoveDevice_WhenDeviceIsAvailable()
	{
		// Arrange
		var created = await CreateDeviceAsync(UniqueBrand(), "Available");

		// Act
		var response = await _client.DeleteAsync($"/api/devices/{created.Id}");

		// Assert
		Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
		var getResponse = await _client.GetAsync($"/api/devices/{created.Id}");
		Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
	}

	// Every test creates its own data with a unique brand, so tests never depend on each other
	private static string UniqueBrand()
	{
		return $"Brand-{Guid.NewGuid()}";
	}

	private async Task<DeviceResponse> CreateDeviceAsync(string brand, string state)
	{
		var response = await _client.PostAsJsonAsync("/api/devices", new { name = "Test device", brand, state });
		response.EnsureSuccessStatusCode();
		return await ReadDeviceAsync(response);
	}

	private async Task<DeviceResponse> GetDeviceAsync(Guid id)
	{
		var device = await _client.GetFromJsonAsync<DeviceResponse>($"/api/devices/{id}", _jsonOptions);
		return device!;
	}

	private async Task<DeviceResponse> ReadDeviceAsync(HttpResponseMessage response)
	{
		var device = await response.Content.ReadFromJsonAsync<DeviceResponse>(_jsonOptions);
		return device!;
	}
}
