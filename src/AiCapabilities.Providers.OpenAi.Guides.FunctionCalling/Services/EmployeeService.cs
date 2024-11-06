using AiCapabilities.Providers.OpenAi.Guides.FunctionCalling.Models;

namespace AiCapabilities.Providers.OpenAi.Guides.FunctionCalling.Services;

public class EmployeeService
{
    private readonly List<Employee> _employees;

    public EmployeeService()
    {
        _employees =
        [
            new Employee { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe" },
            new Employee { Id = Guid.NewGuid(), FirstName = "Jane", LastName = "Smith" }
        ];

        _employees.Add(new Employee { Id = Guid.NewGuid(), FirstName = "Alice", LastName = "Johnson", ManagerId = _employees[0].Id });
        _employees.Add(new Employee { Id = Guid.NewGuid(), FirstName = "Bob", LastName = "Brown", ManagerId = _employees[0].Id });
        _employees.Add(new Employee { Id = Guid.NewGuid(), FirstName = "Charlie", LastName = "Davis", ManagerId = _employees[1].Id });
        _employees.Add(new Employee { Id = Guid.NewGuid(), FirstName = "Eve", LastName = "Wilson", ManagerId = _employees[1].Id });
    }


    public Employee? GetById(Guid employeeId)
    {
        return _employees.FirstOrDefault(e => e.Id == employeeId);
    }

    public Employee? GetByName(string firstName, string lastName)
    {
        return _employees.FirstOrDefault(e =>
            e.FirstName.Equals(firstName, StringComparison.OrdinalIgnoreCase)
            && e.LastName.Equals(lastName, StringComparison.OrdinalIgnoreCase));
    }
}