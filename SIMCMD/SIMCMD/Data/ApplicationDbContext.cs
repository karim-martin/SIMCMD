using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SIMCMD.Core;
using SIMCMD.Models;

namespace SIMCMD.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityExtendUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {

        }

        public DbSet<ReportRequest> ReportRequest { get; set; }
        public DbSet<UserRole> UserRole { get; set; }
        public DbSet<MyBackgroundJob> MyBackgroundJob { get; set; }
        public DbSet<Provider> Provider { get; set; }
        public DbSet<FileConversion> FileConversion { get; set; }
        public DbSet<ManualFileUpload> ManualFileUpload { get; set; }
    }
}


