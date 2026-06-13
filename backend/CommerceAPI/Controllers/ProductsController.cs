using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Data;
using CommerceAPI.Models;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly NpgsqlDataSource _dataSource;
    
    public ProductsController(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    //GET
    [HttpGet]
    public async Task<IActionResult> GetProducts()
    {
        using var connection = await _dataSource.OpenConnectionAsync();
        using var command = new NpgsqlCommand("SELECT * FROM inventory.products", connection);
        using var reader = await command.ExecuteReaderAsync();
        
        var products = new List<Product>();
        while (await reader.ReadAsync())
        {
            products.Add(new Product
            {
                ProductId = reader.GetInt32("id"),
                Name = reader.GetString("name"),
                Description = reader.GetString("description"),
                Price = reader.GetDecimal("price")
            });
        }

        return Ok(products);
    }

    //GET product by ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(int id)
    {
        using var connection = await _dataSource.OpenConnectionAsync();
        using var command = new NpgsqlCommand("SELECT * FROM inventory.products WHERE id = @id", connection);
        command.Parameters.AddWithValue("@id", id);
        using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return NotFound();
        }

        var product = new Product
        {
            ProductId = reader.GetInt32("id"),
            Name = reader.GetString("name"),
            Description = reader.GetString("description"),
            Price = reader.GetDecimal("price")
        };

        return Ok(product);
    }
}