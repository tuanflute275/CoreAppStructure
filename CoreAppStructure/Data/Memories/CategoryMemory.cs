namespace CoreAppStructure.Data.Memories
{
    public class CategoryMemory
    {
        public Dictionary<string, Category> CategoryInMemory { get; set; }

        public CategoryMemory()
        {
            CategoryInMemory = new Dictionary<string, Category>();
        }
    }
}
