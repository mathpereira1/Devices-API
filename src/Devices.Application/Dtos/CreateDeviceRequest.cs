using Devices.Domain.Enums;

namespace Devices.Application.Dtos;

public record CreateDeviceRequest(string Name, string Brand, DeviceState State);