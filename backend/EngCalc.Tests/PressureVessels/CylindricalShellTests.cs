using System;
using System.Collections.Generic;
using UnitsNet;
using Xunit;
using EngCalc.Domain.PressureVessels;

namespace EngCalc.Tests.PressureVessels;

public class CylindricalShellTests
{
    public static IEnumerable<object[]> ValidShellTestData => new List<object[]>
    {
        // CENÁRIO 1: Casco Fino (Thin Shell) - Governado por Tensão Circunferencial (UG-27)
        // P = 2 MPa, S = 138 MPa, CA = 3 mm. P < 0.385*SE (2 < 53.13).
        // tr = (2 * 1000) / (138*1 - 0.6*2) = 14.6198 mm. Total com CA = 17.620 mm.
        new object[] { 2.0, 2000.0, 138.0, 3.0, 1.0, 0.0, 17.620 },

        // CENÁRIO 2: Casco Espesso (Thick Shell) - Pelo Limite de Pressão (Appendix 1-2)
        // P = 60 MPa, S = 138 MPa, CA = 0 mm. P > 0.385*SE (60 > 53.13). Gatilho acionado.
        // Z = (138 + 60) / (138 - 60) = 2.5384. tr = 1000 * (sqrt(2.5384) - 1) = 593.255 mm.
        new object[] { 60.0, 2000.0, 138.0, 0.0, 1.0, 0.0, 593.255 },

        // CENÁRIO 3: Casco Fino com Pressão de Coluna de Fluido (UG-22)
        // P_projeto = 1.5 MPa. P_fluido = 0.098 MPa. P_efetivo = 1.598 MPa. E = 0.85 (RT Parcial).
        // tr = (1.598 * 1000) / (138*0.85 - 0.6*1.598) = 13.736 mm. 
        new object[] { 1.5, 2000.0, 138.0, 0.0, 0.85, 0.098, 13.735 }
    };

    [Theory]
    [MemberData(nameof(ValidShellTestData))]
    public void CalculateMinimumRequiredThickness_ValidInputs_ShouldReturnCorrectThickness(
        double designPressureMpa,
        double internalDiameterMillimeters,
        double allowableStressMpa,
        double allowableCorrosionMillimeters,
        double jointEfficiency,
        double fluidColumnPressureMpa,
        double expectedThicknessMillimeters)
    {
        // Arrange
        var designPressure = Pressure.FromMegapascals(designPressureMpa);
        var internalDiameter = Length.FromMillimeters(internalDiameterMillimeters);
        var allowableStress = Pressure.FromMegapascals(allowableStressMpa);
        var allowableCorrosion = Length.FromMillimeters(allowableCorrosionMillimeters);
        var fluidColumnPressure = Pressure.FromMegapascals(fluidColumnPressureMpa);

        var shell = new CylindricalShell(
            designPressure,
            internalDiameter,
            allowableStress,
            allowableCorrosion,
            jointEfficiency,
            fluidColumnPressure);

        // Act
        var actualThickness = shell.CalculateMinimumRequiredThickness();

        // Assert 
        Assert.Equal(expectedThicknessMillimeters, actualThickness.Millimeters, 3);
    }

    public static IEnumerable<object[]> ValidShellMawpTestData => new List<object[]>
    {
        // CENÁRIO 1: Casco Fino (Thin Shell) - Inverso do Cenário 1 de Espessura (UG-27)
        // t_nom = 17.61988 mm, CA = 3.0 mm => t = 14.61988 mm. D = 2000 mm (R = 1000 mm), S = 138 MPa, E = 1.0.
        // MAWP esperado = 2.000 MPa.
        new object[] { 17.61988, 2000.0, 138.0, 3.0, 1.0, 0.0, 2.000 },

        // CENÁRIO 2: Casco Espesso (Thick Shell) - Inverso do Cenário 2 de Espessura (Appendix 1-2)
        // t_nom = 593.25501 mm, CA = 0.0 mm => t > 0.5*R (593.255 > 500).
        // MAWP esperado = 60.000 MPa.
        new object[] { 593.25501, 2000.0, 138.0, 0.0, 1.0, 0.0, 60.000 },

        // CENÁRIO 3: Casco Fino com Pressão de Coluna de Fluido (UG-22)
        // t_nom = 13.73545 mm, CA = 0.0 mm, E = 0.85, P_fluido = 0.098 MPa.
        // P_efetivo = 1.598 MPa => MAWP = 1.598 - 0.098 = 1.500 MPa.
        new object[] { 13.73545, 2000.0, 138.0, 0.0, 0.85, 0.098, 1.500 }
    };

