using CoreAppStructure.Core.Configurations;
using CoreAppStructure.Core.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;
// Cấu hình AppSettings và các dịch vụ vào DI container
services.AddDerivativeTradeServices(configuration);

// Cấu hình Serilog từ appsettings.json (được gọi trước khi thêm các service khác)
builder.Host.UseSerilog(); // Đảm bảo Serilog được cấu hình trước khi bất kỳ dịch vụ nào được thêm vào


// Add services to the container.
builder.Services.AddControllers();

// Cấu hình Swagger/OpenAPI cho API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// Gọi phương thức cấu hình các middleware từ AppConfiguration
AppConfiguration.ConfigureMiddleware(app);

// Chạy ứng dụng
app.UseRouting();
app.MapControllers();
app.Run();
