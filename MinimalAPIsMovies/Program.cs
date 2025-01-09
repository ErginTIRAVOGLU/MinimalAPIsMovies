using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using MinimalAPIsMovies.Data;
using MinimalAPIsMovies.Endpoints;
using MinimalAPIsMovies.Entities;
using MinimalAPIsMovies.Migrations;
using MinimalAPIsMovies.Repositories;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer("name=DefaultConnection"));


builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(configuration =>
    {
        configuration.WithOrigins(builder.Configuration["AllowedOrigins"]!)
               .AllowAnyMethod()
               .AllowAnyHeader();
    });

    options.AddPolicy("AllowAll", configuration =>
    {
        configuration.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
    /*
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
    */
});

builder.Services.AddOutputCache();

builder.Services.AddScoped<IGenresRepository, GenresRepository>();

builder.Services.AddAutoMapper(typeof(Program));

var app = builder.Build();

app.UseCors();

app.UseOutputCache();

app.MapOpenApi();

app.MapScalarApiReference();

app.MapGet("/", [EnableCors(policyName: "AllowAll")] () => "Hello World!").CacheOutput(config => config.Expire(TimeSpan.FromSeconds(15)));

app.MapGroup("/genres").MapGenres();


app.Run();

