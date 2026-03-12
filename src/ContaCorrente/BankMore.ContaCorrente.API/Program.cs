using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using BankMore.ContaCorrente.Application.Services.Interfaces;
using BankMore.ContaCorrente.Domain.Interfaces;
using BankMore.ContaCorrente.Infrastructure.Data;
using BankMore.ContaCorrente.Infrastructure.Repositories;
using BankMore.ContaCorrente.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(
        typeof(BankMore.ContaCorrente.Application
               .Commands.CadastrarContaCorrente
               .CadastrarContaCorrenteHandler).Assembly));

// Banco de dados
var connectionString = builder.Configuration
    .GetConnectionString("Default")!;

builder.Services.AddSingleton(
    new DbConnectionFactory(connectionString));

builder.Services.AddSingleton<DatabaseInitializer>();

// Repositórios
builder.Services.AddScoped<IContaCorrenteRepository,
                            ContaCorrenteRepository>();
builder.Services.AddScoped<IMovimentoRepository,
                            MovimentoRepository>();

// JWT Service 
var secretKey = builder.Configuration["Jwt:SecretKey"]!;
var expiracaoMinutos = builder.Configuration
    .GetValue<int>("Jwt:ExpiracaoMinutos");

builder.Services.AddSingleton<IJwtService>(
    new JwtService(secretKey, expiracaoMinutos));

// Autenticação JWT 
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "BankMore",
            ValidAudience = "BankMore",
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(secretKey))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();

// Swagger 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Inicializar banco 
app.Services.GetRequiredService<DatabaseInitializer>().Initialize();

app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();