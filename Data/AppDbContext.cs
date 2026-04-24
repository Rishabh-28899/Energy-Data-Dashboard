using Microsoft.EntityFrameworkCore;
using Emergy_report.models;   // adjust if needed

namespace Emergy_report.Data
{
    public class AppDbContext : DbContext   // ✅ MUST inherit
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)   // ✅ MUST call base
        {
        }

        public DbSet<Emerguapp> Emerguapp { get; set; }
    }
}
