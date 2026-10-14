namespace PressureScada.Api.Models;

public sealed class PressureSensor
{
    private PressureSensor() { }

    public PressureSensor(string code, string model, string unit, double minPressure, double maxPressure, int stationId)
    {
        Update(code, model, unit, minPressure, maxPressure);
        StationId = stationId;
    }

    public int Id { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Model { get; private set; } = string.Empty;
    public string Unit { get; private set; } = "bar";
    public double MinPressure { get; private set; }
    public double MaxPressure { get; private set; }
    public int StationId { get; private set; }
    public bool IsActive { get; private set; } = true;

    public void Update(string code, string model, string unit, double minPressure, double maxPressure)
    {
        if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(model) ||
            minPressure >= maxPressure)
        {
            throw new ArgumentException("Некорректные данные датчика или диапазон давления.");
        }

        Code = code.Trim();
        Model = model.Trim();
        Unit = string.IsNullOrWhiteSpace(unit) ? "bar" : unit.Trim();
        MinPressure = minPressure;
        MaxPressure = maxPressure;
    }

    public bool IsValueInRange(double value) => value >= MinPressure && value <= MaxPressure;
    public void Deactivate() => IsActive = false;
}
