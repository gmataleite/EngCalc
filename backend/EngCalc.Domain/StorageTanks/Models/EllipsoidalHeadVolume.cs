namespace EngCalc.Domain.StorageTanks.Models;

using System;
using EngCalc.Domain.Common;

public class EllipsoidalHeadVolume : DishedHeadVolume
{
    public override double HeadHeight { get; }

    public override double InternalVolume => Math.PI * Math.Pow(InternalDiameter, 2) * HeadHeight / 6.0;

    public EllipsoidalHeadVolume(double internalDiameter, double? headHeight = null) : base(internalDiameter)
    {
        if (headHeight.HasValue)
        {
            Guard.AgainstNegativeOrZero(headHeight.Value, nameof(headHeight));
        }

        HeadHeight = headHeight ?? (internalDiameter / 4.0);
    }

    public override double CalculateVerticalProductVolume(double heightFromBase)
    {
        if (heightFromBase <= 0) return 0.0;
        if (heightFromBase >= HeadHeight) return InternalVolume;

        double z = heightFromBase;
        double r = InternalRadius;
        return Math.PI * Math.Pow(r, 2) * z * (1.0 - Math.Pow(z, 2) / (3.0 * Math.Pow(HeadHeight, 2)));
    }
}
