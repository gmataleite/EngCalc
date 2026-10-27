namespace EngCalc.Domain.StorageTanks.Models;

using EngCalc.Domain.Common;
using EngCalc.Domain.StorageTanks.Interfaces;

public class ConicalHeadVolume : IGeometryVolume
{
    public double InternalDiameter { get;  private set; }
    public double InternalHeight { get;  private set; }
    public double? AngleToHorizontal { get; private set; }
    public double InternalRadius { get { return InternalDiameter / 2; } }
    public double InternalVolume { get; private set; }
    

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
        return Math.PI * Math.Pow(InternalRadius, 2) * InternalHeight / 3;
    }
}