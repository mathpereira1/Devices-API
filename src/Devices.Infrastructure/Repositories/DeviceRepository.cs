using Devices.Application.Interfaces;
using Devices.Domain.Enums;
using Devices.Domain.Models;
using Devices.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Devices.Infrastructure.Repositories;

public class DeviceRepository : IDeviceRepository
{
	private readonly DevicesDbContext _context;

	public DeviceRepository(DevicesDbContext context)
	{
		_context = context;
	}

	public async Task<Device?> GetByIdAsync(Guid id)
	{
		return await _context.Devices.FindAsync(id);
	}

	public async Task<IReadOnlyList<Device>> GetAllAsync(string? brand, DeviceState? state)
	{
		var query = _context.Devices.AsNoTracking();

		if (!string.IsNullOrWhiteSpace(brand))
		{
			query = query.Where(d => d.Brand == brand);
		}

		if (state.HasValue)
		{
			query = query.Where(d => d.State == state.Value);
		}

		return await query.ToListAsync();
	}

	public async Task AddAsync(Device device)
	{
		_context.Devices.Add(device);
		await _context.SaveChangesAsync();
	}

	public async Task UpdateAsync(Device device)
	{
		await _context.SaveChangesAsync();
	}

	public async Task DeleteAsync(Device device)
	{
		_context.Devices.Remove(device);
		await _context.SaveChangesAsync();
	}
}
