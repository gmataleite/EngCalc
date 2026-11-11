namespace EngCalc.Domain.StorageTanks.Models;

using System;
using EngCalc.Domain.Common;
using EngCalc.Domain.StorageTanks.Interfaces;

public abstract class DishedHeadVolume : IHeadVolume
{
    public double InternalDiameter { get; private set; }
    public double InternalRadius => InternalDiameter / 2.0;
    public abstract double InternalVolume { get; }
    public abstract double HeadHeight { get; }

    protected DishedHeadVolume(double internalDiameter)
    {
        Guard.AgainstNegativeOrZero(internalDiameter, nameof(internalDiameter));
        InternalDiameter = internalDiameter;
    }

    /// <summary>
    /// Calculates the partial volume occupied by liquid in a horizontally positioned dished head,
    /// for a product height ranging from 0 to the internal diameter.
    /// </summary>
    /// <param name="productHeight">Liquid level height from the bottom.</param>
    /// <returns>Partial product volume contained within the dished head.</returns>
    public virtual double CalculateHorizontalProductVolume(double productHeight)
    {
        Guard.AgainstNegative(productHeight, nameof(productHeight));
        if (productHeight > InternalDiameter)
        {
            throw new ArgumentOutOfRangeException(
                nameof(productHeight), 
                "Product height cannot be greater than the internal diameter of the tank.");
        }

        if (productHeight == 0)
        {
            return 0.0;
        }

        if (productHeight == InternalDiameter)
        {
            return InternalVolume;
        }

        double y = productHeight / InternalDiameter;
        double fillFraction = y * y * (3.0 - 2.0 * y);
        return InternalVolume * fillFraction;
    }

    /// <summary>
    /// Calculates the partial volume occupied by liquid in a vertically positioned dished head,
    /// from the base tangent line up to heightFromBase.
    /// </summary>
    /// <param name="heightFromBase">Liquid height within the head from its base.</param>
    /// <returns>Partial product volume contained within the vertical dished head.</returns>
    public abstract double CalculateVerticalProductVolume(double heightFromBase);
}
