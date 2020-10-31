﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Scaffolding;
using BibleAppEF.Areas.ImportBible.Models;

namespace BibleAppEF.Areas.ImportBible.Data
{
    public class BibleContext : DbContext
    {
        public DbSet<Register> Registers { get; set; }
        public DbSet<Bible> Bibles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL("server=localhost;uid=root;pwd=Nisarascalcj1!r;database=biblebase");
        }
    }
}
