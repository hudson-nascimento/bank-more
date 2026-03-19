using BankMore.Transferencia.API.Middlewares;
using BankMore.Transferencia.Application.Services.Interfaces;
using BankMore.Transferencia.Domain.Interfaces;
using BankMore.Transferencia.Infrastructure.Data;
using BankMore.Transferencia.Infrastructure.Repositories;
using BankMore.Transferencia.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SQLitePCL;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(
        typeof(BankMore.Transferencia.Application
               .Commands.EfetuarTransferencia
               .EfetuarTransferenciaHandler).Assembly));

// Banco de dados
Batteries_V2.Init();

builder.Services.AddSingleton<DatabaseInitializer>();

var connectionString = builder.Configuration
    .GetConnectionString("Default")!;

builder.Services.AddSingleton(
    new DbConnectionFactory(connectionString));

builder.Services.AddSingleton<DatabaseInitializer>();

// Repositórios
builder.Services.AddScoped<ITransferenciaRepository,
                            TransferenciaRepository>();
// Domain / Application services
// Register implementation for IContaCorrenteService required by EfetuarTransferenciaHandler
builder.Services.AddScoped<IContaCorrenteService, ContaCorrenteService>();

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
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(secretKey))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();

// Swagger 
builder.Services.AddEndpointsApiExplorer();

// Criar extensaoo para configurar o Swagger, deixando o Program.cs mais limpo
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "BankMore - Transferencia API",
        Version = "v1",
        Description = "API de gerenciamento de transferência entre contas correntes do BankMore"
    });

    // Habilitar comentários XML(documentação dos endpoints)
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);

    // Adicionar suporte a JWT no Swagger UI
    // Suporte a autenticação JWT no Swagger
    const string scheme = "Bearer";

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "JWT Authorization header utilizando Bearer scheme.",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = scheme
        }
    };

    options.AddSecurityDefinition(scheme, securityScheme);

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, Array.Empty<string>() }
    });
});

var app = builder.Build();

// Inicializar banco 
app.Services.GetRequiredService<DatabaseInitializer>().Initialize();

// Middleware de erros — deve ser o PRIMEIRO da pipeline
app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();