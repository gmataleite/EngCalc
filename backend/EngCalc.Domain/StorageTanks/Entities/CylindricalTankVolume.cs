namespace EngCalc.Domain.StorageTanks.Entities;
using EngCalc.Domain.Common;
using EngCalc.Domain.StorageTanks.Interfaces;
using EngCalc.Domain.StorageTanks.Models;
using EngCalc.Domain.StorageTanks.Enums;

public class CylindricalTankVolume : IGeometryVolume
{
    public double BodyVolume { get;  private set; }
    public double HeadVolume { get;  private set; }
    public double BottomVolume { get;  private set; }
    public double InternalVolume { get; private set; }
    public double ProductVolume { get; private set; }

    public CylindricalTankVolume(double bodyVolume, double headVolume, double bottomVolume)
    {
        BodyVolume = bodyVolume;
        HeadVolume = headVolume;
        BottomVolume = bottomVolume;
        InternalVolume = CalculateInternalVolume();
    }

    public CylindricalTankVolume(
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

    public CylindricalTankVolume(
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


        BodyVolume = new CylindricalBodyVolume(internalDiameter, internalBodyHeight).InternalVolume;

        HeadVolume = CalculateHeadVolume(headType, internalDiameter, headHeight, headAngle);
        BottomVolume = CalculateBottomVolume(bottomType, internalDiameter, bottomHeight, bottomAngle);
        InternalVolume = CalculateInternalVolume();

        if (productHeight.HasValue){
            Guard.AgainstNegativeOrZero(productHeight.Value, nameof(productHeight));
            ProductVolume = CalculateProductVolume(
            internalDiameter, 
            productHeight.Value, 
            bottomType, 
            bottomHeight, 
            bottomAngle);
        }
    }

    public static double CalculateHeadVolume(HeadTanksTypes type, double internalDiameter, double? height, double? angle)
    {
        if (type == HeadTanksTypes.Conical)
        {
            return new ConicalHeadVolume(internalDiameter, angle, height).InternalVolume;
        }
        else if (type == HeadTanksTypes.Flat)
        {
            return new FlatHeadVolume(internalDiameter).InternalVolume;
        }
        else if (type == HeadTanksTypes.Spherical)
        {
            return new SphericalHeadVolume(internalDiameter).InternalVolume;
        }
        else
        {
            throw new ArgumentException("Invalid head or bottom type.");
        }
    }

    public static double CalculateBottomVolume(HeadTanksTypes type, double internalDiameter, double? height, double? angle) => CalculateHeadVolume(type, internalDiameter, height, angle);

    public double CalculateInternalVolume() => BodyVolume + HeadVolume + BottomVolume;

    public static double CalculateProductVolume(
        double internalDiameter,
        double productHeight, 
        HeadTanksTypes bottomType, 
        double? bottomHeight, 
        double? bottomAngle) => new CylindricalTankVolume(
        internalDiameter: internalDiameter,
        internalBodyHeight: productHeight,
        bottomType: bottomType,
        bottomHeight: bottomHeight,
        bottomAngle: bottomAngle,
        headType: HeadTanksTypes.Flat,
        headHeight: 0,
        headAngle: 0).CalculateInternalVolume();
}