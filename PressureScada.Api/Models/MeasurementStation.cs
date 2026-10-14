namespace PressureScada.Api.Models;

public sealed class MeasurementStation
{
    private MeasurementStation() { }

    public MeasurementStation(string name, string productionUnit, string location)
    {
        Update(name, productionUnit, location);
    }

    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string ProductionUnit { get; private set; } = string.Empty;
    public string Location { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public void Update(string name, string productionUnit, string location)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(productionUnit) ||
            string.IsNullOrWhiteSpace(location))
        {
            throw new ArgumentException("Название, производственный участок и расположение обязательны.");
        }

        Name = name.Trim();
        ProductionUnit = productionUnit.Trim();
        Location = location.Trim();
    }
}
