using CoreAppStructure.Core.Configurations;
using CoreAppStructure.Core.Middlewares;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Cấu hình AppSettings và các dịch vụ vào DI container
builder.Services.ConfigureServices(builder.Configuration);

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

// Cấu hình các middleware
app.UseCors("AllowOrigin");  // CORS policy
app.UseStaticFiles();        // Cung cấp các file tĩnh (nếu có)
app.UseHttpsRedirection();   // Chuyển hướng tất cả yêu cầu HTTP sang HTTPS

// Middleware cho xác thực và phân quyền
app.UseAuthentication();
app.UseAuthorization();

// Kết nối các controller vào pipeline
app.MapControllers();

// Cấu hình middleware cho xử lý ngoại lệ (ExceptionMiddleware)
app.UseMiddleware<ExceptionMiddleware>();

// Ghi log các request vào Serilog
app.UseSerilogRequestLogging(); // Ghi log các request HTTP

// Chạy ứng dụng
app.Run();
