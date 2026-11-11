namespace EngCalc.Domain.StorageTanks.Models;

using System;

public class HemisphericalHeadVolume : DishedHeadVolume
{
    public override double HeadHeight => InternalRadius;
    public override double InternalVolume => Math.PI * Math.Pow(InternalRadius, 3) * 2.0 / 3.0;

    public HemisphericalHeadVolume(double internalDiameter) : base(internalDiameter)
    {
    }

    public override double CalculateVerticalProductVolume(double heightFromBase)
    {
        if (heightFromBase <= 0) return 0.0;
        if (heightFromBase >= HeadHeight) return InternalVolume;

        double z = heightFromBase;
        double r = InternalRadius;
        return Math.PI * z * (Math.Pow(r, 2) - Math.Pow(z, 2) / 3.0);
    }
}
