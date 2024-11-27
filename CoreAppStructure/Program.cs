﻿var builder       = WebApplication.CreateBuilder(args);
var services      = builder.Services;
var configuration = builder.Configuration;
var appSetting    = AppSetting.MapValues(configuration);

// Cấu hình AppSettings và các dịch vụ vào DI container
services.AddDerivativeTradeServices(configuration, appSetting);

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
app.ConfigureMiddleware();

// Chạy ứng dụng
app.UseRouting();
app.MapControllers();
app.Run();
