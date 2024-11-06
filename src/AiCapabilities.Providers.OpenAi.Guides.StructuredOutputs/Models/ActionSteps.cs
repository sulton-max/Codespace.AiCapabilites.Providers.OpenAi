namespace AiCapabilities.Providers.OpenAi.Guides.StructuredOutputs.Models;

public sealed record ActionSteps
{
    public string Action { get; set; } = default!;
    
    public int Order { get; init; }
    
    public bool IsOptional { get; set; }
}