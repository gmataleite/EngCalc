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
        public string? Notes { get; private set; }

        private readonly List<MaterialTemperatureProperty> _temperatureProperties = new();
        public IReadOnlyCollection<MaterialTemperatureProperty> TemperatureProperties => _temperatureProperties.AsReadOnly();

        // Construtor vazio exigido pelo EF Core
        #pragma warning disable CS8618 
        protected Material() { }
        #pragma warning restore CS8618

        public Material(string materialId, string specification, string? grade, string? productForm, decimal minTensile, decimal minYield, string? notes = null)
        {
            MaterialId = materialId;
            Specification = specification;
            Grade = grade;
            ProductForm = productForm;
            MinTensileStrength = minTensile;
            MinYieldStrength = minYield;
            Notes = notes;
        }

        public void AddTemperaturePoint(decimal temperature, decimal? allowableStress, decimal? yieldStress)
        {
            if (_temperatureProperties.Any(p => p.Temperature == temperature))
                throw new InvalidOperationException($"Temperatura {temperature} já cadastrada para {MaterialId}.");

            _temperatureProperties.Add(new MaterialTemperatureProperty(temperature, allowableStress, yieldStress));
        }

        // Interpolação linear conforme ASME II-D
        public decimal? GetAllowableStress(decimal temperature)
        {
            var points = _temperatureProperties
                .Where(p => p.AllowableStress.HasValue)
                .OrderBy(p => p.Temperature)
                .ToList();

            if (points.Count == 0) return null;

            var exact = points.FirstOrDefault(p => p.Temperature == temperature);
            if (exact is not null) return exact.AllowableStress;

            var lower = points.LastOrDefault(p => p.Temperature < temperature);
            var upper = points.FirstOrDefault(p => p.Temperature > temperature);

            if (lower is null || upper is null) return null; // fora da faixa / NP

            var ratio = (temperature - lower.Temperature) / (upper.Temperature - lower.Temperature);
            return lower.AllowableStress!.Value
                + ratio * (upper.AllowableStress!.Value - lower.AllowableStress.Value);
        }
    }
}