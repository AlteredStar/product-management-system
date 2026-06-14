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

    //GET api/payments/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPayment(int id)
    {
        using var connection = await _dataSource.OpenConnectionAsync();
        using var command = new NpgsqlCommand("SELECT * FROM sales.payments WHERE payment_id = @id", connection);
        command.Parameters.AddWithValue("@id", id);
        using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return NotFound();
        }

        var payment = new Payment
        {
            PaymentId = reader.GetInt32("payment_id"),
            OrderId = reader.GetInt32("order_id"),
            AmountPaid = reader.GetDecimal("amount_paid"),
            PaymentDate = reader.GetDateTime("payment_date"),
            Status = reader.GetString("status")
        };

        return Ok(payment);
    }

    //GET api/payments/order/{orderId}
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
            PaymentId = reader.GetInt32("payment_id"),
            OrderId = reader.GetInt32("order_id"),
            AmountPaid = reader.GetDecimal("amount_paid"),
            PaymentDate = reader.GetDateTime("payment_date"),
            Status = reader.GetString("status")
        };

        return Ok(payment);
    }

    //POST api/payments
    [HttpPost]
    public async Task<IActionResult> CreatePayment([FromBody] Payment payment)
    {
        using var connection = await _dataSource.OpenConnectionAsync();
        using var command = new NpgsqlCommand("INSERT INTO sales.payments (order_id, amount_paid, payment_date, status) VALUES (@orderId, @amountPaid, @paymentDate, @status) RETURNING payment_id", connection);
        command.Parameters.AddWithValue("@orderId", payment.OrderId);
        command.Parameters.AddWithValue("@amountPaid", payment.AmountPaid);
        command.Parameters.AddWithValue("@paymentDate", payment.PaymentDate ?? DateTime.Today);
        command.Parameters.AddWithValue("@status", payment.Status ?? "Pending");
        var id = await command.ExecuteScalarAsync();
        return CreatedAtAction(nameof(GetPayment), new { id }, payment);
    }
}