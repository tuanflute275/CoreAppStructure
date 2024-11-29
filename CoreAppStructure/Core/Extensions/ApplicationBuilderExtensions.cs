namespace CoreAppStructure.Core.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Nạp dữ liệu từ DbContext vào bộ nhớ trong (In-Memory) khi khởi động ứng dụng.
        /// Phương thức này thực hiện gọi `SeedAsync` để khởi tạo dữ liệu trong bộ nhớ từ cơ sở dữ liệu.
        /// </summary>
        /// <param name="app">WebApplication.</param>
        public static void LoadDataFromDbContextToMemory(this WebApplication app)
        {
            app.LoadDataToMemory<CategoryMemory, ApplicationDbContext>((dataInMem, dbContext) =>
            {
                // Gọi SeedAsync để nạp dữ liệu từ ApplicationDbContext vào CategoryMemory
                new CategoryMemorySeedAsync().SeedAsync(dataInMem, dbContext).Wait();
            });
        }
    }
}
