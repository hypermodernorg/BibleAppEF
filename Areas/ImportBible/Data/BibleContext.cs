using BibleAppEF.Areas.ImportBible.Models;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace BibleAppEF.Areas.ImportBible.Data
{
    public class BibleContext : DbContext
    {
        public DbSet<Register> Registers { get; set; }
        public DbSet<Bible> Bibles { get; set; }
        public DbSet<UserNotes> Notes { get; set; }
        public DbSet<Books> Books { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseMySql("server=localhost;uid=hypermodern;pwd=Nisarascalcj1!h;database=biblebase",
                   MySqlServerVersion.LatestSupportedServerVersion,
                        mySqlOptions => mySqlOptions
                            .CharSetBehavior(CharSetBehavior.NeverAppend)
                            .EnableRetryOnFailure())
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging();
        }
    }
}
