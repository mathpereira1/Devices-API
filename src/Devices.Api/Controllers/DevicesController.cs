using Devices.Application.Dtos;
using Devices.Application.Services;
using Devices.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Devices.Api.Controllers;

[ApiController]
[Route("api/devices")]
public class DevicesController : ControllerBase
{
	private readonly DeviceService _deviceService;

	public DevicesController(DeviceService deviceService)
	{
		_deviceService = deviceService;
	}

	[HttpPost]
	public async Task<ActionResult<DeviceResponse>> Create([FromBody] CreateDeviceRequest request)
	{
		var device = await _deviceService.CreateDeviceAsync(request);
		return CreatedAtAction(nameof(GetById), new { id = device.Id }, device);
	}

	[HttpGet("{id:guid}")]
	public async Task<ActionResult<DeviceResponse>> GetById(Guid id)
	{
		var device = await _deviceService.GetDeviceByIdAsync(id);
		return Ok(device);
	}

	[HttpGet]
	public async Task<ActionResult<IReadOnlyList<DeviceResponse>>> GetAll([FromQuery] string? brand, [FromQuery] DeviceState? state)
	{
		var devices = await _deviceService.GetAllDevicesAsync(brand, state);
		return Ok(devices);
	}

	[HttpPut("{id:guid}")]
	public async Task<ActionResult<DeviceResponse>> Update(Guid id, [FromBody] UpdateDeviceRequest request)
	{
		var device = await _deviceService.UpdateDeviceAsync(id, request);
		return Ok(device);
	}

	[HttpPatch("{id:guid}")]
	public async Task<ActionResult<DeviceResponse>> Patch(Guid id, [FromBody] PatchDeviceRequest request)
	{
		var device = await _deviceService.PatchDeviceAsync(id, request);
		return Ok(device);
	}

	[HttpDelete("{id:guid}")]
	public async Task<IActionResult> Delete(Guid id)
	{
		await _deviceService.DeleteDeviceAsync(id);
		return NoContent();
	}
}
