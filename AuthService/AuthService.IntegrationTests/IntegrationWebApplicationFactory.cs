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
        public readonly TestcontainerDatabase container;

        public IntegrationWebApplicationFactory()
        {
            container = new TestcontainersBuilder<PostgreSqlTestcontainer>()
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
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.RemoveDbContext<TDbContext>();
                services.AddDbContext<TDbContext>(options => { options.UseNpgsql(container.ConnectionString); });
                services.EnsureDbCreated<TDbContext>();
            });
        }

        public async Task InitializeAsync() => await container.StartAsync();

        public new async Task DisposeAsync() => await container.DisposeAsync();

    }
}
