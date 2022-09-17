using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;

namespace AuthService.IntegrationTests
{
    public class IntegrationWebApplicationFactory<TProgram, TDbContext> : WebApplicationFactory<TProgram>, IAsyncLifetime
    where TProgram : class where TDbContext : DbContext
    {
        public readonly TestcontainerDatabase postgresContainer;
        public readonly TestcontainersContainer natsContainer;

        public IntegrationWebApplicationFactory()
        {
            postgresContainer = new TestcontainersBuilder<PostgreSqlTestcontainer>()
                .WithDatabase(new PostgreSqlTestcontainerConfiguration
                {
                    Database = "test_db",
                    Username = "postgres",
                    Password = "postgres",
                })
                .WithImage("postgres:11")
                .WithName("postgres")
                .WithCleanUp(true)
                .Build();

            natsContainer = new TestcontainersBuilder<TestcontainersContainer>()
            .WithImage("nats:2")
            .WithName("nats")
            .WithCleanUp(true)
            .WithPortBinding(4222)
            .WithPortBinding(8222)
            .Build();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.RemoveDbContext<TDbContext>();
                services.AddDbContext<TDbContext>(options => { options.UseNpgsql(postgresContainer.ConnectionString); });
                services.EnsureDbCreated<TDbContext>();
            });
        }

        public async Task InitializeAsync()
        {
            await postgresContainer.StartAsync();
            await natsContainer.StartAsync();
        }

        public new async Task DisposeAsync()
        {
            await postgresContainer.DisposeAsync();
            await natsContainer.DisposeAsync();
        }

    }
}
