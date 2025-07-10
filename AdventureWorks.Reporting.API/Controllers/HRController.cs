using AdventureWorks.Data.OLTP;
using AdventureWorks.DomainServices.HR;
using Microsoft.AspNetCore.Mvc;

namespace AdventureWorks.Reporting.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HRController : ControllerBase
{
    private readonly ILogger<HRController> _logger;
    private readonly AdventureWorksOLTPContext _context;
    private readonly EmployeeService  _employeeService;
    
    public HRController(ILogger<HRController> logger, AdventureWorksOLTPContext context, EmployeeService employeeService)
    {
        _logger = logger;
        _context = context;
        _employeeService = employeeService;
    }

    [HttpGet("echo")]
    public string Echo () => "HRController::Echo";
    
    [HttpGet("employee-salaries")]
    public async Task<IEnumerable<EmployeeSalaryInfo>> Get() => 
        await _employeeService.GetEmployeeSalaries();
}