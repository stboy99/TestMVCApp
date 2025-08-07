using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestMVCApp.Models;
using System;

public class PersonRepository
{
    private readonly IConfiguration _configuration;

    public PersonRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private IDbConnection CreateConnection()
    {
        return new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
    }

    public async Task<IEnumerable<Person>> GetAllAsync()
    {
        try
        {
            using var connection = CreateConnection();
            return await connection.QueryAsync<Person>(
                "sp_AllPerson",
                commandType: CommandType.StoredProcedure
            );
        }
        catch (SqlException ex)
        {
            // log or rethrow as needed
            throw new Exception("An error occurred while retrieving people from the database.", ex);
        }
    }

    public async Task<int> AddAsync(Person person)
    {
        try
        {
            using var connection = CreateConnection();
            var parameters = new { person.Name, person.Age, person.Email };

            return await connection.ExecuteAsync(
                "sp_AddPerson",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }
        catch (SqlException ex)
        {
            // You can inspect ex.Number to check for specific SQL Server error codes
            throw new Exception("Failed to insert person into the database.", ex);
        }
    }

    public async Task<int> EditAsync(Person person)
    {
        var existedPerson = await GetByIdAsync(person.Id);

        if (existedPerson == null)
        {
            throw new KeyNotFoundException($"Person with Id {person.Id} was not found.");
        }

        try
        {
            using var connection = CreateConnection();
            var parameters = new { person.Id, person.Name, person.Age, person.Email };

            return await connection.ExecuteAsync(
                "sp_EditPerson",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }
        catch (SqlException ex)
        {
            // You can inspect ex.Number to check for specific SQL Server error codes
            throw new Exception("Failed to update person into the database.", ex);
        }
    }

    public async Task<Person> GetByIdAsync(int id)
    {
        using var connection = CreateConnection();

        var person = await connection.QueryFirstOrDefaultAsync<Person>(
            "sp_GetPersonById",
            new { Id = id },
            commandType: CommandType.StoredProcedure
        );

        return person;
    }

    public async Task DeleteAsync(int id)
    {
        try
        {
            using var connection = CreateConnection();

            int rowsAffected = await connection.ExecuteAsync(
                "sp_DeletePerson",
                new { Id = id },
                commandType: CommandType.StoredProcedure
            );

            if (rowsAffected == 0)
            {
                throw new KeyNotFoundException($"Person with Id {id} was not found or already deleted.");
            }
        }
        catch (SqlException ex)
        {
            throw new Exception("Failed to delete person from the database.", ex);
        }
    }


}
