using EngCalc.Domain.Common;
using UnitsNet;

namespace EngCalc.Tests.Domain;

public class GuardTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void AgainstNegativeOrZero_ShouldRejectNonPositiveDouble(double value)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            Guard.AgainstNegativeOrZero(value, "value"));
    }

    [Fact]
    public void AgainstNegativeOrZero_ShouldAcceptPositiveUnitsNetValues()
    {
        Guard.AgainstNegativeOrZero(Pressure.FromMegapascals(1), "pressure");
        Guard.AgainstNegativeOrZero(Length.FromMillimeters(1), "length");
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(-10)]
    public void AgainstNegative_ShouldRejectNegativeValues(double value)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            Guard.AgainstNegative(value, "value"));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public void AgainstInvalidJointEfficiency_ShouldAcceptBoundaryValues(double value)
    {
        if (value == 0)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                Guard.AgainstInvalidJointEfficiency(value, "efficiency"));
            return;
        }

        Guard.AgainstInvalidJointEfficiency(value, "efficiency");
    }
}
