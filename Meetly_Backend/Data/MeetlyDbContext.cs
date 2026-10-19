using Meetly_Backend.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Meetly_Backend.Data
{
    public class MeetlyDbContext : IdentityDbContext<User>//User matas från identitydbcontext
    {
        public DbSet<Meeting> Meetings => Set<Meeting>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<MeetingParticipant> MeetingParticipants => Set<MeetingParticipant>();


        public MeetlyDbContext(DbContextOptions<MeetlyDbContext>options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b); // required for Identity tables

            // Host: one user hosts many meetings
            b.Entity<Meeting>()
                .HasOne(m => m.Host)
                .WithMany(u => u.HostedMeetings)
                .HasForeignKey(m => m.HostId)
                .OnDelete(DeleteBehavior.Restrict);

            // Participant -> Meeting
            b.Entity<MeetingParticipant>()
                .HasOne(mp => mp.Meeting)
                .WithMany(m => m.Participants)
                .HasForeignKey(mp => mp.MeetingId)
                .OnDelete(DeleteBehavior.Cascade);

            // Participant -> User
            b.Entity<MeetingParticipant>()
                .HasOne(mp => mp.User)
                .WithMany(u => u.Participations)
                .HasForeignKey(mp => mp.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Prevent duplicate sign-ups
            b.Entity<MeetingParticipant>()
                .HasIndex(mp => new { mp.UserId, mp.MeetingId })
                .IsUnique();

            // One-sided many-to-many for categories
            b.Entity<Meeting>()
                .HasMany(m => m.Categories)
                .WithMany(c => c.Meetings);

            // DB-level checks (SQL Server syntax; drop the brackets and use "Grade" for PostgreSQL/SQLite)
            b.Entity<MeetingParticipant>()
                .ToTable(t => t.HasCheckConstraint("CK_MeetingParticipant_Grade", "[Grade] BETWEEN 1 AND 5"));
            b.Entity<Meeting>()
                .ToTable(t => t.HasCheckConstraint("CK_Meeting_MaxAttendants", "[MaxAttendants] >= 1"));
        }
    }
}
