using apifinal.BaseDados;
using apifinal.Services;
using apifinal.Services.DTOs;
using apifinal.Services.Validate;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
        .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<ProductDTOValidator>());

builder.Services.AddTransient<IValidator<ProductDTO>, ProductDTOValidator>();
builder.Services.AddTransient<IValidator<ProductUpdateDTO>, ProductUpdateDTOValidator>();
builder.Services.AddTransient<IValidator<ProductStockUpdateDTO>, ProductStockUpdateValidator>();

// Register AutoMapper
builder.Services.AddAutoMapper(Assembly.GetEntryAssembly());
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddControllers();
builder.Services.AddDbContext<TfDbContext>();
builder.Services.AddScoped<StockLogService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<PromotionService>();
builder.Services.AddScoped<SalesService>();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "API PARADIGMAS",
        Description = "Uma API Desenvolvida em ambiente acadêmico da Horus Faculdades",
        Contact = new OpenApiContact
        {
            Name = "Contato",
            Url = new Uri("https://github.com/4biDeN")
        },
        License = new OpenApiLicense
        {
            Name = "Linçenca By eu Mesmo",
            Url = new Uri("https://github.com/4biDeN")
        }
    });

    // using System.Reflection;
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

//builder.Logging.AddFile("Logs/apifinal-{Date}.log");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(c =>
    {
        c.SerializeAsV2 = true;
    });
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();