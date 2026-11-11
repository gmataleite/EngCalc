namespace EngCalc.Domain.StorageTanks.Models;

using System;

public class TorisphericalHeadVolume : DishedHeadVolume
{
    public const double AsmeVolumeConstant = 0.0847;

    public override double HeadHeight => 0.16934 * InternalDiameter;

    public override double InternalVolume => AsmeVolumeConstant * Math.Pow(InternalDiameter, 3);

    public TorisphericalHeadVolume(double internalDiameter) : base(internalDiameter)
    {
    }

    public override double CalculateVerticalProductVolume(double heightFromBase)
    {
        if (heightFromBase <= 0) return 0.0;
        if (heightFromBase >= HeadHeight) return InternalVolume;

        double u = heightFromBase / HeadHeight;
        return InternalVolume * 1.5 * u * (1.0 - Math.Pow(u, 2) / 3.0);
    }
}
