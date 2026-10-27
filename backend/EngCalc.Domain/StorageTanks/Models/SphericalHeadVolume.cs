namespace EngCalc.Domain.StorageTanks.Models;

using EngCalc.Domain.Common;
using EngCalc.Domain.StorageTanks.Interfaces;

public class SphericalHeadVolume : IGeometryVolume
{
    public double InternalDiameter { get;  private set; }
    public double InternalRadius { get { return InternalDiameter / 2; } }
    public double InternalVolume { get { return Math.PI * Math.Pow(InternalRadius, 3) * 2 / 3; } }
    

    public SphericalHeadVolume(double internalDiameter)
    {
        Guard.AgainstNegativeOrZero(internalDiameter, nameof(internalDiameter));
        InternalDiameter = internalDiameter;
    }
}