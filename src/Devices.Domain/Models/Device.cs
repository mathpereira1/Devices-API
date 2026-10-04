namespace Devices.Domain.Models;

using Devices.Domain.Enums;
using Devices.Domain.Exceptions;

public class Device
{
	public Guid Id { get; private set; }
	public string Name { get; private set; } = string.Empty;
	public string Brand { get; private set; } = string.Empty;
	public DeviceState State { get; private set; }
	public DateTime CreatedAt { get; private set; }

	private Device() { }
	public Device(string name, string brand, DeviceState state)
	{
		Validate(name, brand);
		Id = Guid.CreateVersion7();
		Name = name;
		Brand = brand;
		State = state;
		CreatedAt = DateTime.UtcNow;
	}

	public void Update(string? name, string? brand, DeviceState? state)
	{
		State = state ?? State;

		if (State == DeviceState.InUse && (name != null || brand != null))
		{
			throw new DeviceInUseException(Id);
		}
		else
		{
			Name = string.IsNullOrWhiteSpace(name) ? Name : name;
			Brand = string.IsNullOrWhiteSpace(brand) ? Brand : brand;
		}
	}
	
	private static void Validate(string name, string brand)
	{
		if (string.IsNullOrWhiteSpace(name))
		{
			throw new DomainValidationException("Device name cannot be empty.");
		}

		if (string.IsNullOrWhiteSpace(brand))
		{
			throw new DomainValidationException("Device brand cannot be empty.");
		}
	}
	
}