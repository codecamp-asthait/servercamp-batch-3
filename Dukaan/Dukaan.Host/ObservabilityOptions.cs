using System.ComponentModel.DataAnnotations;

namespace Dukaan.Host;

public class ObservabilityOptions
{
    public const string SectionName = "Observability";

    [Required]
    public string OtlpEndpoint { get; set; } = "http://localhost:4317";

    [Required]
    public string ServiceName { get; set; } = "dukaan-api";

    [Required]
    public string Environment { get; set; } = "Development";
}
