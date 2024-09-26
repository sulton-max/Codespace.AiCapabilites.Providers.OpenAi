namespace AiCapabilities.Providers.OpenAi.Capabilities.FunctionCalling.Models;

public class Employee
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = default!;

    public string LastName { get; set; } = default!;
    
    public Guid? ManagerId { get; set; }
}