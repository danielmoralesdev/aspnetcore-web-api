using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using my_books.Data;
using my_books.Data.Services;
using my_books.Exceptions;
using Serilog;


WebApplicationBuilder builder = null;

//try
//{
var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(config)
    .CreateLogger();

//Log.Logger = new LoggerConfiguration()
//    .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day)
//    .CreateLogger();

builder = WebApplication.CreateBuilder(args);

//}
//finally
//{
//    Log.CloseAndFlush();
//}

builder.Host.UseSerilog();

// Add services to the container.

builder.Services.AddControllers();

// Configure DBContext with SQL
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString")));

// Configure the Services
builder.Services.AddTransient<BooksService>();
builder.Services.AddTransient<PublishersService>();
builder.Services.AddTransient<AuthorsService>();
builder.Services.AddTransient<LogsService>();

builder.Services.AddApiVersioning(config =>
{
    config.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    config.AssumeDefaultVersionWhenUnspecified = true;

    //config.ApiVersionReader = new HeaderApiVersionReader("custom-version-header");
    //config.ApiVersionReader = new MediaTypeApiVersionReader();
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

ILoggerFactory loggerFactory = new LoggerFactory();
//Exception handling
app.ConfigureBuildExceptionHandler(loggerFactory);
//app.ConfigureCustomExceptionHandler();

app.MapControllers();

//AppDbInitializer.Seed(app);

app.Run();
