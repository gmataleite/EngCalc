namespace VesselCalc.Domain.Constants;

using UnitsNet;

public static class PhysicalConstants
{
    /// <summary>
    /// Aceleração padrão da gravidade terrestre (g).
    /// </summary>
    public static readonly Acceleration StandardGravity = Acceleration.FromMetersPerSecondSquared(9.80665);
}