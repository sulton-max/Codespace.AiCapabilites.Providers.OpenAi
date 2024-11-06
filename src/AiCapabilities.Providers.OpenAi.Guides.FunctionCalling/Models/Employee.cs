namespace AiCapabilities.Providers.OpenAi.Guides.FunctionCalling.Models;

public class Employee
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = default!;

    public string LastName { get; set; } = default!;
    
    public Guid? ManagerId { get; set; }
}