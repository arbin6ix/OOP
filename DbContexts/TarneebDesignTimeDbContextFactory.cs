using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace OOP4200_Tarneeb.DbContexts
{
    class TarneebDesignTimeDbContextFactory : IDesignTimeDbContextFactory<TarneebDbContext>
    {
        public TarneebDbContext CreateDbContext(string[] args)
        {
            DbContextOptions options = new DbContextOptionsBuilder().UseSqlite("Data Source=tarneeb.db").Options;

            return new TarneebDbContext(options);
        }
    }
}
