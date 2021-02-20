using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using PaymentCore;

namespace PaymentRepository.Data
{
    public class PaymentDb : DbContext
    {
        public PaymentDb(DbContextOptions<PaymentDb> options)
        : base(options)
        {
        }

        public PaymentDb()
        {
        }
        public DbSet<Payments> Payments { get; set; }

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
