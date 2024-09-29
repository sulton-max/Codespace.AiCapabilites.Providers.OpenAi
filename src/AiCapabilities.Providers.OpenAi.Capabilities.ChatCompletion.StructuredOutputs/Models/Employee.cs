namespace AiCapabilities.Providers.OpenAi.Capabilities.ChatCompletion.StructuredOutputs.Models;

public class Employee
{
    public string FirstName { get; set; } = default!;

    public string LastName { get; set; } = default!;
    
    public DateTimeOffset StartDate { get; set; }

    public EmployeeType Type { get; set; }
}