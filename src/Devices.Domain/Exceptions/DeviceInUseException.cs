namespace Devices.Domain.Exceptions;

public sealed class DeviceInUseException : DomainException
{
	public Guid DeviceId { get; }
	public DeviceInUseException(Guid id) : base($"Device with ID {id} is currently in use.")
	{
		DeviceId = id;
	} 
}