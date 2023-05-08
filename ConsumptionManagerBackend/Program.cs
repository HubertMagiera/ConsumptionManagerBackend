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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ConsumptionManagerBackend.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);


//read authentication settings from appsettings.json and bind them to appropriate class
AuthenticationSettings settings = new AuthenticationSettings();
builder.Configuration.GetSection("Jwt").Bind(settings);
//.AddSingleton(settings) means there is only one instance of this class
builder.Services.AddSingleton(settings);
//configure authentication with bearer token
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.SaveToken = true;
    //options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,//issuer identifies who created the token
        ValidateAudience = true,//identifies the recipients of the token
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = settings.Issuer,
        ValidAudience = settings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key))
    };
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//add db context in order to be able to connect with the database
//connection string is stored in appsettings.json file
//EnergySaverDbContext is a class which defines all of the tables from database and classes in a program which are their equivalent
builder.Services.AddDbContext<EnergySaverDbContext>(options => options.UseMySQL(builder.Configuration.GetConnectionString("ConnectionString")));
builder.Services.AddSwaggerGen();
//add services
builder.Services.AddScoped<IUserService,UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ITariffService, TariffService>();
builder.Services.AddScoped<IDeviceService,DeviceService>();
builder.Services.AddScoped<IMeasurementService,MeasurementService>();
builder.Services.AddHttpContextAccessor();
//add automapper
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
//add password hasher
builder.Services.AddScoped<IPasswordHasher<UserCredentials>,PasswordHasher<UserCredentials>>();
//add middleware which handles all types of errors
builder.Services.AddScoped<ExceptionHandlingMiddleware>();


var app = builder.Build();

//use middleware which handles all types of errors
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//add authentication
//for implementing jwt token authentication, both app.UseAuthentication() and app.UseAuthorization()
//are reqiured, but useAuthentication needs to be before useAuthorization
app.UseAuthentication();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
