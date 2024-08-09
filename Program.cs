// See https://aka.ms/new-console-template for more information
using System.Text.Json;

string fileName = "correct-employees.json";

if (!File.Exists(fileName))
{
    Console.WriteLine($"File {fileName} not found");
    return;
}
var  dod = await File.ReadAllTextAsync(fileName);
if(string.IsNullOrEmpty(dod))
{
    Console.WriteLine("No data found");
    return;
}

List<Employee> employees = JsonSerializer.Deserialize<List<Employee>>(dod);
bool valid=ValidateEmployees(employees);

// If validation fails, return
if(!valid)
{
    return;
}

foreach(var emp in employees)
{
    Console.WriteLine($"Employee id: {emp.id}, name: {emp.name}, managerId: {emp.managerId}");
}


int indirectReportsCount = 0;
Employee f;

while(true){
    Console.Write("Enter employee name to search, or 'q' to quit program:");
    string search = Console.ReadLine();
    if(search == "q")
    {
        break;
    }
    f = employees.Where(e => e.name.Contains(search)).FirstOrDefault();
    if(f == null)
    {
        Console.WriteLine("Employee not found");
        continue;
    }
    PrintManager(f, employees);
    CountDirectReports(f, employees);
    CountIndirectReports(f, employees);
    Console.WriteLine($"- {f.name} has the following indirect reports: {indirectReportsCount}");

}

#region functions


void CountIndirectReports(Employee found, List<Employee> employees)
{
    if(found == null || employees ==null)
        return;
    var reports = employees.Where(e => e.managerId == found.id);
    if(reports.Count() == 0)
    {
        return;
    }
    foreach(var report in reports)
    {
        if(f!=found)
            indirectReportsCount++;
        CountIndirectReports(report, employees);
    }
}

void CountDirectReports(Employee found, List<Employee> employees)
{
    if(found == null || employees == null)
        return;
    var reports = employees.Where(e => e.managerId == found.id);
    if(reports.Count() == 0)
    {
        Console.WriteLine($"- {found.name} has no direct reports");
        return;
    }
    Console.WriteLine($"- {found.name} has {reports.Count()} direct reports:");
}

// Recursive function to find manager of an employee
void PrintManager(Employee found, List<Employee> employees)
{
    if(found == null || employees == null)
        return;
    var mgr = employees.Where(e => e.id == found.managerId).FirstOrDefault();
    if(mgr!=null)
        Console.WriteLine($"- {found.name}'s manager is " + mgr.name);
    PrintManager(mgr, employees);
}



// Function to validate employees collection
bool ValidateEmployees(List<Employee>? employees)
{
    if(employees == null)
    {
        Console.WriteLine("No data found");
        return false;
    }
    if(employees.Count(x=> x.managerId==null) > 1)
    {
        Console.WriteLine("More than one employee has no manager");
        return false;
    }
    var uniqueIds = employees.Select(x=>x.id).Distinct().Count();
    if( uniqueIds < employees.Count)
    {
        Console.WriteLine("Duplicate employee ids found");
        return false;
    }
    return true;
}

#endregion
