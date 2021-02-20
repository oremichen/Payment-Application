using ContentServiceManagementAPI.Models;
using ContentServiceManagementAPI.Models.DomainModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ContentServiceManagementAPI.Data
{
    public class ANQContentServiceManageDb : DbContext
    {
        private static readonly string Test213ConnectionString = $"Data Source=10.0.0.67;Initial Catalog=Coure.Anq.CoureManagementSystemDb;Persist Security Info=True;User ID=sa;Password=coure202*";

       

        private readonly ConnectionStrings _options;

        public ANQContentServiceManageDb(DbContextOptions<ANQContentServiceManageDb> options)
        : base(options)
        {
        }

        public ANQContentServiceManageDb()
        {
        }
        public DbSet<Content> Content { get; set; }
        public DbSet<MapContentToServiceProvider> MapContentToServiceProvider { get; set; }
        public DbSet<MapServiceToClient> MapServiceToClient { get; set; }

        public DbSet<ContentProvider> ContentProvider { get; set; }
        public DbSet<ServiceProvider> ServiceProvider { get; set; }
        public DbSet<ServiceContentMapp> ServiceContentMap { get; set; }

        public DbSet<Client> Client { get; set; }
        public DbSet<Service> Service { get; set; }
        public DbSet<VasSystemService> VasSystemServices { get; set; }

        public DbSet<CommandRecord> CommandRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        //public class ANQContentServiceManageDbDesignFactory: IDesignTimeDbContextFactory<ANQContentServiceManageDb>
        //{
        //    public ANQContentServiceManageDb CreateDbContext(string[] args)
        //    {
        //        var optionsBuilder = new DbContextOptionsBuilder<ANQContentServiceManageDb>()
        //            .UseSqlServer(Test213ConnectionString);
        //        return new ANQContentServiceManageDb(optionsBuilder.Options);

        //    }
        //}
    }
}
