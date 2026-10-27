namespace EngCalc.Domain.StorageTanks.Models;

using EngCalc.Domain.Common;
using EngCalc.Domain.StorageTanks.Interfaces;

public class FlatHeadVolume : IGeometryVolume
{    public double InternalDiameter { get;  private set; }
    public double InternalRadius { get { return InternalDiameter / 2; } }
    public double InternalVolume { get { return 0; } } 

    public FlatHeadVolume(double internalDiameter)
    {
        Guard.AgainstNegativeOrZero(internalDiameter, nameof(internalDiameter));
        InternalDiameter = internalDiameter;
    }
}