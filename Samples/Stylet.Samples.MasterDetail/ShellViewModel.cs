using Stylet.Avalonia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stylet.Samples.MasterDetail;
public class ShellViewModel : Screen
{
    public IObservableCollection<EmployeeModel> Employees { get; private set; }

    private EmployeeModel _selectedEmployee;
    public EmployeeModel SelectedEmployee
    {
        get { return this._selectedEmployee; }
        set { SetAndNotify(ref this._selectedEmployee, value); }
    }

    public ShellViewModel()
    {
        this.DisplayName = "Master-Detail";

        this.Employees = new BindableCollection<EmployeeModel>();

        this.Employees.Add(new EmployeeModel() { Name = "Fred" });
        this.Employees.Add(new EmployeeModel() { Name = "Bob" });

        this.SelectedEmployee = this.Employees.FirstOrDefault();
    }

    public void AddEmployee()
    {
        this.Employees.Add(new EmployeeModel() { Name = "Unnamed" });
    }

    public void RemoveEmployee(EmployeeModel item)
    {
        this.Employees.Remove(item);
    }
}
public class EmployeeModel : PropertyChangedBase
{
    private string _name;
    public string Name
    {
        get { return this._name; }
        set { this.SetAndNotify(ref this._name, value); }
    }
}