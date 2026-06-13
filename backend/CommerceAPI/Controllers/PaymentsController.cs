using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Data;
using CommerceAPI.Models;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly NpgsqlDataSource _dataSource;

    public PaymentsController(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    //GET
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPayment(int id)
    {
        using var connection = await _dataSource.OpenConnectionAsync();
        using var command = new NpgsqlCommand("SELECT * FROM sales.payments WHERE id = @id", connection);
        command.Parameters.AddWithValue("@id", id);
        using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return NotFound();
        }

        var payment = new Payment
        {
            PaymentId = reader.GetInt32("id"),
            OrderId = reader.GetInt32("order_id"),
            AmountPaid = reader.GetDecimal("amount"),
            PaymentDate = reader.GetDateTime("payment_date"),
            Status = reader.GetString("status")
        };

        return Ok(payment);
    }

    //GET by id
    [HttpGet("order/{orderId}")]
    public async Task<IActionResult> GetPaymentByOrderId(int orderId)
    {
        using var connection = await _dataSource.OpenConnectionAsync();
        using var command = new NpgsqlCommand("SELECT * FROM sales.payments WHERE order_id = @orderId", connection);
        command.Parameters.AddWithValue("@orderId", orderId);
        using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return NotFound();
        }

        var payment = new Payment
        {
            PaymentId = reader.GetInt32("id"),
            OrderId = reader.GetInt32("order_id"),
            AmountPaid = reader.GetDecimal("amount"),
            PaymentDate = reader.GetDateTime("payment_date"),
            Status = reader.GetString("status")
        };

        return Ok(payment);
    }

    //POST
    [HttpPost]
    public async Task<IActionResult> CreatePayment([FromBody] Payment payment)
    {
        using var connection = await _dataSource.OpenConnectionAsync();
        using var command = new NpgsqlCommand("INSERT INTO sales.payments (order_id, amount, payment_date, status) VALUES (@orderId, @amount, @paymentDate, @status) RETURNING id", connection);
        command.Parameters.AddWithValue("@orderId", payment.OrderId);
        command.Parameters.AddWithValue("@amount", payment.AmountPaid);
        command.Parameters.AddWithValue("@paymentDate", payment.PaymentDate ?? DateTime.Today);
        command.Parameters.AddWithValue("@status", payment.Status ?? "Pending");
        var id = await command.ExecuteScalarAsync();
        return CreatedAtAction(nameof(GetPayment), new { id }, payment);
    }
}