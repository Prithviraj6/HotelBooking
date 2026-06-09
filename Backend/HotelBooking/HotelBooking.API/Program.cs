using HotelBooking.API.Extensions;
using HotelBooking.API.Middleware;
using HotelBooking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// ─── Serilog — Beautiful Structured Logging ──────────────────────────────────
// Console: human-readable with emoji level indicators and source context
// File:    structured JSON for production log aggregators (Seq, ELK, Loki)
builder.Host.UseSerilog((ctx, lc) => lc
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Query", LogEventLevel.Error)
    .MinimumLevel.Override("LuckyPennySoftware.AutoMapper", LogEventLevel.Error)
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .WriteTo.Console(
        outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}\n" +
        "  {Message:lj}{NewLine}{Exception}",
        restrictedToMinimumLevel: LogEventLevel.Information)
    .WriteTo.File(
        path: "logs/log-.json",
        rollingInterval: RollingInterval.Day,
        formatter: new Serilog.Formatting.Json.JsonFormatter(),
        retainedFileCountLimit: 30)
    .ReadFrom.Configuration(ctx.Configuration));

// ─── Services ────────────────────────────────────────────────────────────────
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString;
    });
builder.Services.AddEndpointsApiExplorer();

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Extensions — order: Repos → Services → Mappings → Validation → Auth → UI
builder.Services.AddRepositories();
builder.Services.AddApplicationServices();
builder.Services.AddMappings();      // AutoMapper
builder.Services.AddValidation();    // FluentValidation
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddSwaggerDocumentation();
builder.Services.AddRateLimiting();
builder.Services.AddAuthorization();

// CORS — AllowAll for development
// In production, replace "AllowAll" with a named policy using WithOrigins("https://yourfrontend.com")
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

var app = builder.Build();

// ─── Seed Data ───────────────────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        DataSeeder.SeedData(context);
    }
    catch (Exception ex)
    {
        Log.Error(ex, "An error occurred while seeding the database.");
    }
}

// ─── Middleware Pipeline — ORDER MATTERS ─────────────────────────────────────
app.UseMiddleware<ExceptionMiddleware>();        // 1. Catch all exceptions
app.UseSerilogRequestLogging();                  // 2. Standard ASP.NET request logging

app.UseStaticFiles(); // Serve images from wwwroot
app.UseCors("AllowAll");
app.UseSwaggerDocumentation();
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

Log.Information("🚀 HotelBooking API started | Environment={Environment}", app.Environment.EnvironmentName);

app.Run();