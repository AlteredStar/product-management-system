using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Data;
using CommerceAPI.Models;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly NpgsqlDataSource _dataSource;
    
    public UsersController(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    //GET api/users
    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        using var connection = await _dataSource.OpenConnectionAsync();
        using var command = new NpgsqlCommand("SELECT * FROM customers.users", connection);
        using var reader = await command.ExecuteReaderAsync();
        
        var users = new List<User>();
        while (await reader.ReadAsync())
        {
            users.Add(new User
            {
                UserId = reader.GetInt32("id"),
                Username = reader.GetString("username"),
                Email = reader.GetString("email")
            });
        }

        return Ok(users);
    }

    //GET api/users/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
        using var connection = await _dataSource.OpenConnectionAsync();
        using var command = new NpgsqlCommand("SELECT * FROM customers.users WHERE id = @id", connection);
        command.Parameters.AddWithValue("@id", id);
        using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return NotFound();
        }

        var user = new User
        {
            UserId = reader.GetInt32("id"),
            Username = reader.GetString("username"),
            Email = reader.GetString("email")
        };

        return Ok(user);
    }

    //POST api/users
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] User user)
    {
        using var connection = await _dataSource.OpenConnectionAsync();
        using var command = new NpgsqlCommand("INSERT INTO customers.users (name, email) VALUES (@name, @email) RETURNING id", connection);
        command.Parameters.AddWithValue("@name", user.Username);
        command.Parameters.AddWithValue("@email", user.Email);
        using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return BadRequest();
        }

        user.UserId = reader.GetInt32("id");
        return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, user);
    }

    //PUT api/users/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] User user)
    {
        using var connection = await _dataSource.OpenConnectionAsync();
        using var command = new NpgsqlCommand("UPDATE customers.users SET name = @name, email = @email WHERE id = @id", connection);
        command.Parameters.AddWithValue("@name", user.Username);
        command.Parameters.AddWithValue("@email", user.Email);
        command.Parameters.AddWithValue("@id", id);
        using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return NotFound();
        }

        return NoContent();
    }

    //DELETE api/users/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        using var connection = await _dataSource.OpenConnectionAsync();
        using var command = new NpgsqlCommand("DELETE FROM customers.users WHERE id = @id", connection);
        command.Parameters.AddWithValue("@id", id);
        using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return NotFound();
        }

        return NoContent();
    }
}