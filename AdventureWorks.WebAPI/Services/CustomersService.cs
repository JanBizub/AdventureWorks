using DependencyInjectionApi.Data;
using DependencyInjectionApi.DTO;
using Microsoft.EntityFrameworkCore;

namespace AdventureWorks.Services;

public class CustomersService
{
    private readonly AdventureWorksDw2022Context _dbContext;
    
    private readonly ILogger _logger;

    public CustomersService(AdventureWorksDw2022Context dbContext, ILogger<CustomersService> logger)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<object> GetCustomersAsync(int pageNumber, int customersPerPage, int maxRecordsRetrieved)
    {
        if (pageNumber <= 0)
            throw new InvalidOperationException("PageNumber must be non-negative");
        if (customersPerPage > maxRecordsRetrieved)
            throw new InvalidOperationException($"There is a limit of max {maxRecordsRetrieved} customers per page.");

        var totalRecords = await _dbContext.DimCustomers.CountAsync();

        var skip = (pageNumber - 1) * customersPerPage;

        var customers = await _dbContext.DimCustomers
            .OrderBy(c => c.LastName)
            .Select(c => new
            {
                c.FirstName,
                c.LastName,
                c.Gender
            })
            .Skip(skip)
            .Take(customersPerPage)
            .ToListAsync();

        return new
        {
            TotalRecords = totalRecords,
            PageNumber = pageNumber,
            CustomersPerPage = customersPerPage,
            Customers = customers
        };
    }

    public async Task<object?> GetCustomerAsync(int id)
    {
        return await _dbContext.DimCustomers
            .Where(c => c.CustomerKey == id)
            .Select(c => new
            {
                c.FirstName,
                c.LastName,
                c.Gender
            })
            .FirstOrDefaultAsync();
    }

    // todo: Use DTO
    public async Task<bool> EditCustomerAsync(int id, DimCustomer updatedCustomer)
    {
        var customer = await _dbContext.DimCustomers.FirstOrDefaultAsync(c => c.CustomerKey == id);

        if (customer == null)
        {
            return false;
        }

        customer.FirstName = updatedCustomer.FirstName;
        customer.LastName = updatedCustomer.LastName;
        customer.Gender = updatedCustomer.Gender;

        await _dbContext.SaveChangesAsync();

        return true;
    }
    
    public async Task<bool> CreateCustomerAsync(NewCustomerDto newCustomer)
    {
        var customerMailExists = await _dbContext.DimCustomers.AnyAsync(c => c.EmailAddress == newCustomer.EmailAddress);
        
        if (customerMailExists) throw new InvalidOperationException("Customer already exists.");

        var dbCustomer = new DimCustomer();
        dbCustomer.EmailAddress = newCustomer.EmailAddress;
        
        await _dbContext.DimCustomers.AddAsync(dbCustomer);
        await _dbContext.SaveChangesAsync();
        
        // todo: on error do nothing. If logger service does not work, do not interrupt the createion process
        _logger.LogInformation("Created new customer with email address {@newCustomer}", newCustomer);
        
        return true;
    }
    
    public async Task<bool> DeleteCustomerAsync(int id)
    {
        var customer = await _dbContext.DimCustomers.FirstOrDefaultAsync(c => c.CustomerKey == id);

        if (customer == null)
        {
            return false;
        }

        _dbContext.DimCustomers.Remove(customer);
        await _dbContext.SaveChangesAsync();

        return true;
    }
}
