namespace EngCalc.Domain.PressureVessels;

using System;
using EngCalc.Domain.Common;
using UnitsNet;

public class TorisphericalHead
{
    private readonly Pressure _designPressure;
    private readonly Length _nominalThickness;
    private readonly Length _crownRadius;
    private readonly Length _knuckleRadius;
    private readonly double _jointEfficiency;
    private readonly Pressure _allowableStress;
    private readonly Length _allowableCorrosion;
    private readonly Pressure _fluidColumnPressure;
    private readonly double _factorM;
        
    private Pressure EffectivePressure => _designPressure + _fluidColumnPressure;
    private Length CorrodedThickness => _nominalThickness - _allowableCorrosion;

    public TorisphericalHead(
        Pressure designPressure, 
        Length crownRadius, 
        Length knuckleRadius, 
        Pressure allowableStress, 
        Length allowableCorrosion, 
        double jointEfficiency, 
        Pressure fluidColumnPressure)
    {
        Guard.AgainstNegativeOrZero(designPressure, nameof(designPressure));
        Guard.AgainstNegativeOrZero(crownRadius, nameof(crownRadius));
        Guard.AgainstNegativeOrZero(knuckleRadius, nameof(knuckleRadius));
        Guard.AgainstNegativeOrZero(allowableStress, nameof(allowableStress));
        Guard.AgainstNegative(allowableCorrosion, nameof(allowableCorrosion));
        Guard.AgainstNegative(fluidColumnPressure, nameof(fluidColumnPressure));
        Guard.AgainstInvalidJointEfficiency(jointEfficiency, nameof(jointEfficiency));

        if (knuckleRadius.Millimeters < 0.06 * crownRadius.Millimeters)
            throw new ArgumentOutOfRangeException(nameof(knuckleRadius), "O raio da junta (r) não pode ser menor que 6% do raio da coroa (L) conforme UG-32(e).");
        if (knuckleRadius.Millimeters > 0.5 * crownRadius.Millimeters)
            throw new ArgumentOutOfRangeException(nameof(knuckleRadius), "O raio da junta (r) não pode ser maior que 50% do raio da coroa (L) conforme UG-32(e).");

        _designPressure = designPressure;
        _crownRadius = crownRadius;
        _knuckleRadius = knuckleRadius;
        _allowableStress = allowableStress;
        _allowableCorrosion = allowableCorrosion;
        _jointEfficiency = jointEfficiency;
        _fluidColumnPressure = fluidColumnPressure;

        _factorM = CalculateFactorM(_crownRadius, _knuckleRadius);
    }

    public TorisphericalHead(
        Pressure designPressure, 
        Length internalDiameter, 
        Pressure allowableStress, 
        Length allowableCorrosion, 
        double jointEfficiency, 
        Pressure fluidColumnPressure, double ratio) 
        : this(
            designPressure, 
            crownRadius: internalDiameter,                       // L = D
            knuckleRadius: internalDiameter * ratio,             // r = 6% de D
            allowableStress, 
            allowableCorrosion, 
            jointEfficiency, 
            fluidColumnPressure)
    {
        Guard.AgainstNegativeOrZero(internalDiameter, nameof(internalDiameter));
        Guard.AgainstNegativeOrZero(ratio, nameof(ratio));

        if (ratio < 0.06)
            throw new ArgumentOutOfRangeException(nameof(ratio), "O raio da junta (r) não pode ser menor que 6% do diâmetro interno (D) conforme UG-32(e).");
        if (ratio > 0.5)            
            throw new ArgumentOutOfRangeException(nameof(ratio), "O raio da junta (r) não pode ser maior que 50% do diâmetro interno (D) conforme UG-32(e).");
    }

