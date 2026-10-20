namespace VesselCalc.Domain.Common;

using System;
using UnitsNet;

public static class Guard
{
    public static void AgainstNegativeOrZero(Pressure value, string parameterName)
    {
        if (value <= Pressure.Zero)
            throw new ArgumentOutOfRangeException(parameterName, $"O valor de {parameterName} deve ser estritamente maior que zero.");
    }

    public static void AgainstNegativeOrZero(Length? value, string parameterName)
    {
        if (value <= Length.Zero)
            throw new ArgumentOutOfRangeException(parameterName, $"O valor de {parameterName} deve ser estritamente maior que zero.");
    }

    public static void AgainstNegativeOrZero(double value, string parameterName)
    {
        if (value <= 0)
            throw new ArgumentOutOfRangeException(parameterName, $"O valor de {parameterName} deve ser estritamente maior que zero.");
    }

    public static void AgainstNegative(Pressure value, string parameterName)
    {
        if (value < Pressure.Zero)
            throw new ArgumentOutOfRangeException(parameterName, $"O valor de {parameterName} não pode ser negativo.");
    }

    public static void AgainstNegative(Length? value, string parameterName)
    {
        if (value < Length.Zero)
            throw new ArgumentOutOfRangeException(parameterName, $"O valor de {parameterName} não pode ser negativo.");
    }

    public static void AgainstInvalidJointEfficiency(double value, string parameterName)
    {
        if (value <= 0 || value > 1.0)
            throw new ArgumentOutOfRangeException(parameterName, "A eficiência de junta (E) deve estar entre 0 e 1.0 de acordo com o ASME VIII Div. 1.");
    }
}