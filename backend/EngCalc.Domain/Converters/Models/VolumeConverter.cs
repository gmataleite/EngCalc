namespace EngCalc.Domain.Converters.Models;

using EngCalc.Domain.Converters.Enums;
using EngCalc.Domain.Converters.Interfaces;
using UnitsNet;

public class VolumeConverter : IConverter
{
    public double ConvertedValue { get; private set; }

    public VolumeConverter(double initialValue, VolumeUnits initialUnit, VolumeUnits targetUnit)
    {
        if (!double.IsFinite(initialValue))
        {
            throw new ArgumentOutOfRangeException(nameof(initialValue), $"Invalid initial value: {initialValue}");
        }
        if (!Enum.IsDefined(typeof(VolumeUnits), initialUnit))
        {
            throw new ArgumentException($"Invalid initial unit: {initialUnit}", nameof(initialUnit));
        }
        if (!Enum.IsDefined(typeof(VolumeUnits), targetUnit))
        {
            throw new ArgumentException($"Invalid target unit: {targetUnit}", nameof(targetUnit));
        }
        if (initialValue < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(initialValue), $"Volume cannot be negative: {initialValue}");
        }

        ConvertValue(initialValue, initialUnit, targetUnit);
    }

    public double ConvertValue(double initialValue, Enum initialUnit, Enum targetUnit)
    {
        switch (initialUnit, targetUnit)
        {
            case (VolumeUnits u1, VolumeUnits u2) when u1 == u2:
                ConvertedValue = Math.Round(initialValue, 2);
                break;
            case (VolumeUnits.CubicMeters, VolumeUnits.Liters):
                ConvertedValue = CubicMetersToLiters(initialValue);
                break;
            case (VolumeUnits.Liters, VolumeUnits.CubicMeters):
                ConvertedValue = LitersToCubicMeters(initialValue);
                break;
            case (VolumeUnits.CubicMeters, VolumeUnits.Gallons):
                ConvertedValue = CubicMetersToGallons(initialValue);
                break;
            case (VolumeUnits.Gallons, VolumeUnits.CubicMeters):
                ConvertedValue = GallonsToCubicMeters(initialValue);
                break;
            case (VolumeUnits.Liters, VolumeUnits.Gallons):
                ConvertedValue = LitersToGallons(initialValue);
                break;
            case (VolumeUnits.Gallons, VolumeUnits.Liters):
                ConvertedValue = GallonsToLiters(initialValue);
                break;
            default:
                throw new ArgumentException("Invalid volume units provided.");
        }
        return ConvertedValue;
    }

    public double CubicMetersToLiters(double initialValue) => Math.Round(Volume.FromCubicMeters(initialValue).Liters, 2);
    public double LitersToCubicMeters(double initialValue) => Math.Round(Volume.FromLiters(initialValue).CubicMeters, 2);
    public double CubicMetersToGallons(double initialValue) => Math.Round(Volume.FromCubicMeters(initialValue).UsGallons, 2);
    public double GallonsToCubicMeters(double initialValue) => Math.Round(Volume.FromUsGallons(initialValue).CubicMeters, 2);
    public double LitersToGallons(double initialValue) => Math.Round(Volume.FromLiters(initialValue).UsGallons, 2);
    public double GallonsToLiters(double initialValue) => Math.Round(Volume.FromUsGallons(initialValue).Liters, 2);
}
