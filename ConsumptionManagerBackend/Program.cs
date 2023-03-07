using ConsumptionManagerBackend.Database;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using System.Configuration;
using System.Reflection;
using ConsumptionManagerBackend.Services;
using Microsoft.AspNetCore.Identity;
using ConsumptionManagerBackend.Database.DatabaseModels;

var builder = WebApplication.CreateBuilder(args);

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

var app = builder.Build();

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
