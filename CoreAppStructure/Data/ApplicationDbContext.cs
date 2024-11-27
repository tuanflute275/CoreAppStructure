namespace CoreAppStructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Category>            Categories { get; set; }
        public DbSet<Product>             Products   { get; set; }
        public DbSet<User>                Users      { get; set; }
        public DbSet<RoleModel.Role>      Roles      { get; set; }
        public DbSet<UserRole>            UserRoles  { get; set; }
        public DbSet<Tokens>              Tokens     { get; set; }
        public DbSet<Parameter>           Parameters { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var cascadeFKs = modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetForeignKeys())
                .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

            foreach (var fk in cascadeFKs)
                fk.DeleteBehavior = DeleteBehavior.Restrict;

            base.OnModelCreating(modelBuilder);
        }
    }
}
