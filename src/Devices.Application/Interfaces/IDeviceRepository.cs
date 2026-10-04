
using Devices.Domain.Models;
using Devices.Domain.Enums;

namespace Devices.Application.Interfaces;

public interface IDeviceRepository
{
	Task<Device?> GetByIdAsync(Guid id);
	Task<IReadOnlyList<Device>> GetAllAsync(string? brand, DeviceState? state);
	Task AddAsync(Device device);
	Task UpdateAsync(Device device);
	Task DeleteAsync(Device device);
}