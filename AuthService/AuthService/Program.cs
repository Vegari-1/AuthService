﻿using AuthService.Middlewares;
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

var allowSpecificOrigins = "_allowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(allowSpecificOrigins,
                          policy =>
                          {
                              policy.WithOrigins("http://localhost:3000")
                                    .AllowAnyHeader()
                                    .AllowAnyMethod();
                          });
});

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DislinktDbConnection")));
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

//repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();

//services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(allowSpecificOrigins);

app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<ExceptionHandlerMiddleware>();

// Prometheus metrics
app.UseMetricServer();

app.Run();

namespace AuthService
{
    public partial class Program { }
}
