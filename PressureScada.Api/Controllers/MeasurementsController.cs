using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PressureScada.Api.Data;
using PressureScada.Api.Models;

namespace PressureScada.Api.Controllers;

[ApiController]
[Route("api/measurements")]
public sealed class MeasurementsController(ScadaDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PressureMeasurement>>> GetAll([FromQuery] int? sensorId) =>
        Ok(await db.Measurements.AsNoTracking()
            .Where(x => sensorId == null || x.SensorId == sensorId.Value)
            .OrderByDescending(x => x.MeasuredAt)
            .ToListAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PressureMeasurement>> Get(int id)
    {
        var measurement = await db.Measurements.FindAsync(id);
        return measurement is null ? NotFound() : Ok(measurement);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<PressureMeasurement>> Create(MeasurementRequest request)
    {
        var sensor = await db.Sensors.FindAsync(request.SensorId);
        if (sensor is null) return BadRequest("Датчик не найден.");
        var status = sensor.IsValueInRange(request.Value) ? "Норма" : "Выход за диапазон";
        var measurement = new PressureMeasurement(request.SensorId, request.Value, request.MeasuredAt ?? DateTime.UtcNow, status);
        db.Measurements.Add(measurement);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = measurement.Id }, measurement);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, MeasurementRequest request)
    {
        var measurement = await db.Measurements.FindAsync(id);
        var sensor = await db.Sensors.FindAsync(request.SensorId);
        if (measurement is null || sensor is null) return NotFound();
        measurement.MarkStatus(sensor.IsValueInRange(request.Value) ? "Норма" : "Выход за диапазон");
        db.Entry(measurement).Property(x => x.Value).CurrentValue = request.Value;
        db.Entry(measurement).Property(x => x.SensorId).CurrentValue = request.SensorId;
        db.Entry(measurement).Property(x => x.MeasuredAt).CurrentValue = request.MeasuredAt ?? DateTime.UtcNow;
        await db.SaveChangesAsync();
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var measurement = await db.Measurements.FindAsync(id);
        if (measurement is null) return NotFound();
        db.Measurements.Remove(measurement);
        await db.SaveChangesAsync();
        return NoContent();
    }
}

public sealed record MeasurementRequest(int SensorId, double Value, DateTime? MeasuredAt);
