using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;

namespace Practical_Assignment_221519Z.Model
{
    public class AuthDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<AuditLog> AuditLog { get; set; }
        private readonly IConfiguration _configuration;
        //public AuthDbContext(DbContextOptions<AuthDbContext> options):base(options){ }
        public AuthDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = _configuration.GetConnectionString("AuthConnectionString"); optionsBuilder.UseSqlServer(connectionString);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the AuditLog entity
            modelBuilder.Entity<AuditLog>().ToTable("AuditLog");
        }

        public void AddToAuditLog(string username, string action)
        {
            
                var auditLogEntry = new AuditLog
                {
                    UserName = username,
                    Action = action,
                    TimeStamp = DateTime.Now.ToString(),
                };

                AuditLog.Add(auditLogEntry);
                SaveChanges();
            
        }

        public bool IsUserLoggedIn(string username)
        {
            var latestLogEntry = AuditLog.Where(log => log.UserName == username).OrderByDescending(log => log.TimeStamp).FirstOrDefault();
            if (latestLogEntry != null)
            {
                if (latestLogEntry.Action == "User Logged In")
                {
                    // User most recently logged in within the specified time window
                    return true;
                }
                return false;
            }
            return false;

        }
    }
}
