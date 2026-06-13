using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Data;
using CommerceAPI.Models;

[ApiController]
[Route("api/[controller]")]
public class OrderItemsController : ControllerBase
{
    private readonly NpgsqlDataSource _dataSource;
    
    public OrderItemsController(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    //GET
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
                OrderItemsId = reader.GetInt32("id"),
                OrderId = reader.GetInt32("order_id"),
                ProductId = reader.GetInt32("product_id"),
                Quantity = reader.GetInt32("quantity")
            });
        }

        return Ok(orderItems);
    }

    //GET order item by ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderItem(int id)
    {
        using var connection = await _dataSource.OpenConnectionAsync();
        using var command = new NpgsqlCommand("SELECT * FROM sales.order_items WHERE id = @id", connection);
        command.Parameters.AddWithValue("@id", id);
        using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return NotFound();
        }

        var orderItem = new OrderItem
        {
            OrderItemsId = reader.GetInt32("id"),
            OrderId = reader.GetInt32("order_id"),
            ProductId = reader.GetInt32("product_id"),
            Quantity = reader.GetInt32("quantity")
        };

        return Ok(orderItem);
    }
}