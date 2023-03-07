using ConsumptionManagerBackend.Database;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using System.Configuration;
using System.Reflection;
using ConsumptionManagerBackend.Services;
using Microsoft.AspNetCore.Identity;
using ConsumptionManagerBackend.Database.DatabaseModels;
using ConsumptionManagerBackend.Exceptions;
using ConsumptionManagerBackend;

var builder = WebApplication.CreateBuilder(args);


//read authentication settings from appsettings.json and bind them to appropriate class
AuthenticationSettings settings = new AuthenticationSettings();
builder.Configuration.GetSection("Jwt").Bind(settings);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<EnergySaverDbContext>(options => options.UseMySQL(builder.Configuration.GetConnectionString("ConnectionString")));
builder.Services.AddSwaggerGen();
//add services
builder.Services.AddScoped<IUserService,UserService>();
//add automapper
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
//add password hasher
builder.Services.AddScoped<IPasswordHasher<UserCredentials>,PasswordHasher<UserCredentials>>();
//add middleware
builder.Services.AddScoped<ExceptionHandlingMiddleware>();


var app = builder.Build();

//use middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
