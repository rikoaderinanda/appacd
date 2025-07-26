using System.Data;
using appacd.Models;
using appacd.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddControllersWithViews()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
        options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
    });

builder.Services.AddEndpointsApiExplorer();
// Tambahkan Swagger
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IDbConnection>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("DefaultConnection");
    return new Npgsql.NpgsqlConnection(connectionString);
});
builder.Services.AddScoped<ILayananRepository, LayananRepository>();
builder.Services.AddScoped<IPemesananRepository, PemesananRepository>();
builder.Services.AddScoped<IPerangkatRepository, PerangkatRepository>();
builder.Services.AddHttpClient<ITripayRepository, TripayRepository>();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
            "https://localhost:7298",
            "https://mitraacd.onrender.com"
            )
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Tambahkan ini
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
    c.RoutePrefix = "swagger"; // akses Swagger di /swagger
});

app.UseHttpsRedirection();

app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        if (ctx.File.Name == "manifest.json" || ctx.File.Name == "sw.js")
        {
            ctx.Context.Response.Headers.Append("Cache-Control", "no-cache");
        }
    }
});

app.UseRouting();
app.UseCors();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
