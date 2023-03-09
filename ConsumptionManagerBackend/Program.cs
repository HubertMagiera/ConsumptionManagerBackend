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

var builder = WebApplication.CreateBuilder(args);


//read authentication settings from appsettings.json and bind them to appropriate class
AuthenticationSettings settings = new AuthenticationSettings();
builder.Configuration.GetSection("Jwt").Bind(settings);
builder.Services.AddSingleton(settings);
//configure authentication with bearer
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
builder.Services.AddDbContext<EnergySaverDbContext>(options => options.UseMySQL(builder.Configuration.GetConnectionString("ConnectionString")));
builder.Services.AddSwaggerGen();
//add services
builder.Services.AddScoped<IUserService,UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ITariffService, TariffService>();
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
//add authentication
//for implementing jwt token authentication, both app.UseAuthentication() and app.UseAuthorization()
//are reqiured, but useAuthentication needs to be before useAuthorization
app.UseAuthentication();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
