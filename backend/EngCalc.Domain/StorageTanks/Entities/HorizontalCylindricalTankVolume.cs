namespace EngCalc.Domain.StorageTanks.Entities;

using System;
using EngCalc.Domain.Common;
using EngCalc.Domain.StorageTanks.Enums;
using EngCalc.Domain.StorageTanks.Factories;
using EngCalc.Domain.StorageTanks.Interfaces;
using EngCalc.Domain.StorageTanks.Models;

public class HorizontalCylindricalTankVolume : IGeometryVolume
{
    public double BodyVolume { get; private set; }
    public double HeadVolume { get; private set; }
    public double TotalHeadsVolume => HeadVolume * 2.0;

    [Obsolete("In horizontal cylindrical tanks, both ends are identical Heads. Use HeadVolume or TotalHeadsVolume.")]
    public double BottomVolume => HeadVolume;

    public double InternalVolume { get; private set; }
    public double ProductVolume { get; private set; }

    public HorizontalCylindricalTankVolume(double bodyVolume, double headVolume)
    {
        BodyVolume = bodyVolume;
        HeadVolume = headVolume;
        InternalVolume = CalculateInternalVolume();
    }

    public HorizontalCylindricalTankVolume(double bodyVolume, double headVolume, double bottomVolume)
        : this(bodyVolume, headVolume)
    {
    }

    public HorizontalCylindricalTankVolume(
        double internalDiameter, 
        double internalBodyLength, 
        HeadTanksTypes headType, 
        double? headHeight = null, 
        double? productHeight = null)
    {
        Guard.AgainstNegativeOrZero(internalDiameter, nameof(internalDiameter));
        Guard.AgainstNegativeOrZero(internalBodyLength, nameof(internalBodyLength));
        ValidateHorizontalHeadType(headType);

        var body = new CylindricalBodyVolume(internalDiameter, internalBodyLength);
        var head = (DishedHeadVolume)HeadVolumeFactory.Create(headType, internalDiameter, headHeight);

        BodyVolume = body.InternalVolume;
        HeadVolume = head.InternalVolume;
        InternalVolume = CalculateInternalVolume();

        if (productHeight.HasValue)
        {
            ProductVolume = CalculateProductVolume(
                internalDiameter, 
                internalBodyLength, 
                headType, 
                headHeight, 
                productHeight.Value);
        }
    }

    public HorizontalCylindricalTankVolume(
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
            headType,
            headHeight,
            null)
    {
    }

    public HorizontalCylindricalTankVolume(
        double internalDiameter, 
        double internalBodyHeight, 
        HeadTanksTypes bottomType, 
        double? bottomHeight, 
        double? bottomAngle, 
        HeadTanksTypes headType, 
        double? headHeight, 
        double? headAngle,
        double? productHeight)
        : this(
            internalDiameter,
            internalBodyHeight,
            headType,
            headHeight,
            productHeight)
    {
    }

    public static void ValidateHorizontalHeadType(HeadTanksTypes type)
    {
        if (type != HeadTanksTypes.Hemispherical &&
            type != HeadTanksTypes.Ellipsoidal &&
            type != HeadTanksTypes.Torispherical)
        {
            throw new ArgumentException(
                "For horizontal cylindrical tanks, only Hemispherical, Ellipsoidal, or Torispherical heads are supported.");
        }
    }

    public static double CalculateHeadVolume(HeadTanksTypes type, double internalDiameter, double? height = null, double? angle = null)
    {
        ValidateHorizontalHeadType(type);
        return HeadVolumeFactory.Create(type, internalDiameter, height, angle).InternalVolume;
    }

    [Obsolete("In horizontal cylindrical tanks, both ends are identical Heads. Use CalculateHeadVolume.")]
    public static double CalculateBottomVolume(HeadTanksTypes type, double internalDiameter, double? height = null, double? angle = null)
        => CalculateHeadVolume(type, internalDiameter, height, angle);

    public static double CalculateBodyProductVolume(double internalDiameter, double internalBodyLength, double productHeight)
    {
        var body = new CylindricalBodyVolume(internalDiameter, internalBodyLength);
        return body.CalculateHorizontalProductVolume(productHeight);
    }

    public static double CalculateHeadProductVolume(
        HeadTanksTypes type, 
        double internalDiameter, 
        double productHeight, 
        double? headHeight = null)
    {
        ValidateHorizontalHeadType(type);
        var head = (DishedHeadVolume)HeadVolumeFactory.Create(type, internalDiameter, headHeight);
        return head.CalculateHorizontalProductVolume(productHeight);
    }

    public static double CalculateProductVolume(
        double internalDiameter,
        double internalBodyLength, 
        HeadTanksTypes headType, 
        double? headHeight, 
        double productHeight)
    {
        ValidateHorizontalHeadType(headType);

        var body = new CylindricalBodyVolume(internalDiameter, internalBodyLength);
        var head = (DishedHeadVolume)HeadVolumeFactory.Create(headType, internalDiameter, headHeight);

        double bodyProductVolume = body.CalculateHorizontalProductVolume(productHeight);
        double headsProductVolume = 2.0 * head.CalculateHorizontalProductVolume(productHeight);

        return bodyProductVolume + headsProductVolume;
    }

    public double CalculateInternalVolume() => BodyVolume + TotalHeadsVolume;
}
