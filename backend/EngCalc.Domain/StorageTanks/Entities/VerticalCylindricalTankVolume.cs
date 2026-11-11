namespace EngCalc.Domain.StorageTanks.Entities;

using System;
using EngCalc.Domain.Common;
using EngCalc.Domain.StorageTanks.Interfaces;
using EngCalc.Domain.StorageTanks.Models;
using EngCalc.Domain.StorageTanks.Enums;
using EngCalc.Domain.StorageTanks.Factories;

public class VerticalCylindricalTankVolume : IGeometryVolume
{
    public double BodyVolume { get; private set; }
    public double HeadVolume { get; private set; }
    public double BottomVolume { get; private set; }
    public double InternalVolume { get; private set; }
    public double ProductVolume { get; private set; }
    public double? ProductHeight { get; private set; }
    public double TotalTankHeight { get; private set; }

    /// <summary>
    /// Indicates whether the liquid height exceeds the cylindrical shell height (weld seam line),
    /// penetrating into the top head.
    /// </summary>
    public bool ExceedsWeldSeamLine { get; private set; }

    public VerticalCylindricalTankVolume(double bodyVolume, double headVolume, double bottomVolume)
    {
        BodyVolume = bodyVolume;
        HeadVolume = headVolume;
        BottomVolume = bottomVolume;
        InternalVolume = CalculateInternalVolume();
    }

    public VerticalCylindricalTankVolume(
        double internalDiameter, 
        double internalBodyHeight, 
        HeadTanksTypes bottomType, 
        double? bottomHeight, 
        double? bottomAngle, 
        HeadTanksTypes headType, 
        double? headHeight, 
        double? headAngle)
        : this(
            internalDiameter,
            internalBodyHeight,
            bottomType,
            bottomHeight,
            bottomAngle,
            headType,
            headHeight,
            headAngle,
            null)
    {
    }

    public VerticalCylindricalTankVolume(
        double internalDiameter, 
        double internalBodyHeight, 
        HeadTanksTypes bottomType, 
        double? bottomHeight, 
        double? bottomAngle, 
        HeadTanksTypes headType, 
        double? headHeight, 
        double? headAngle,
        double? productHeight)
    {
        Guard.AgainstNegativeOrZero(internalDiameter, nameof(internalDiameter));
        Guard.AgainstNegativeOrZero(internalBodyHeight, nameof(internalBodyHeight));

        var head = HeadVolumeFactory.Create(headType, internalDiameter, headHeight, headAngle);
        var bottom = HeadVolumeFactory.Create(bottomType, internalDiameter, bottomHeight, bottomAngle);
        var body = new CylindricalBodyVolume(internalDiameter, internalBodyHeight);

        BodyVolume = body.InternalVolume;
        HeadVolume = head.InternalVolume;
        BottomVolume = bottom.InternalVolume;
        InternalVolume = CalculateInternalVolume();

        TotalTankHeight = internalBodyHeight + head.HeadHeight + bottom.HeadHeight;

        if (productHeight.HasValue)
        {
            Guard.AgainstNegativeOrZero(productHeight.Value, nameof(productHeight));
            if (productHeight.Value > TotalTankHeight)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(productHeight),
                    $"Liquid height ({productHeight.Value}) cannot exceed total tank height ({TotalTankHeight}). Liquid volume cannot exceed total internal volume ({InternalVolume}).");
            }

            ProductHeight = productHeight.Value;
            ExceedsWeldSeamLine = productHeight.Value > internalBodyHeight;

            ProductVolume = CalculateProductVolume(
                internalDiameter, 
                internalBodyHeight,
                productHeight.Value, 
                bottomType, 
                bottomHeight, 
                bottomAngle,
                headType,
                headHeight,
                headAngle);
        }
    }

    public static double CalculateHeadVolume(HeadTanksTypes type, double internalDiameter, double? height, double? angle)
        => HeadVolumeFactory.Create(type, internalDiameter, height, angle).InternalVolume;

    public static double CalculateBottomVolume(HeadTanksTypes type, double internalDiameter, double? height, double? angle) 
        => CalculateHeadVolume(type, internalDiameter, height, angle);

    public double CalculateInternalVolume() => BodyVolume + HeadVolume + BottomVolume;

    public static double CalculateProductVolume(
        double internalDiameter,
        double productHeight, 
        HeadTanksTypes bottomType, 
        double? bottomHeight, 
        double? bottomAngle)
        => CalculateProductVolume(
            internalDiameter: internalDiameter,
            internalBodyHeight: productHeight,
            productHeight: productHeight,
            bottomType: bottomType,
            bottomHeight: bottomHeight,
            bottomAngle: bottomAngle,
            headType: HeadTanksTypes.Flat,
            headHeight: null,
            headAngle: null);

    public static double CalculateProductVolume(
        double internalDiameter,
        double internalBodyHeight,
        double productHeight, 
        HeadTanksTypes bottomType, 
        double? bottomHeight, 
        double? bottomAngle,
        HeadTanksTypes headType,
        double? headHeight,
        double? headAngle)
    {
        Guard.AgainstNegativeOrZero(internalDiameter, nameof(internalDiameter));
        Guard.AgainstNegativeOrZero(internalBodyHeight, nameof(internalBodyHeight));
        Guard.AgainstNegativeOrZero(productHeight, nameof(productHeight));

        var head = HeadVolumeFactory.Create(headType, internalDiameter, headHeight, headAngle);
        var bottom = HeadVolumeFactory.Create(bottomType, internalDiameter, bottomHeight, bottomAngle);
        var body = new CylindricalBodyVolume(internalDiameter, internalBodyHeight);
        double internalVolume = body.InternalVolume + head.InternalVolume + bottom.InternalVolume;

        double totalTankHeight = internalBodyHeight + head.HeadHeight + bottom.HeadHeight;
        if (productHeight > totalTankHeight)
        {
            throw new ArgumentOutOfRangeException(
                nameof(productHeight),
                $"Liquid height ({productHeight}) cannot exceed total tank height ({totalTankHeight}).");
        }

        double bottomVolume = bottom.InternalVolume;

        if (productHeight <= bottom.HeadHeight)
        {
            return bottom.CalculateVerticalProductVolume(productHeight);
        }

        double heightAboveBottom = productHeight - bottom.HeadHeight;

        if (heightAboveBottom <= internalBodyHeight)
        {
            double bodyLiquidVolume =
                new CylindricalBodyVolume(internalDiameter, heightAboveBottom).InternalVolume;
            return bottomVolume + bodyLiquidVolume;
        }
        else
        {
            double bodyLiquidVolume = body.InternalVolume;
            double headLiquidHeight = heightAboveBottom - internalBodyHeight;
            double headLiquidVolume = head.CalculateVerticalProductVolume(headLiquidHeight);
            return Math.Min(
                bottomVolume + bodyLiquidVolume + headLiquidVolume,
                internalVolume);
        }
    }
}
