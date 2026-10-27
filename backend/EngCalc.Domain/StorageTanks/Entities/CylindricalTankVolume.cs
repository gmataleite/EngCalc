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
    {
        Guard.AgainstNegativeOrZero(internalDiameter, nameof(internalDiameter));
        Guard.AgainstNegativeOrZero(internalBodyHeight, nameof(internalBodyHeight));
        
        BodyVolume = new CylindricalBodyVolume(internalDiameter, internalBodyHeight).InternalVolume;

        HeadVolume = CalculateHeadOrBottomVolume(headType, internalDiameter, headHeight, headAngle);
        BottomVolume = CalculateHeadOrBottomVolume(bottomType, internalDiameter, bottomHeight, bottomAngle);

    }
    public double CalculateInternalVolume()
    {
        return BodyVolume + HeadVolume + BottomVolume;
    }

    public double CalculateHeadOrBottomVolume(HeadTanksTypes type, double internalDiameter, double? height, double? angle)
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
}