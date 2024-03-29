﻿using AuthService.Model;
using AuthService.Repository;
using Npgsql;
using System;

namespace AuthService.IntegrationTests
{
    public static class DbExtensions
    {

        public static long CountTableRows(this IntegrationWebApplicationFactory<Program, AppDbContext> factory,
            string schemaName, string tableName)
        {
            long totalRows = -1;
            using (var connection = new NpgsqlConnection(factory.postgresContainer.ConnectionString))
            {
                using (var command = new NpgsqlCommand())
                {
                    connection.Open();
                    command.Connection = connection;
                    command.CommandText = "SELECT COUNT(*) FROM " + schemaName + ".\"" + tableName + "\"";
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            totalRows = (long)reader[0];
                        }
                    }
                }
            }
            return totalRows;
        }

        public static void Insert(this IntegrationWebApplicationFactory<Program, AppDbContext> factory,
            string schemaName, string tableName, User user)
        {
            string insertQuery = "INSERT INTO " + schemaName + ".\"" + tableName + 
                                 "\" (\"Id\", \"Username\", \"Email\", \"Password\", \"Role\") " +
                                 "VALUES (@Id, @Username, @Email, @Password, @Role)";
            using (var connection = new NpgsqlConnection(factory.postgresContainer.ConnectionString))
            {
                using (var command = new NpgsqlCommand(insertQuery, connection))
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@Id", user.Id);
                    command.Parameters.AddWithValue("@Username", user.Username);
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.Parameters.AddWithValue("@Password", user.Password);
                    command.Parameters.AddWithValue("@Role", user.Role);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void DeleteById(this IntegrationWebApplicationFactory<Program, AppDbContext> factory,
            string schemaName, string tableName, Guid id)
        {
            using (var connection = new NpgsqlConnection(factory.postgresContainer.ConnectionString))
            {
                using (var command = new NpgsqlCommand())
                {
                    connection.Open();
                    command.Connection = connection;
                    command.CommandText = "DELETE FROM " + schemaName + ".\"" + tableName + "\" WHERE \"Id\" = @id";
                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
