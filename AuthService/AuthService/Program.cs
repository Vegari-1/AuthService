using AuthService.Middlewares;
using AuthService.Repository;
using AuthService.Repository.Interface;
using AuthService.Service;
using AuthService.Service.Interface;
using Microsoft.EntityFrameworkCore;
using OpenTracing;
using Jaeger.Reporters;
using Jaeger;
using Jaeger.Senders.Thrift;
using Jaeger.Samplers;
using OpenTracing.Contrib.NetCore.Configuration;
using OpenTracing.Util;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

// DB_HOST from Docker-Compose or Local if null
var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
if (dbHost == null)
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(
            builder.Configuration.GetConnectionString("DislinktDbConnection"),
            x => x.MigrationsHistoryTable("__MigrationsHistory", "auth")));
else
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(dbHost, x => x.MigrationsHistoryTable("__MigrationsHistory", "auth")));
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IProfileService, ProfileService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddOpenTracing();

builder.Services.AddSingleton<ITracer>(sp =>
{
    var serviceName = sp.GetRequiredService<IWebHostEnvironment>().ApplicationName;
    var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
    var reporter = new RemoteReporter.Builder()
                    .WithLoggerFactory(loggerFactory)
                    .WithSender(new UdpSender("host.docker.internal", 6831, 0))
                    .Build();
    var tracer = new Tracer.Builder(serviceName)
        // The constant sampler reports every span.
        .WithSampler(new ConstSampler(true))
        // LoggingReporter prints every reported span to the logging framework.
        .WithLoggerFactory(loggerFactory)
        .WithReporter(reporter)
        .Build();

    GlobalTracer.Register(tracer);

    return tracer;
});

builder.Services.Configure<HttpHandlerDiagnosticOptions>(options =>
        options.OperationNameResolver =
            request => $"{request.Method.Method}: {request?.RequestUri?.AbsoluteUri}");

var app = builder.Build();

// Run all migrations only on Docker container
if (dbHost != null)
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;

        var context = services.GetRequiredService<AppDbContext>();
        context.Database.Migrate();
    }


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.UseMiddleware<ExceptionHandlerMiddleware>();

// Prometheus metrics
app.UseMetricServer();

app.Run();

namespace AuthService
{
    public partial class Program { }
}
