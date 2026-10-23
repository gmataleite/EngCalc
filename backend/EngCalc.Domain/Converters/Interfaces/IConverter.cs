using UnitsNet;

namespace EngCalc.Domain.Converters.Interfaces;

public interface IConverter
{
    double ConvertValue(double initialValue, Enum initialUnit, Enum targetUnit);
}

