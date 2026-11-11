namespace EngCalc.Domain.StorageTanks.Models;

using System;
using EngCalc.Domain.Common;
using EngCalc.Domain.StorageTanks.Interfaces;

public class CylindricalBodyVolume : IGeometryVolume
{
    public double InternalDiameter { get; private set; }
    public double InternalHeight { get; private set; }
    public double InternalRadius => InternalDiameter / 2.0;
    public double InternalVolume => Math.PI * Math.Pow(InternalRadius, 2) * InternalHeight;

    public CylindricalBodyVolume(double internalDiameter, double internalHeight)
    {
        Guard.AgainstNegativeOrZero(internalDiameter, nameof(internalDiameter));
        Guard.AgainstNegativeOrZero(internalHeight, nameof(internalHeight));
        
        InternalDiameter = internalDiameter;
        InternalHeight = internalHeight;
    }

    /// <summary>
    /// Calculates the partial volume occupied by liquid in a horizontally positioned cylindrical body
    /// (length = InternalHeight) for a given product height from the bottom.
    /// </summary>
    /// <param name="productHeight">Liquid level height from the bottom (0 to InternalDiameter).</param>
    /// <returns>Partial product volume in the cylindrical shell.</returns>
    public double CalculateHorizontalProductVolume(double productHeight)
    {
        Guard.AgainstNegative(productHeight, nameof(productHeight));
        if (productHeight > InternalDiameter)
        {
            throw new ArgumentOutOfRangeException(
                nameof(productHeight), 
                "Product height cannot be greater than the internal diameter of the cylinder.");
        }

        if (productHeight == 0)
        {
            return 0.0;
        }

        if (productHeight == InternalDiameter)
        {
            return InternalVolume;
        }

        double r = InternalRadius;
        // Circular segment area: r² * acos((r - h) / r) - (r - h) * sqrt(2*r*h - h²)
        double segmentArea = Math.Pow(r, 2) * Math.Acos((r - productHeight) / r)
                           - (r - productHeight) * Math.Sqrt(2.0 * r * productHeight - Math.Pow(productHeight, 2));

        return segmentArea * InternalHeight;
    }
}