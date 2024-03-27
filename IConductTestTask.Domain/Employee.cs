namespace IConductTestTask.Domain;

public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int? ManagerId { get; set; }
    public bool Enabled { get; set; }
    public List<Domain.Employee>? SupervisedEmployees { get; set; } // in case the employee is also a manager
}