    public TorisphericalHead(
        Length nominalThickness,
        Length crownRadius,
        Length knuckleRadius,
        Pressure allowableStress,
        Length allowableCorrosion,
        double jointEfficiency,
        Pressure fluidColumnPressure)
    {
        Guard.AgainstNegativeOrZero(nominalThickness, nameof(nominalThickness));
        Guard.AgainstNegativeOrZero(crownRadius, nameof(crownRadius));
        Guard.AgainstNegativeOrZero(knuckleRadius, nameof(knuckleRadius));
        Guard.AgainstNegativeOrZero(allowableStress, nameof(allowableStress));
        Guard.AgainstNegative(allowableCorrosion, nameof(allowableCorrosion));
        Guard.AgainstNegative(fluidColumnPressure, nameof(fluidColumnPressure));
        Guard.AgainstInvalidJointEfficiency(jointEfficiency, nameof(jointEfficiency));

        if (nominalThickness <= allowableCorrosion)
            throw new ArgumentOutOfRangeException(nameof(nominalThickness), "A espessura nominal (t) deve ser estritamente maior que a sobreespessura de corrosão (CA).");

        if (knuckleRadius.Millimeters < 0.06 * crownRadius.Millimeters)
            throw new ArgumentOutOfRangeException(nameof(knuckleRadius), "O raio da junta (r) não pode ser menor que 6% do raio da coroa (L) conforme UG-32(e).");
        if (knuckleRadius.Millimeters > 0.5 * crownRadius.Millimeters)
            throw new ArgumentOutOfRangeException(nameof(knuckleRadius), "O raio da junta (r) não pode ser maior que 50% do raio da coroa (L) conforme UG-32(e).");

        _nominalThickness = nominalThickness;
        _crownRadius = crownRadius;
        _knuckleRadius = knuckleRadius;
        _allowableStress = allowableStress;
        _allowableCorrosion = allowableCorrosion;
        _jointEfficiency = jointEfficiency;
        _fluidColumnPressure = fluidColumnPressure;

        _factorM = CalculateFactorM(_crownRadius, _knuckleRadius);
    }

    public TorisphericalHead(
        Length nominalThickness,
        Length internalDiameter,
        Pressure allowableStress,
        Length allowableCorrosion,
        double jointEfficiency,
        Pressure fluidColumnPressure,
        double ratio)
        : this(
            nominalThickness,
            crownRadius: internalDiameter,
            knuckleRadius: internalDiameter * ratio,
            allowableStress,
            allowableCorrosion,
            jointEfficiency,
            fluidColumnPressure)
    {
        Guard.AgainstNegativeOrZero(internalDiameter, nameof(internalDiameter));
        Guard.AgainstNegativeOrZero(ratio, nameof(ratio));

        if (ratio < 0.06)
            throw new ArgumentOutOfRangeException(nameof(ratio), "O raio da junta (r) não pode ser menor que 6% do diâmetro interno (D) conforme UG-32(e).");
        if (ratio > 0.5)
            throw new ArgumentOutOfRangeException(nameof(ratio), "O raio da junta (r) não pode ser maior que 50% do diâmetro interno (D) conforme UG-32(e).");
    }

    public Length CalculateMinimumRequiredThickness() // ASME VIII Div. 1, Appendix 1
    {
        double P = EffectivePressure.Megapascals;
        double L = _crownRadius.Millimeters;
        double S = _allowableStress.Megapascals;
        double E = _jointEfficiency;
        double M = _factorM;

        double denominator = 2.0 * S * E - 0.2 * P;
        if (denominator <= 0 || P >= S * E)
        {
            throw new InvalidOperationException(
                "Effective design pressure (P) exceeds the allowable membrane stress limit (S·E) for torispherical heads.");
        }

        double thickness = P * L * M / denominator;
        
        return Length.FromMillimeters(thickness) + _allowableCorrosion;
    }

    public Pressure CalculateMaximumAllowableWorkingPressure() // ASME VIII Div. 1, UG-32(d) / Appendix 1-4(d)
    {
        Length corrodedThickness = CorrodedThickness;
        if (corrodedThickness <= Length.Zero)
        {
            throw new InvalidOperationException(
                "Corroded thickness (nominal thickness minus corrosion allowance) must be strictly greater than zero.");
        }

        double t = corrodedThickness.Millimeters;
        double L = _crownRadius.Millimeters;
        double S = _allowableStress.Megapascals;
        double E = _jointEfficiency;
        double M = _factorM;

        double denominator = L * M + 0.2 * t;
        if (denominator <= 0)
        {
            throw new InvalidOperationException(
                "Invalid geometry for torispherical head MAWP calculation.");
        }

        double effectivePressureMpa = (2.0 * S * E * t) / denominator;
        Pressure mawp = Pressure.FromMegapascals(effectivePressureMpa) - _fluidColumnPressure;

        if (mawp <= Pressure.Zero)
        {
            throw new InvalidOperationException(
                "Calculated Maximum Allowable Working Pressure (MAWP) cannot sustain the static fluid column pressure.");
        }

        return mawp;
    }

    public Pressure CalculateMAWP() => CalculateMaximumAllowableWorkingPressure();

    private double CalculateFactorM(Length crownRadius, Length knuckleRadius) =>
        (3.0 + Math.Sqrt(crownRadius / knuckleRadius)) / 4;
}
