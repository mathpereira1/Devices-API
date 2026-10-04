namespace Devices.Application.Exceptions;

public sealed class DeviceNotFoundException : Exception
{
	public Guid DeviceId { get; }
	public DeviceNotFoundException(Guid deviceId) 
		: base($"Device with ID '{deviceId}' was not found.")
	{
		DeviceId = deviceId;
	}
}