using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using InvestmentTracker.API.Data;
using InvestmentTracker.API.Services;
using MediatR;
using System.Reflection;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Services.AddDbContext<ApplicationDbContext>(opt =>
    opt.UseMySql(
        builder.Configuration.GetConnectionString("Default"),
        new MySqlServerVersion(new Version(8, 0, 36))
    ));


builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<AssetStatsQuery>();
builder.Services.AddScoped<AssetSummaryQuery>();
builder.Services.AddScoped<IAssetService, AssetService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<TransactionsQuery>();
builder.Services.AddScoped<DashboardSummaryQuery>();
builder.Services.AddScoped<TransactionsHistoryQuery>();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
});

// JWT
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = config["Jwt:Issuer"],
            ValidAudience = config["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(config["Jwt:Key"]!))
        };
    });

const string CorsPolicy = "AppCors";
builder.Services.AddCors(opt =>
{
    opt.AddPolicy(CorsPolicy, p =>
        p.WithOrigins(config.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>())
         .AllowAnyHeader()
         .AllowAnyMethod()
         .AllowCredentials()); // optional
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(CorsPolicy);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
