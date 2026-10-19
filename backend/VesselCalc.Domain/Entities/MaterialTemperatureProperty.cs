namespace VesselCalc.Domain.Entities;

public class MaterialTemperatureProperty
{
    public int Id { get; private set; }
    public string MaterialId { get; private set; } = null!;
    public decimal Temperature { get; private set; }
    public decimal? AllowableStress { get; private set; } // null = NP
    public decimal? YieldStress { get; private set; }     // null = "---"

    protected MaterialTemperatureProperty() { }

    internal MaterialTemperatureProperty(decimal temperature, decimal? allowableStress, decimal? yieldStress)
    {
        Temperature = temperature;
        AllowableStress = allowableStress;
        YieldStress = yieldStress;
    }
}