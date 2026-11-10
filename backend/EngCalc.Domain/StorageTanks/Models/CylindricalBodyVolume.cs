namespace EngCalc.Domain.StorageTanks.Models;

using EngCalc.Domain.Common;
using EngCalc.Domain.StorageTanks.Interfaces;

public class CylindricalBodyVolume : IGeometryVolume
{
    public double InternalDiameter { get;  private set; }
    public double InternalHeight { get;  private set; }
    public double InternalRadius { get { return InternalDiameter / 2; } }
    public double InternalVolume { get { return Math.PI * Math.Pow(InternalRadius, 2) * InternalHeight; } }

    public CylindricalBodyVolume(double internalDiameter, double internalHeight)
    {
        Guard.AgainstNegativeOrZero(internalDiameter, nameof(internalDiameter));
        Guard.AgainstNegativeOrZero(internalHeight, nameof(internalHeight));
        
        InternalDiameter = internalDiameter;
        InternalHeight = internalHeight;
    }
}