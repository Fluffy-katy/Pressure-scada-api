using Microsoft.EntityFrameworkCore;
using PressureScada.Api.Models;

namespace PressureScada.Api.Data;

public sealed class ScadaDbContext(DbContextOptions<ScadaDbContext> options) : DbContext(options)
{
    public DbSet<MeasurementStation> Stations => Set<MeasurementStation>();
    public DbSet<PressureSensor> Sensors => Set<PressureSensor>();
    public DbSet<PressureMeasurement> Measurements => Set<PressureMeasurement>();
}

public static class SeedData
{
    public static void Initialize(ScadaDbContext db)
    {
        if (db.Stations.Any())
        {
            return;
        }

        var station = new MeasurementStation("Скважина 101", "Цех добычи №1", "Устьевая площадка");
        var station2 = new MeasurementStation("Скважина 205", "Цех добычи №2", "Дожимная насосная");

        db.Stations.AddRange(station, station2);
        db.SaveChanges();

        var sensor = new PressureSensor("PT-101", "Rosemount 3051", "bar", 0, 160, station.Id);
        var sensor2 = new PressureSensor("PT-205", "WIKA S-20", "bar", 0, 250, station2.Id);
        db.Sensors.AddRange(sensor, sensor2);
        db.Measurements.AddRange(
            new PressureMeasurement(sensor.Id, 82.4, DateTime.UtcNow.AddMinutes(-30), "Норма"),
            new PressureMeasurement(sensor.Id, 84.1, DateTime.UtcNow.AddMinutes(-10), "Норма"),
            new PressureMeasurement(sensor2.Id, 198.6, DateTime.UtcNow.AddMinutes(-15), "Предупреждение"));
        db.SaveChanges();
    }
}
