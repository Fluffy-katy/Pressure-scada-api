using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PressureScada.Api.Data;

namespace PressureScada.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/reports")]
public sealed class ReportsController(ScadaDbContext db) : ControllerBase
{
    [HttpGet("latest-by-station")]
    public async Task<IActionResult> LatestByStation()
    {
        var result = await db.Measurements
            .Join(db.Sensors, measurement => measurement.SensorId, sensor => sensor.Id,
                (measurement, sensor) => new { measurement, sensor })
            .Join(db.Stations, item => item.sensor.StationId, station => station.Id,
                (item, station) => new { item.measurement, item.sensor, station })
            .GroupBy(x => new { x.station.Id, x.station.Name, x.station.ProductionUnit })
            .Select(group => new
            {
                StationId = group.Key.Id,
                Station = group.Key.Name,
                group.Key.ProductionUnit,
                LatestPressure = group.OrderByDescending(x => x.measurement.MeasuredAt).First().measurement.Value,
                MeasuredAt = group.Max(x => x.measurement.MeasuredAt)
            })
            .ToListAsync();
        return Ok(result);
    }

    [HttpGet("sensor-statistics")]
    public async Task<IActionResult> SensorStatistics()
    {
        var result = await db.Sensors
            .Join(db.Stations, sensor => sensor.StationId, station => station.Id,
                (sensor, station) => new { sensor, station })
            .GroupJoin(db.Measurements, item => item.sensor.Id, measurement => measurement.SensorId,
                (item, measurements) => new
                {
                    Sensor = item.sensor.Code,
                    Station = item.station.Name,
                    MeasurementsCount = measurements.Count(),
                    AveragePressure = measurements.Select(x => (double?)x.Value).Average() ?? 0,
                    MaximumPressure = measurements.Select(x => (double?)x.Value).Max() ?? 0
                })
            .ToListAsync();
        return Ok(result);
    }

    [HttpGet("alerts")]
    public async Task<IActionResult> Alerts([FromQuery] double threshold = 180)
    {
        var result = await db.Measurements
            .Where(x => x.Value >= threshold)
            .Join(db.Sensors, measurement => measurement.SensorId, sensor => sensor.Id,
                (measurement, sensor) => new { measurement, sensor })
            .Join(db.Stations, item => item.sensor.StationId, station => station.Id,
                (item, station) => new
                {
                    station.Name,
                    SensorCode = item.sensor.Code,
                    item.measurement.Value,
                    item.measurement.MeasuredAt,
                    item.measurement.Status
                })
            .OrderByDescending(x => x.Value)
            .ToListAsync();
        return Ok(result);
    }
}
