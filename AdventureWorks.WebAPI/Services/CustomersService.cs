using DependencyInjectionApi.Data;
using Microsoft.EntityFrameworkCore;

namespace DependencyInjectionApi.Services;

public class CustomersService
{
    private readonly AdventureWorksDw2022Context _dbContext;

    public CustomersService(AdventureWorksDw2022Context dbContext)
    {
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
