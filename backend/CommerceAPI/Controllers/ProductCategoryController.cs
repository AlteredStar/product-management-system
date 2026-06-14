using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Data;
using CommerceAPI.Models;

[ApiController]
[Route("api/[controller]")]
public class ProductCategoryController : ControllerBase
{
    private readonly NpgsqlDataSource _dataSource;

    public ProductCategoryController(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    //GET api/product-categories
    [HttpGet]
    public async Task<IActionResult> GetAllProductCategories()
    {
        using var connection = await _dataSource.OpenConnectionAsync();
        using var command = new NpgsqlCommand("SELECT * FROM inventory.categories", connection);
        using var reader = await command.ExecuteReaderAsync();

        var productCategories = new List<ProductCategory>();
        while (await reader.ReadAsync())
        {
            productCategories.Add(new ProductCategory
            {
                CategoryId = reader.GetInt32("category_id"),
                Name = reader.GetString("name"),
                Description = reader.GetString("description")
            });
        }

        return Ok(productCategories);
    }

    //GET api/product-categories/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductCategory(int id)
    {
        using var connection = await _dataSource.OpenConnectionAsync();
        using var command = new NpgsqlCommand("SELECT * FROM sales.product_categories WHERE id = @id", connection);
        command.Parameters.AddWithValue("@id", id);
        using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return NotFound();
        }

        var productCategory = new ProductCategory
        {
            CategoryId = reader.GetInt32("id"),
            Name = reader.GetString("category_name"),
            Description = reader.GetString("description")
        };

        return Ok(productCategory);
    }

    //GET api/product-categories/name/{name}
    [HttpGet("name/{name}")]
    public async Task<IActionResult> GetProductCategoryByName(string name)
    {
        using var connection = await _dataSource.OpenConnectionAsync();
        using var command = new NpgsqlCommand("SELECT * FROM sales.product_categories WHERE category_name = @name", connection);
        command.Parameters.AddWithValue("@name", name);
        using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return NotFound();
        }

        var productCategory = new ProductCategory
        {
            CategoryId = reader.GetInt32("id"),
            Name = reader.GetString("category_name"),
            Description = reader.GetString("description")
        };

        return Ok(productCategory);
    }
}