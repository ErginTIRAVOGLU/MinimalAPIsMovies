using FluentValidation;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Diagnostics; 
using Microsoft.EntityFrameworkCore;
using MinimalAPIsMovies.Data;
using MinimalAPIsMovies.Endpoints;
using MinimalAPIsMovies.Entities; 
using MinimalAPIsMovies.Repositories;
using MinimalAPIsMovies.Services;
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
builder.Services.AddScoped<IActorsRepository, ActorsRepository>();
builder.Services.AddScoped<IMoviesRepository, MoviesRepository>();
builder.Services.AddScoped<ICommentsRepository, CommentsRepositories>();
builder.Services.AddScoped<IErrorsRepository, ErrorsRepository>();

//builder.Services.AddTransient<IFileStorage, AzureFileStorage>();
builder.Services.AddTransient<IFileStorage, LocalFileStorage>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddAutoMapper(typeof(Program));    

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddProblemDetails();

builder.Services.AddAuthentication().AddJwtBearer();
builder.Services.AddAuthorization();    

var app = builder.Build();

app.UseExceptionHandler(exceptionHandlerApp => exceptionHandlerApp.Run( async context =>
{
    var exceptionHandlerFeature=context.Features.Get<IExceptionHandlerFeature>();
    var exception = exceptionHandlerFeature?.Error!;

    var error = new Error();
    error.Date = DateTime.UtcNow;
    error.ErrorMessage=exception.Message;
    error.StackTrace= exception.StackTrace;

    var repository=context.RequestServices.GetRequiredService<IErrorsRepository>();
    await repository.Create(error);

    await Results
        .BadRequest(new
        {
            type = "error",
            message = "an unexpected exception has occurred",
            status = 500
        }).ExecuteAsync(context);
}));
app.UseStatusCodePages();

app.UseStaticFiles();

app.UseCors();

app.UseOutputCache();

app.UseAuthorization();

app.MapOpenApi();

app.MapScalarApiReference();

app.MapGet("/", [EnableCors(policyName: "AllowAll")] () => "Hello World!").CacheOutput(config => config.Expire(TimeSpan.FromSeconds(15)));

app.MapGet("/error", () =>
{
    throw new InvalidOperationException("example error");
});
app.MapGroup("/genres").MapGenres();
app.MapGroup("/actors").MapActors();
app.MapGroup("/movies").MapMovies();
app.MapGroup("/movie/{movieId:int}/comments").MapComments();


app.Run();

