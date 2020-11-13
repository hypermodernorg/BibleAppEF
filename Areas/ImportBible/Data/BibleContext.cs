using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Scaffolding;
using BibleAppEF.Areas.ImportBible.Models;
using Newtonsoft.Json.Linq;
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
                .UseMySql("server=localhost;uid=root;pwd=Nisarascalcj1!r;database=biblebase",
                   MySqlServerVersion.LatestSupportedServerVersion,
                        mySqlOptions => mySqlOptions
                            .CharSetBehavior(CharSetBehavior.NeverAppend))
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging();
        }
    }
}
