var builder       = WebApplication.CreateBuilder(args);
var services      = builder.Services;
var configuration = builder.Configuration;
var appSetting    = AppSetting.MapValues(configuration);

// Cấu hình AppSettings và các dịch vụ vào DI container
services.AddDerivativeTradeServices(configuration, appSetting);

// Cấu hình Serilog từ appsettings.json (được gọi trước khi thêm các service khác)
builder.Host.UseSerilog();

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
// test mornitoring prometheus
var counter = Metrics.CreateCounter("my_custom_counter", "Số lần thực thi một số thao tác.");
counter.Inc();

// Gọi phương thức cấu hình các middleware từ AppConfiguration
app.ConfigureMiddleware();
app.UseRouting();
app.MapControllers();
app.MapMetrics();
app.Run();
