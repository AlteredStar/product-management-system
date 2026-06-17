using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Data;
using CommerceAPI.Models;

public class CategoryRevenue
{
    public required string CategoryName { get; set; }
    public decimal TotalRevenue { get; set; }
}

public class DateRevenue
{
    public DateTime OrderDate { get; set; }
    public decimal TotalRevenue { get; set; }
}

[ApiController]
[Route("api/[controller]")]
public class OrderItemsController : ControllerBase
{
    private readonly NpgsqlDataSource _dataSource;
    
    public OrderItemsController(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    //GET api/orderitems
    [HttpGet]
    public async Task<IActionResult> GetOrderItems()
    {
        using var connection = await _dataSource.OpenConnectionAsync();
        using var command = new NpgsqlCommand("SELECT * FROM sales.order_items", connection);
        using var reader = await command.ExecuteReaderAsync();
        
        var orderItems = new List<OrderItem>();
        while (await reader.ReadAsync())
        {
            orderItems.Add(new OrderItem
            {
                OrderItemsId = reader.GetInt32("order_items_id"),
                OrderId = reader.GetInt32("order_id"),
                ProductId = reader.GetInt32("product_id"),
                Quantity = reader.GetInt32("quantity")
            });
        }

        return Ok(orderItems);
    }

    //GET api/orderitems/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderItem(int id)
    {
        using var connection = await _dataSource.OpenConnectionAsync();
        using var command = new NpgsqlCommand("SELECT * FROM sales.order_items WHERE order_items_id = @id", connection);
        command.Parameters.AddWithValue("@id", id);
        using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return NotFound();
        }

        var orderItem = new OrderItem
        {
            OrderItemsId = reader.GetInt32("order_items_id"),
            OrderId = reader.GetInt32("order_id"),
            ProductId = reader.GetInt32("product_id"),
            Quantity = reader.GetInt32("quantity")
        };

        return Ok(orderItem);
    }

    //GET api/orderitems/category
    [HttpGet("category")]
    public async Task<IActionResult> GetOrderItemsByCategory()
    {
      using var connection = await _dataSource.OpenConnectionAsync();
      using var command = new NpgsqlCommand("""
        SELECT
          inventory.categories.name as category_name,
          SUM(sales.order_items.quantity * inventory.products.price) as total_revenue
        FROM sales.order_items
        JOIN inventory.products ON sales.order_items.product_id = inventory.products.product_id
        JOIN inventory.categories ON inventory.products.category_id = inventory.categories.category_id
        GROUP BY inventory.categories.name
        ORDER BY total_revenue DESC
        """, connection);
      using var reader = await command.ExecuteReaderAsync();

      var categoryRevenue = new List<CategoryRevenue>();
      while (await reader.ReadAsync())
      {
        categoryRevenue.Add(new CategoryRevenue
        {
          CategoryName = reader.GetString("category_name"),
          TotalRevenue = reader.GetDecimal("total_revenue")
        });
      }

      return Ok(categoryRevenue);
    }

    //GET api/orderitems/date
    [HttpGet("date")]
    public async Task<IActionResult> GetOrderItemsByDate()
    {
      using var connection = await _dataSource.OpenConnectionAsync();
      using var command = new NpgsqlCommand("""
        SELECT 
          sales.orders.order_date,
          SUM(sales.order_items.quantity * inventory.products.price) as total_revenue
        FROM sales.order_items
        JOIN inventory.products ON sales.order_items.product_id = inventory.products.product_id
        JOIN sales.orders ON sales.order_items.order_id = sales.orders.order_id
        GROUP BY sales.orders.order_date
        ORDER BY sales.orders.order_date DESC
        """, connection);
      using var reader = await command.ExecuteReaderAsync();

      var dateRevenue = new List<DateRevenue>();
      while (await reader.ReadAsync())
      {
        dateRevenue.Add(new DateRevenue
        {
          OrderDate = reader.GetDateTime("order_date"),
          TotalRevenue = reader.GetDecimal("total_revenue")
        });
      }

      return Ok(dateRevenue);
    }
}