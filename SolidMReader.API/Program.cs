using Microsoft.EntityFrameworkCore;
using SolidMReader.Data.Context;
using SolidMReader.Models.DTO;
using SolidMReader.Services.Interfaces;
using SolidMReader.Services.Repositories;
using SolidMReader.Services.Services;
using SolidMReader.Services.Validation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("SolidMReaderConnection");

builder.Services.AddDbContext<SolidMReaderContext>(options =>
{
    options.UseSqlServer(connectionString);
});

builder.Services.AddScoped<IMeterReadingsRepository, MeterReadingsRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IValidation<MeterReading>, MeterReadingValidationRules>();
builder.Services.AddScoped<ICsvMeterReadingsProcessor, CsvMeterReadingsProcessor>();

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.Run();

public partial class Program() { }