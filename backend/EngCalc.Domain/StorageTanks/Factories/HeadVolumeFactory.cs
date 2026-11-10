namespace EngCalc.Domain.StorageTanks.Factories;

using System;
using EngCalc.Domain.StorageTanks.Enums;
using EngCalc.Domain.StorageTanks.Interfaces;
using EngCalc.Domain.StorageTanks.Models;

public static class HeadVolumeFactory
{
    public static IHeadVolume Create(
        HeadTanksTypes type, 
        double internalDiameter, 
        double? height = null, 
        double? angle = null)
    {
        return type switch
        {
            HeadTanksTypes.Flat => new FlatHeadVolume(internalDiameter),
            HeadTanksTypes.Conical => new ConicalHeadVolume(internalDiameter, angle, height),
            HeadTanksTypes.Hemispherical => new HemisphericalHeadVolume(internalDiameter),
            HeadTanksTypes.Ellipsoidal => new EllipsoidalHeadVolume(internalDiameter, height),
            HeadTanksTypes.Torispherical => new TorisphericalHeadVolume(internalDiameter),
            _ => throw new ArgumentException($"Unknown or invalid head type: {type}")
        };
    }
}
