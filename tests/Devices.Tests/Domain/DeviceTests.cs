using Devices.Domain.Models;
using Devices.Domain.Enums;
using Devices.Domain.Exceptions;

namespace Devices.Tests.Domain;

public class DeviceTests
{
	[Fact]
	public void CreateDevice_ShouldCreateDeviceWithValidProperties()
	{
		// Arrange
		var name = "Device 1";
		var brand = "Brand A";
		var state = DeviceState.Available;
		var beforeCreating = DateTime.UtcNow;

		// Act
		var device = new Device(name, brand, state);
		var afterCreating = DateTime.UtcNow;

		// Assert
		Assert.Equal(name, device.Name);
		Assert.Equal(brand, device.Brand);
		Assert.Equal(state, device.State);
		Assert.NotEqual(Guid.Empty, device.Id);
		Assert.InRange(device.CreatedAt, beforeCreating, afterCreating);
		Assert.Equal(DateTimeKind.Utc, device.CreatedAt.Kind);
	}

	[Theory]
	[InlineData(null, null, DeviceState.InUse, DeviceState.Available)]
	[InlineData("Device 1", "Brand A", DeviceState.Available, DeviceState.InUse)] // PUT
	[InlineData("X", "", DeviceState.Available, DeviceState.InUse)] 
	public void UpdateDevice_ShouldChangeState(string? name, string? brand, DeviceState initialState, DeviceState newState)
	{
		// Arrange
		var device = new Device("Device 1", "Brand A", initialState);

		// Act
		device.Update(name, brand, newState);

		// Assert
		Assert.Equal(newState, device.State);
	}

	[Theory]
	[InlineData("X", null, null)]
	[InlineData(null, "X", null)]
	[InlineData("X", null, DeviceState.Available)]
	public void UpdateDevice_ShouldThrowWhenInUseAndNameOrBrandChanges(string? name, string? brand, DeviceState? newState)
	{
		// Arrange
		var device = new Device("Device 1", "Brand A", DeviceState.InUse);

		// Act & Assert
		Assert.Throws<DeviceInUseException>(() => device.Update(name, brand, newState));

		Assert.Equal("Device 1", device.Name);
		Assert.Equal("Brand A", device.Brand);
		Assert.Equal(DeviceState.InUse, device.State);
	}

	[Fact]
	public void UpdateDevice_ShouldAcceptWhenInUseAndNameAndBrandAreTheSame()
	{
		// Arrange
		var device = new Device("Device 1", "Brand A", DeviceState.InUse);

		// Act
		device.Update("Device 1", "Brand A", DeviceState.InUse);

		// Assert
		Assert.Equal("Device 1", device.Name);
		Assert.Equal("Brand A", device.Brand);
		Assert.Equal(DeviceState.InUse, device.State);
	}

	[Theory]
	[InlineData(null, "X", "Device 1", "X")]
	[InlineData("X", null, "X", "Brand A")] 
	[InlineData(null, null, "Device 1", "Brand A")] 
	public void UpdateDevice_ShouldKeepFieldsNotSent(string? name, string? brand, string expectedName, string expectedBrand)
	{
		// Arrange
		var device = new Device("Device 1", "Brand A", DeviceState.Available);

		// Act
		device.Update(name, brand, null);

		// Assert
		Assert.Equal(expectedName, device.Name);
		Assert.Equal(expectedBrand, device.Brand);
		Assert.Equal(DeviceState.Available, device.State);
	}

	[Theory]
	[InlineData("", null, DeviceState.Available)]
	[InlineData(" ", null, DeviceState.Available)]
	[InlineData("", null, DeviceState.InUse)]
	[InlineData(" ", null, DeviceState.InUse)]
	[InlineData(null, "", DeviceState.Available)]
	[InlineData(null, " ", DeviceState.Available)]
	[InlineData(null, "", DeviceState.InUse)]
	[InlineData(null, " ", DeviceState.InUse)]
	public void UpdateDevice_ShouldIgnoreEmptyNameOrBrand(string? name, string? brand, DeviceState initialState)
	{
		// Arrange
		var device = new Device("Device 1", "Brand A", initialState);

		// Act
		device.Update(name, brand, null);

		// Assert
		Assert.Equal("Device 1", device.Name);
		Assert.Equal("Brand A", device.Brand);
	}

	[Theory]
	[InlineData(" ")]
	[InlineData("")]
	[InlineData(null)]
	public void CreateDevice_ShouldThrowDomainValidationException_WhenNameIsInvalid(string? name)
	{
		// Arrange
		var brand = "Brand A";
		var state = DeviceState.Available;

		// Act & Assert
		Assert.Throws<DomainValidationException>(() => new Device(name!, brand, state));
	}

	[Theory]
	[InlineData(" ")]
	[InlineData("")]
	[InlineData(null)]
	public void CreateDevice_ShouldThrowDomainValidationException_WhenBrandIsInvalid(string? brand)
	{
		// Arrange
		var name = "Device 1";
		var state = DeviceState.Available;

		// Act & Assert
		Assert.Throws<DomainValidationException>(() => new Device(name, brand!, state));
	}

	[Fact]
	public void EnsureCanBeDeleted_ShouldThrowWhenDeviceIsInUse()
	{
		// Arrange
		var device = new Device("Device 1", "Brand A", DeviceState.InUse);

		// Act & Assert
		Assert.Throws<DeviceInUseException>(() => device.EnsureCanBeDeleted());
	}

	[Theory]
	[InlineData(DeviceState.Available)]
	[InlineData(DeviceState.Inactive)]	
	public void EnsureCanBeDeleted_ShouldNotThrowWhenDeviceIsNotInUse(DeviceState state)
	{
		// Arrange
		var device = new Device("Device 1", "Brand A", state);

		// Act & Assert
		var exception = Record.Exception(() => device.EnsureCanBeDeleted());
		Assert.Null(exception);
	}
}
