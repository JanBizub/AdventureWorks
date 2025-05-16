using AdventureWorks.Services;
using DependencyInjectionApi.Data;
using Microsoft.AspNetCore.Mvc;

namespace DependencyInjectionApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly CustomersService _customersService;
    private readonly ICalculationService _calculationService;
    private const int MaxRecordsRetrieved = 1000;

    public CustomersController(CustomersService customersService, ICalculationService calculationService)
    {
        _customersService = customersService;
        _calculationService = calculationService;
    }

    [HttpGet("echo")]
    public IActionResult Echo() => Ok("Echo");

    [HttpGet("calculate-discount")]
    public IActionResult CalculateDiscount(int originalPrice, int discountPercentage)
    {
        try
        {
            var discountedPrice = _calculationService.CalculateDiscount(originalPrice, discountPercentage);
            return Ok(new { OriginalPrice = originalPrice, DiscountPercentage = discountPercentage, DiscountedPrice = discountedPrice });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("")]
    public async Task<IActionResult> GetCustomers(int pageNumber = 1, int customersPerPage = 10)
    {
        try
        {
            var response = await _customersService.GetCustomersAsync(pageNumber, customersPerPage, MaxRecordsRetrieved);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCustomer(int id)
    {
        var customer = await _customersService.GetCustomerAsync(id);

        if (customer == null)
        {
            return NotFound();
        }

        return Ok(customer);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> EditCustomer(int id, [FromBody] DimCustomer? updatedCustomer)
    {
        if (updatedCustomer == null)
        {
            return BadRequest("Updated customer data is required.");
        }

        var result = await _customersService.EditCustomerAsync(id, updatedCustomer);

        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCustomer(int id)
    {
        var result = await _customersService.DeleteCustomerAsync(id);

        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
}
