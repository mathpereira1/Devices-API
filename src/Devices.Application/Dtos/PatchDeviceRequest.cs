using Devices.Domain.Enums;

namespace Devices.Application.Dtos;

public record PatchDeviceRequest(string? Name, string? Brand, DeviceState? State);
