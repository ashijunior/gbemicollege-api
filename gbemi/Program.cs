using gbemi.Contexts;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using gbemi.Repository.Implementation;
using gbemi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using gbemi.Repository.Interface;
using AutoMapper;
using System.Web.Http;
using Microsoft.Extensions.Logging;
using Serilog;
using gbemi.UtilityService;

var builder = WebApplication.CreateBuilder(args);

//for Log4Net file settings
Log.Logger = new LoggerConfiguration().
    MinimumLevel.Information()
    .WriteTo.File("Log/log.txt",
    rollingInterval: RollingInterval.Minute)
    .CreateLogger();

//Use this line to override the built-in loggers
builder.Host.UseSerilog();

//Use serilog along with built-in loggers
builder.Logging.AddSerilog();

// Add services to the container.

//Add signup and login to database
builder.Services.AddCors(option =>
{
    option.AddPolicy("MyPolicys", builder =>
    {
        builder.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});


//User sql connection
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer
    (builder.Configuration.GetConnectionString("SqlServerConnectionStr"));
});

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("veryverysecret.....")),
        ValidateAudience = false,
        ValidateIssuer = false
    };
});

builder.Services.AddControllers();

builder.Services.AddScoped<IEmailService, EmailService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//mapper
builder.Services.AddScoped<IUserRepo, UserRepo>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("MyPolicys");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                      