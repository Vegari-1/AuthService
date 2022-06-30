using AuthService.Model;
using AuthService.Repository;
using Npgsql;
using System;

namespace AuthService.IntegrationTests
{
    public static class DbExtensions
    {

        public static long CountTableRows(this IntegrationWebApplicationFactory<Program, AppDbContext> factory,
            string tableName)
        {
            long totalRows = -1;
            using (var connection = new NpgsqlConnection(factory.container.ConnectionString))
            {
                using (var command = new NpgsqlCommand())
                {
                    connection.Open();
                    command.Connection = connection;
                    command.CommandText = "SELECT COUNT(*) FROM \"" + tableName + "\"";
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
            string tableName, User user)
        {
            string insertQuery = "INSERT INTO \"" + tableName + "\" (\"Id\", \"Username\", \"Email\", \"Password\", \"Name\", \"Surname\") " +
                                 "VALUES (@Id, @Username, @Email, @Password, @Name, @Surname)";
            using (var connection = new NpgsqlConnection(factory.container.ConnectionString))
            {
                using (var command = new NpgsqlCommand(insertQuery, connection))
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@Id", user.Id);
                    command.Parameters.AddWithValue("@Username", user.Username);
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.Parameters.AddWithValue("@Password", user.Password);
                    command.Parameters.AddWithValue("@Name", user.Name);
                    command.Parameters.AddWithValue("@Surname", user.Surname);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void DeleteById(this IntegrationWebApplicationFactory<Program, AppDbContext> factory,
            string tableName, Guid id)
        {
            using (var connection = new NpgsqlConnection(factory.container.ConnectionString))
            {
                using (var command = new NpgsqlCommand())
                {
                    connection.Open();
                    command.Connection = connection;
                    command.CommandText = "DELETE FROM \"" + tableName + "\" WHERE \"Id\" = @id";
                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
