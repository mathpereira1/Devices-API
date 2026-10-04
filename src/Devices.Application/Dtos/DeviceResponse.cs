using Devices.Domain.Enums;
using Devices.Domain.Models;

namespace Devices.Application.Dtos;

public record DeviceResponse(Guid Id, string Name, string Brand, DeviceState State, DateTime CreatedAt)
{
	public static DeviceResponse FromEntity(Device device)
	{
		return new DeviceResponse(device.Id, device.Name, device.Brand, device.State, device.CreatedAt);
	}
}
