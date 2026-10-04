using Devices.Domain.Enums;
using Devices.Domain.Models;
using Devices.Application.Interfaces;
using Devices.Application.Dtos;
using Devices.Application.Exceptions;

namespace Devices.Application.Services;

public class DeviceService
{
	private readonly IDeviceRepository _deviceRepository;

	public DeviceService(IDeviceRepository deviceRepository)
	{
		_deviceRepository = deviceRepository;
	}

	private async Task<Device> GetDeviceOrThrowAsync(Guid id)
	{
		var device = await _deviceRepository.GetByIdAsync(id) ?? throw new DeviceNotFoundException(id);
		return device;
	}

	public async Task<DeviceResponse> CreateDeviceAsync(CreateDeviceRequest request)
	{
		var device = new Device(request.Name, request.Brand, request.State);
		await _deviceRepository.AddAsync(device);
		return DeviceResponse.FromEntity(device);
	}

	public async Task<DeviceResponse> GetDeviceByIdAsync(Guid id)
	{
		var device = await GetDeviceOrThrowAsync(id);
		return DeviceResponse.FromEntity(device);
	}

	public async Task<IReadOnlyList<DeviceResponse>> GetAllDevicesAsync(string? brand, DeviceState? state)
	{
		var devices = await _deviceRepository.GetAllAsync(brand, state);
		return devices.Select(DeviceResponse.FromEntity).ToList();
	}

	public async Task<DeviceResponse> UpdateDeviceAsync(Guid id, UpdateDeviceRequest request)
	{
		var device = await GetDeviceOrThrowAsync(id);
		device.Update(request.Name, request.Brand, request.State);
		await _deviceRepository.UpdateAsync(device);
		return DeviceResponse.FromEntity(device);
	}

	public async Task<DeviceResponse> PatchDeviceAsync(Guid id, PatchDeviceRequest request)
	{
		var device = await GetDeviceOrThrowAsync(id);
		device.Update(request.Name, request.Brand, request.State);
		await _deviceRepository.UpdateAsync(device);
		return DeviceResponse.FromEntity(device);
	}

	public async Task DeleteDeviceAsync(Guid id)
	{
		var device = await GetDeviceOrThrowAsync(id);
		device.EnsureCanBeDeleted();
		await _deviceRepository.DeleteAsync(device);
	}
}
