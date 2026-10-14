namespace PressureScada.Api.Models;

public sealed class PressureMeasurement
{
    private PressureMeasurement() { }

    public PressureMeasurement(int sensorId, double value, DateTime measuredAt, string status)
    {
        if (double.IsNaN(value) || double.IsInfinity(value))
        {
            throw new ArgumentException("Значение давления должно быть числом.");
        }

        SensorId = sensorId;
        Value = value;
        MeasuredAt = measuredAt.Kind == DateTimeKind.Utc ? measuredAt : measuredAt.ToUniversalTime();
        Status = string.IsNullOrWhiteSpace(status) ? "Норма" : status.Trim();
    }

    public int Id { get; private set; }
    public int SensorId { get; private set; }
    public double Value { get; private set; }
    public DateTime MeasuredAt { get; private set; }
    public string Status { get; private set; } = string.Empty;

    public bool IsCritical(double criticalLimit) => Value >= criticalLimit;
    public void MarkStatus(string status) => Status = string.IsNullOrWhiteSpace(status) ? "Норма" : status.Trim();
}
