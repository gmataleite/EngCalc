namespace VesselCalc.Domain.Entities
{
    public class Material
    {
        public string MaterialId { get; private set; }
        public string Specification { get; private set; }
        public string? Grade { get; private set; }
        public string? ProductForm { get; private set; }
        public decimal MinTensileStrength { get; private set; }
        public decimal MinYieldStrength { get; private set; }
        public decimal MaxTemperatureLimit { get; private set; }
        public string? Notes { get; private set; }

        private readonly List<MaterialTemperatureProperty> _temperatureProperties = new();
        public IReadOnlyCollection<MaterialTemperatureProperty> TemperatureProperties => _temperatureProperties.AsReadOnly();

        // Parameterless constructor required by EF Core
        #pragma warning disable CS8618 
        protected Material() { }
        #pragma warning restore CS8618

        public Material(string materialId, string specification, string? grade,
            string? productForm, decimal minTensile, decimal minYield,
            decimal maxTemperatureLimit, string? notes = null)
        {
            MaterialId = materialId;
            Specification = specification;
            Grade = grade;
            ProductForm = productForm;
            MinTensileStrength = minTensile;
            MinYieldStrength = minYield;
            MaxTemperatureLimit = maxTemperatureLimit;
            Notes = notes;
        }

        public void AddTemperaturePoint(decimal temperature, decimal? allowableStress, decimal? yieldStress)
        {
            if (_temperatureProperties.Any(p => p.Temperature == temperature))
                throw new InvalidOperationException($"Temperature {temperature} is already registered for {MaterialId}.");

            _temperatureProperties.Add(new MaterialTemperatureProperty(temperature, allowableStress, yieldStress));
        }

        // Linear interpolation per ASME II-D
        public decimal? GetAllowableStress(decimal temperature)
        {
            if (temperature > MaxTemperatureLimit) return null;

            var points = _temperatureProperties
                .Where(p => p.AllowableStress.HasValue)
                .OrderBy(p => p.Temperature)
                .ToList();

            if (points.Count == 0) return null;

            // ASME II-D Table 1A first column (-20 to 100) applies to temperatures at or below the first point
            if (temperature <= points[0].Temperature) return points[0].AllowableStress;

            var exact = points.FirstOrDefault(p => p.Temperature == temperature);
            if (exact is not null) return exact.AllowableStress;

            var lower = points.LastOrDefault(p => p.Temperature < temperature);
            var upper = points.FirstOrDefault(p => p.Temperature > temperature);

            if (lower is null || upper is null) return null; // out of range / NP

            var ratio = (temperature - lower.Temperature) / (upper.Temperature - lower.Temperature);
            return lower.AllowableStress!.Value
                + ratio * (upper.AllowableStress!.Value - lower.AllowableStress.Value);
        }
    }
}
