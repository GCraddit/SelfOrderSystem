using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Giles_Chen_test_1
{
    public class CafeContextFactory : IDesignTimeDbContextFactory<CafeContext>
    {
        public CafeContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CafeContext>();
            optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;AttachDbFilename=F:\\local\\Giles Chen test 1\\CafeDatabase.mdf;Integrated Security=True");

            return new CafeContext(optionsBuilder.Options);
        }
    }
}
