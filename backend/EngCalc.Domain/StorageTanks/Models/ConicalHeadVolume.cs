namespace EngCalc.Domain.StorageTanks.Models;

using System;
using EngCalc.Domain.Common;
using EngCalc.Domain.StorageTanks.Interfaces;

public class ConicalHeadVolume : IHeadVolume
{
    public double InternalDiameter { get; private set; }
    public double InternalHeight { get; private set; }
    public double? AngleToHorizontal { get; private set; }
    public double InternalRadius => InternalDiameter / 2.0;
    public double InternalVolume { get; private set; }
    public double HeadHeight => InternalHeight;

    public ConicalHeadVolume(double internalDiameter, double? angleToHorizontal, double? internalHeight)
    {
        Guard.AgainstNegativeOrZero(internalDiameter, nameof(internalDiameter));
        InternalDiameter = internalDiameter;

        if (internalHeight == null && angleToHorizontal == null)
            throw new ArgumentException("You should enter at least one value: internal height or angle to the horizontal.");
        if (internalHeight != null && angleToHorizontal != null)
            throw new ArgumentException("Enter only one value: internal height or angle to the horizontal.");
        if (internalHeight == null && angleToHorizontal != null)
        {
            double angleInRadians = (double)angleToHorizontal * (Math.PI / 180); // Convert to radians
            internalHeight = InternalRadius * Math.Tan(angleInRadians);
        }
        if (internalHeight != null && angleToHorizontal == null)
        {
            double angleInRadians = Math.Atan((double)internalHeight / InternalRadius);
            angleToHorizontal = angleInRadians * (180 / Math.PI); // Convert to degrees
        }

        InternalHeight = internalHeight ?? 0;    
        AngleToHorizontal = angleToHorizontal;
        InternalVolume = CalculateInternalVolume();
    }

    public double CalculateInternalVolume()
    {
        return Math.PI * Math.Pow(InternalRadius, 2) * InternalHeight / 3.0;
    }

    public double CalculateVerticalProductVolume(double heightFromBase)
    {
        if (heightFromBase <= 0) return 0.0;
        if (heightFromBase >= InternalHeight) return InternalVolume;

        return InternalVolume * (1.0 - Math.Pow(1.0 - (heightFromBase / InternalHeight), 3.0));
    }
}
