using CrossCutting.Ioc;
using CrossCutting.Ioc.Infra;
using CrossCutting.Ioc.Mappers;
using WebApi.Configurations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddJwtBearerAuthentication();
builder.Services.AddSwagger();

// AutoMapper configuration
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.RegisterMappings();

builder.Services.RegisterServices(builder.Configuration);

// Cors configuration
builder.Services.AddCors(options =>
{
    var allowOrigins = builder.Configuration.GetValue<string>("ALLOW_ORIGINS") ?? "*";
    options.AddPolicy("CorsConfig", builder => builder.WithOrigins(allowOrigins.Split(';')).AllowAnyMethod().AllowAnyHeader());
});


var app = builder.Build();

using var scope = app.Services.CreateScope();
scope.McTechDatabaseInitialize();

app.UseSwaggerV3();
app.UseCors("CorsConfig"); 
app.UseAuth();

app.MapControllers();

await app.RunAsync();
