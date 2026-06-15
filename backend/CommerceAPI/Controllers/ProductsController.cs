using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Microsoft.EntityFrameworkCore;
using System.Data;
using CommerceAPI.Models;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly NpgsqlDataSource _dataSource;
    private readonly CommerceDbContext _context;
    
    public ProductsController(NpgsqlDataSource dataSource, CommerceDbContext context)
    {
        _dataSource = dataSource;
        _context = context;
    }

    // GET api/products?page={page_number}&pageSize={page_size}&search={product_name}
    [HttpGet]
    public async Task<IActionResult> GetProducts([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null)
    {
      int offset = (page - 1) * pageSize;
      var rows = new List<Product>();
      int totalCount = 0;

      string countQuery = "SELECT COUNT(*) FROM inventory.products";
      string selectQuery = "SELECT * FROM inventory.products";
      string whereClause = string.Empty;

      if (!string.IsNullOrWhiteSpace(search))
      {
          whereClause = " WHERE name ILIKE @search";
      }

      await using (var countCmd = _dataSource.CreateCommand(countQuery + whereClause))
      {
          if (!string.IsNullOrWhiteSpace(search))
          {
              countCmd.Parameters.AddWithValue("search", $"%{search}%");
          }
          totalCount = Convert.ToInt32(await countCmd.ExecuteScalarAsync());
      }

      string finalSelect = $"{selectQuery}{whereClause} ORDER BY product_id LIMIT @limit OFFSET @offset";

      await using (var selectCmd = _dataSource.CreateCommand(finalSelect))
      {
          if (!string.IsNullOrWhiteSpace(search))
          {
              selectCmd.Parameters.AddWithValue("search", $"%{search}%");
          }
          selectCmd.Parameters.AddWithValue("limit", pageSize);
          selectCmd.Parameters.AddWithValue("offset", offset);

          await using (var reader = await selectCmd.ExecuteReaderAsync())
          {
              while (await reader.ReadAsync())
              {
                  rows.Add(new Product
                  {
                      ProductId = reader.GetInt32(0),
                      CategoryId = reader.GetInt32(1),
                      Name = reader.GetString(2),
                      Description = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                      Price = reader.GetDecimal(4),
                      Stock = reader.GetInt32(5)
                  });
              }
          }
      }

      int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

      return Ok(new { rows, totalPages });
    }

    //GET api/products/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(int id)
    {
        using var connection = await _dataSource.OpenConnectionAsync();
        using var command = new NpgsqlCommand("SELECT * FROM inventory.products WHERE product_id = @id", connection);
        command.Parameters.AddWithValue("@id", id);
        using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return NotFound();
        }

        var product = new Product
        {
            ProductId = reader.GetInt32(reader.GetOrdinal("product_id")),
            Name = reader.GetString("name"),
            Description = reader.GetString("description"),
            Price = reader.GetDecimal("price")
        };

        return Ok(product);
    }

    //POST api/products
    [HttpPost]
    public async Task<IActionResult> CreateProduct(Product product)
    {
        using var connection = await _dataSource.OpenConnectionAsync();
        using var command = new NpgsqlCommand("INSERT INTO inventory.products (category_id, name, description, price, stock) VALUES (@category_id, @name, @description, @price, @stock)", connection);
        command.Parameters.AddWithValue("@category_id", product.CategoryId);
        command.Parameters.AddWithValue("@name", product.Name);
        command.Parameters.AddWithValue("@description", product.Description ?? string.Empty);
        command.Parameters.AddWithValue("@price", product.Price);
        command.Parameters.AddWithValue("@stock", product.Stock);
        await command.ExecuteNonQueryAsync();
        return Ok();
    }

    //PUT api/products/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, Product product)
    {
        using var connection = await _dataSource.OpenConnectionAsync();
        using var command = new NpgsqlCommand("UPDATE inventory.products SET name = @name, description = @description, price = @price, stock = @stock WHERE product_id = @id", connection);
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@name", product.Name);
        command.Parameters.AddWithValue("@description", product.Description ?? string.Empty);
        command.Parameters.AddWithValue("@price", product.Price);
        command.Parameters.AddWithValue("@stock", product.Stock);
        await command.ExecuteNonQueryAsync();
        return Ok();
    }

    //DELETE api/products/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        using var connection = await _dataSource.OpenConnectionAsync();
        using var command = new NpgsqlCommand("DELETE FROM inventory.products WHERE product_id = @id", connection);
        command.Parameters.AddWithValue("@id", id);
        await command.ExecuteNonQueryAsync();
        return Ok();
    }
}