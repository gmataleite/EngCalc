using System;
using System.Collections.Generic;
using UnitsNet;
using Xunit;
using VesselCalc.Domain;

namespace VesselCalc.Tests;

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

}