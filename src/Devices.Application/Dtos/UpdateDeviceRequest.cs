using Devices.Domain.Enums;

namespace Devices.Application.Dtos;

public record UpdateDeviceRequest(string Name, string Brand, DeviceState State);