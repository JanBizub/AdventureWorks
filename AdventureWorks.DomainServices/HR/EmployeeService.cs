namespace AdventureWorks.DomainServices.HR;

using Data.OLTP;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class EmployeeSalaryInfo
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string JobTitle { get; set; }
    public decimal SalaryMonthly { get; set; }
}

public class EmployeeService
{
    private readonly AdventureWorksOLTPContext _context;
    private readonly ILogger<EmployeeService> _logger;

    public EmployeeService(AdventureWorksOLTPContext context, ILogger<EmployeeService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<EmployeeSalaryInfo>> GetEmployeeSalaries()
    {
        var employeeSalaries = await _context.Employees
            .Join(_context.People,
                e => e.BusinessEntityId,
                p => p.BusinessEntityId,
                (e, p) => new { e, p })
            .Join(_context.EmployeePayHistories,
                ep => ep.e.BusinessEntityId,
                eph => eph.BusinessEntityId,
                (ep, eph) => new { ep.e, ep.p, eph })
            .Where(x => x.eph.RateChangeDate == _context.EmployeePayHistories
                .Where(eph2 => eph2.BusinessEntityId == x.e.BusinessEntityId)
                .Max(eph2 => eph2.RateChangeDate))
            .Select(x => new EmployeeSalaryInfo
            {
                FirstName = x.p.FirstName,
                LastName = x.p.LastName,
                JobTitle = x.e.JobTitle,
                SalaryMonthly = x.eph.Rate * 8 * 20
            })
            .AsNoTracking()
            .ToListAsync();

        return employeeSalaries;
    }
}