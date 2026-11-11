using EngCalc.Application.StorageTanks.DTOs;
using EngCalc.Domain.StorageTanks.Entities;
using EngCalc.Domain.StorageTanks.Enums;
using EngCalc.Application.StorageTanks.Interfaces;
using EngCalc.Application.Common.DTOs;

namespace EngCalc.Application.StorageTanks.Services;

public class TankVolumeService : ITankVolumeService
{
    private static readonly IReadOnlyList<SelectOptionDto> TankHeadOptions = 
        Enum.GetValues<HeadTanksTypes>()
            .Select(unit => new SelectOptionDto(unit.ToString(), unit switch
            {
                HeadTanksTypes.Flat => "Flat Head",
                HeadTanksTypes.Conical => "Conical Head",
                HeadTanksTypes.Hemispherical => "Hemispherical Head",
                HeadTanksTypes.Ellipsoidal => "Ellipsoidal Head",
                HeadTanksTypes.Torispherical => "Torispherical Head",
                _ => unit.ToString()
            }))
            .ToList();

    private static readonly IReadOnlyList<SelectOptionDto> TankOrientationOptions = 
        Enum.GetValues<TankOrientation>()
            .Select(unit => new SelectOptionDto(unit.ToString(), unit switch
            {
                TankOrientation.Horizontal => "Horizontal",
                TankOrientation.Vertical => "Vertical",
                _ => unit.ToString()
            }))
            .ToList();

    public TankHeadOptionsDto GetOptions()
    {
        return new TankHeadOptionsDto(TankHeadOptions, TankOrientationOptions);
    }

    public Task<TankVolumeResponseDto> CalculateVolumeAsync(TankVolumeRequestDto request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!double.IsFinite(request.InternalDiameter) || request.InternalDiameter <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(request.InternalDiameter), $"Invalid internal diameter: {request.InternalDiameter}");
        }

        if (!double.IsFinite(request.InternalBodyHeight) || request.InternalBodyHeight <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(request.InternalBodyHeight), $"Invalid internal body height: {request.InternalBodyHeight}");
        }

        if (string.IsNullOrWhiteSpace(request.BottomType))
        {
            throw new ArgumentException("Bottom type is required.", nameof(request.BottomType));
        }

        if (string.IsNullOrWhiteSpace(request.HeadType))
        {
            throw new ArgumentException("Head type is required.", nameof(request.HeadType));
        }

        if (string.IsNullOrWhiteSpace(request.TankOrientation))
        {
            throw new ArgumentException("Tank orientation is required.", nameof(request.TankOrientation));
        }

        switch (request.TankOrientation.Trim().ToLowerInvariant())
        {
            case "horizontal":
                return Task.FromResult(CalculateHorizontalCylindricalTankVolume(request));
            case "vertical":
                return Task.FromResult(CalculateVerticalCylindricalTankVolume(request));
            default:
                throw new NotSupportedException($"Invalid tank orientation: '{request.TankOrientation}' is not supported.");
        }
    }

    public HeadTanksTypes ParseHeadType(string headTypeString, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(headTypeString) ||
            !Enum.TryParse<HeadTanksTypes>(headTypeString, true, out var headType) ||
            !Enum.IsDefined(typeof(HeadTanksTypes), headType))
        {
            throw new ArgumentException($"Invalid head type: {headTypeString}", parameterName);
        }

        return headType;
    }

    public TankVolumeResponseDto CalculateHorizontalCylindricalTankVolume(TankVolumeRequestDto request)
    {
        // Parse the bottom and head types
        var bottomType = ParseHeadType(request.BottomType, nameof(request.BottomType));
        var headType = ParseHeadType(request.HeadType, nameof(request.HeadType));

        // Calculate the volume of the horizontal cylindrical tank
        var storageTank = new HorizontalCylindricalTankVolume(
            internalDiameter: request.InternalDiameter,
            internalBodyHeight: request.InternalBodyHeight,
            bottomType: bottomType,
            bottomHeight: request.BottomHeight,
            bottomAngle: request.BottomAngle,
            headType: headType,
            headHeight: request.HeadHeight,
            headAngle: request.HeadAngle,
            productHeight: request.ProductHeight);

        return new TankVolumeResponseDto(
            InternalVolume: storageTank.InternalVolume,
            ProductVolume: storageTank.ProductVolume);
    }

    public TankVolumeResponseDto CalculateVerticalCylindricalTankVolume(TankVolumeRequestDto request)
    {
        var bottomType = ParseHeadType(request.BottomType, nameof(request.BottomType));
        var headType = ParseHeadType(request.HeadType, nameof(request.HeadType));

        // Calculate the volume of the vertical cylindrical tank
        var storageTank = new VerticalCylindricalTankVolume(
            internalDiameter: request.InternalDiameter,
            internalBodyHeight: request.InternalBodyHeight,
            bottomType: bottomType,
            bottomHeight: request.BottomHeight,
            bottomAngle: request.BottomAngle,
            headType: headType,
            headHeight: request.HeadHeight,
            headAngle: request.HeadAngle,
            productHeight: request.ProductHeight);

        return new TankVolumeResponseDto(
            InternalVolume: storageTank.InternalVolume,
            ProductVolume: storageTank.ProductVolume);
    }
}