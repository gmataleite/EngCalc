namespace EngCalc.Domain.StorageTanks.Interfaces;

public interface IHeadVolume : IGeometryVolume
{
    public double InternalDiameter { get; }
    public double HeadHeight { get; }
    public double CalculateVerticalProductVolume(double heightFromBase);
}
