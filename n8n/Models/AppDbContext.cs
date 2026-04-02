using Microsoft.EntityFrameworkCore;

namespace n8n.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<AiMessageLog> AiMessageLogs { get; set; }
        public DbSet<AgentStatus> AgentStatuses { get; set; }
    }
}
