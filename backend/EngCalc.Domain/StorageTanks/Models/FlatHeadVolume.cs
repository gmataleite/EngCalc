namespace EngCalc.Domain.StorageTanks.Models;

using EngCalc.Domain.Common;
using EngCalc.Domain.StorageTanks.Interfaces;

public class FlatHeadVolume : IHeadVolume
{
    public double InternalDiameter { get; private set; }
    public double InternalRadius => InternalDiameter / 2.0;
    public double InternalVolume => 0.0;
    public double HeadHeight => 0.0;

    public FlatHeadVolume(double internalDiameter)
    {
        Guard.AgainstNegativeOrZero(internalDiameter, nameof(internalDiameter));
        InternalDiameter = internalDiameter;
    }

    public double CalculateVerticalProductVolume(double heightFromBase) => 0.0;
}
