using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestMVCApp.Models;
using System;
using Microsoft.Extensions.Logging;

public class UserRepository
{
    private readonly IConfiguration _config;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(IConfiguration config, ILogger<UserRepository> logger)
    {
        _config = config;
        _logger = logger;
    }

    private IDbConnection CreateConnection() =>
        new SqlConnection(_config.GetConnectionString("DefaultConnection"));

    public async Task<UserAccount?> GetByUsernameAsync(string username)
    {
        using var connection = CreateConnection();

        return await connection.QueryFirstOrDefaultAsync<UserAccount>(
            "sp_GetUserByUsername",
            new { Username = username },
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task<int> AddUserAsync(string username, string password)
    {
        var hash = BCrypt.Net.BCrypt.HashPassword(password);
        using var connection = CreateConnection();

        return await connection.ExecuteAsync(
            "sp_AddUser",
            new { Username = username, PasswordHash = hash },
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task RegisterAsync(string username, string password)
    {
        _logger.LogInformation("Registering user: {username}", username);
        using var connection = CreateConnection();
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

        try
        {
            await connection.ExecuteAsync(
                "sp_RegisterUser",
                new { Username = username, PasswordHash = passwordHash },
                commandType: CommandType.StoredProcedure
            );
        }
        catch (SqlException ex) when (ex.Number == 50000)
        {
            throw new ApplicationException(ex.Message);
        }
    }
}
