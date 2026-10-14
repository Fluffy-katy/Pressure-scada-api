using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PressureScada.Api.Data;
using PressureScada.Api.Models;

namespace PressureScada.Api.Controllers;

[ApiController]
[Route("api/sensors")]
public sealed class SensorsController(ScadaDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PressureSensor>>> GetAll() =>
        Ok(await db.Sensors.AsNoTracking().ToListAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PressureSensor>> Get(int id)
    {
        var sensor = await db.Sensors.FindAsync(id);
        return sensor is null ? NotFound() : Ok(sensor);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<PressureSensor>> Create(SensorRequest request)
    {
        if (!await db.Stations.AnyAsync(x => x.Id == request.StationId)) return BadRequest("Станция не найдена.");
        var sensor = new PressureSensor(request.Code, request.Model, request.Unit, request.MinPressure, request.MaxPressure, request.StationId);
        db.Sensors.Add(sensor);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = sensor.Id }, sensor);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, SensorRequest request)
    {
        var sensor = await db.Sensors.FindAsync(id);
        if (sensor is null) return NotFound();
        if (!await db.Stations.AnyAsync(x => x.Id == request.StationId)) return BadRequest("Станция не найдена.");
        sensor.Update(request.Code, request.Model, request.Unit, request.MinPressure, request.MaxPressure);
        await db.SaveChangesAsync();
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var sensor = await db.Sensors.FindAsync(id);
        if (sensor is null) return NotFound();
        db.Sensors.Remove(sensor);
        await db.SaveChangesAsync();
        return NoContent();
    }
}

public sealed record SensorRequest(string Code, string Model, string Unit, double MinPressure, double MaxPressure, int StationId);
