using Api.Models.Interfaces;

namespace Api.Models.Implementations;

public class RoutingConfiguration : IRoutingConfiguration
{
    public string? CartServiceAddress { get; set; }
    public string? ApiServiceAddress { get; set; }
}