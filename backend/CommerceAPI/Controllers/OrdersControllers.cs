using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Data;
using CommerceAPI.Models;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
  private readonly NpgsqlDataSource _dataSource;
  
  public OrdersController(NpgsqlDataSource dataSource)
  {
    _dataSource = dataSource;
  }

  //GET api/orders
  [HttpGet]
  public async Task<IActionResult> GetOrders()
  {
    using var connection = await _dataSource.OpenConnectionAsync();
    using var command = new NpgsqlCommand("SELECT * FROM sales.orders", connection);
    using var reader = await command.ExecuteReaderAsync();
    
    var orders = new List<Order>();
    while (await reader.ReadAsync())
    {
        orders.Add(new Order
        {
            OrderId = reader.GetInt32("order_id"),
            UserId = reader.GetInt32("customer_id"),
            OrderDate = reader.GetDateTime("order_date"),
            Status = reader.GetString("status")
        });
    }

    return Ok(orders);
  }

  //GET api/orders/{id}
  [HttpGet("{id}")]
  public async Task<IActionResult> GetOrder(int id)
  {
    using var connection = await _dataSource.OpenConnectionAsync();
    using var command = new NpgsqlCommand("SELECT * FROM sales.orders WHERE order_id = @id", connection);
    command.Parameters.AddWithValue("@id", id);
    using var reader = await command.ExecuteReaderAsync();

    if (!await reader.ReadAsync())
    {
        return NotFound();
    }

    var order = new Order
    {
        OrderId = reader.GetInt32("order_id"),
        UserId = reader.GetInt32("customer_id"),
        OrderDate = reader.GetDateTime("order_date"),
        Status = reader.GetString("status")
    };

    return Ok(order);
  }

  //POST api/orders
  [HttpPost]
  public async Task<IActionResult> CreateOrder([FromBody] Order order)
  {
    using var connection = await _dataSource.OpenConnectionAsync();
    using var command = new NpgsqlCommand("INSERT INTO sales.orders (customer_id, order_date, status) VALUES (@customer_id, @order_date, @status) RETURNING order_id", connection);
    command.Parameters.AddWithValue("@customer_id", order.UserId);
    command.Parameters.AddWithValue("@order_date", order.OrderDate ?? DateTime.Today);
    command.Parameters.AddWithValue("@status", order.Status ?? "Pending");
    using var reader = await command.ExecuteReaderAsync();

    if (!await reader.ReadAsync())
    {
        return BadRequest();
    }

    var orderId = reader.GetInt32("id");
    return CreatedAtAction(nameof(GetOrder), new { id = orderId }, order);
  }

  //DELETE api/orders/{id}
  [HttpDelete("{id}")]
  public async Task<IActionResult> DeleteOrder(int id)
  {
    using var connection = await _dataSource.OpenConnectionAsync();
    using var command = new NpgsqlCommand("DELETE FROM sales.orders WHERE order_id = @id", connection);
    command.Parameters.AddWithValue("@id", id);
    await command.ExecuteNonQueryAsync();
    return NoContent();
  }
}