    [Theory]
    [MemberData(nameof(ValidShellMawpTestData))]
    public void CalculateMaximumAllowableWorkingPressure_ValidInputs_ShouldReturnCorrectMawp(
        double nominalThicknessMillimeters,
        double internalDiameterMillimeters,
        double allowableStressMpa,
        double allowableCorrosionMillimeters,
        double jointEfficiency,
        double fluidColumnPressureMpa,
        double expectedMawpMpa)
    {
        // Arrange
        var nominalThickness = Length.FromMillimeters(nominalThicknessMillimeters);
        var internalDiameter = Length.FromMillimeters(internalDiameterMillimeters);
        var allowableStress = Pressure.FromMegapascals(allowableStressMpa);
        var allowableCorrosion = Length.FromMillimeters(allowableCorrosionMillimeters);
        var fluidColumnPressure = Pressure.FromMegapascals(fluidColumnPressureMpa);

        var shell = new CylindricalShell(
            nominalThickness,
            internalDiameter,
            allowableStress,
            allowableCorrosion,
            jointEfficiency,
            fluidColumnPressure);

        // Act
        var actualMawp = shell.CalculateMaximumAllowableWorkingPressure();

        // Assert
        Assert.Equal(expectedMawpMpa, actualMawp.Megapascals, 3);
    }

    public static IEnumerable<object[]> InvalidShellTestData => new List<object[]>
    {
        // CENÁRIO 1: Pressão Negativa (P < 0)
        new object[] { -2.0, 2000.0, 138.0, 3.0, 1.0, 0.0, "maior que zero" },

        // CENÁRIO 2: Diâmetro Interno Negativo (D < 0)
        new object[] { 2.0, -2000.0, 138.0, 3.0, 1.0, 0.0, "maior que zero" },

        // CENÁRIO 3: Pressão Admissível Negativa (S < 0)
        new object[] { 2.0, 2000.0, -138.0, 3.0, 1.0, 0.0, "maior que zero" },

        // CENÁRIO 4: Corrosão Admissível Negativa (CA < 0)
        new object[] { 2.0, 2000.0, 138.0, -3.0, 1.0, 0.0, "não pode ser negativo" },

        // CENÁRIO 5: Eficiência de Junta Inválida (E < 0 ou E > 1)
        new object[] { 2.0, 2000.0, 138.0, 3.0, -0.1, 0.0, "A eficiência de junta (E) deve estar entre 0 e 1.0" },
        new object[] { 2.0, 2000.0, 138.0, 3.0, 1.1, 0.0, "A eficiência de junta (E) deve estar entre 0 e 1.0" },

        // CENÁRIO 6: Pressão de Coluna de Fluido Negativa (P_fluido < 0)
        new object[] { 2.0, 2000.0, 138.0, 3.0, 1.0, -0.1, "não pode ser negativo" }
    };

    [Theory]
    [MemberData(nameof(InvalidShellTestData))]
    public void Constructor_InvalidParameters_ShouldThrowArgumentOutOfRangeException(
        double designPressureMpa,
        double internalDiameterMillimeters,
        double allowableStressMpa,
        double allowableCorrosionMillimeters,
        double jointEfficiency,
        double fluidColumnPressureMpa,
        string expectedExceptionMessagePart)
    {
        // Arrange
        var designPressure = Pressure.FromMegapascals(designPressureMpa);
        var internalDiameter = Length.FromMillimeters(internalDiameterMillimeters);
        var allowableStress = Pressure.FromMegapascals(allowableStressMpa);
        var allowableCorrosion = Length.FromMillimeters(allowableCorrosionMillimeters);
        var fluidColumnPressure = Pressure.FromMegapascals(fluidColumnPressureMpa);

        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => 
            new CylindricalShell(
                designPressure,
                internalDiameter,
                allowableStress,
                allowableCorrosion,
                jointEfficiency,
                fluidColumnPressure)
        );
        
        Assert.Contains(expectedExceptionMessagePart, exception.Message);
    }

    [Theory]
    [InlineData(3.0, 3.0)]
    [InlineData(2.5, 3.0)]
    public void MawpConstructor_WhenNominalThicknessIsLessThanOrEqualToCorrosionAllowance_ShouldThrowArgumentOutOfRangeException(
        double nominalThicknessMm,
        double corrosionAllowanceMm)
    {
        // Arrange
        var nominalThickness = Length.FromMillimeters(nominalThicknessMm);
        var internalDiameter = Length.FromMillimeters(2000.0);
        var allowableStress = Pressure.FromMegapascals(138.0);
        var allowableCorrosion = Length.FromMillimeters(corrosionAllowanceMm);
        var fluidColumnPressure = Pressure.Zero;

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new CylindricalShell(
                nominalThickness,
                internalDiameter,
                allowableStress,
                allowableCorrosion,
                1.0,
                fluidColumnPressure));
    }
}
