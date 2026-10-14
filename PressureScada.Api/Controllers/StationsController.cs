using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PressureScada.Api.Data;
using PressureScada.Api.Models;

namespace PressureScada.Api.Controllers;

[ApiController]
[Route("api/stations")]
public sealed class StationsController(ScadaDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MeasurementStation>>> GetAll() =>
        Ok(await db.Stations.AsNoTracking().ToListAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<MeasurementStation>> Get(int id)
    {
        var station = await db.Stations.FindAsync(id);
        return station is null ? NotFound() : Ok(station);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<MeasurementStation>> Create(StationRequest request)
    {
        var station = new MeasurementStation(request.Name, request.ProductionUnit, request.Location);
        db.Stations.Add(station);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = station.Id }, station);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, StationRequest request)
    {
        var station = await db.Stations.FindAsync(id);
        if (station is null) return NotFound();
        station.Update(request.Name, request.ProductionUnit, request.Location);
        await db.SaveChangesAsync();
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var station = await db.Stations.FindAsync(id);
        if (station is null) return NotFound();
        db.Stations.Remove(station);
        await db.SaveChangesAsync();
        return NoContent();
    }
}

public sealed record StationRequest(string Name, string ProductionUnit, string Location);
