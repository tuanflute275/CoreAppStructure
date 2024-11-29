namespace CoreAppStructure.Data.SeedDataToMemoryAsync
{
    public class CategoryMemorySeedAsync
    {
        public async Task SeedAsync(CategoryMemory memory, ApplicationDbContext dbContext)
        {
            var data = await dbContext.Categories.ToListAsync();

            if (data.Count > 0)
            {
                foreach (var item in data)
                {
                    memory.CategoryInMemory.Add(item.CategoryId.ToString(), item);
                }
            }
        }
    }
}
