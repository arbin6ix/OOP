using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace OOP4200_Tarneeb.DbContexts
{
    public class TarneebDbContextFactory
    {
        private readonly string _connectionString;

        public TarneebDbContextFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public TarneebDbContext CreateDbContext()
        {
            DbContextOptions options = new DbContextOptionsBuilder().UseSqlite(_connectionString).Options;

            return new TarneebDbContext(options);
        }
    }
}
