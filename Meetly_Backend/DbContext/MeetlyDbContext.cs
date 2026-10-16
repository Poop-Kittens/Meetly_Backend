using Meetly_Backend.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Meetly_Backend.DbContext
{
    public class MeetlyDbContext : IdentityDbContext<User>//User matas från identitydbcontext
    {
        public DbSet<Category> Categories { get; set; }

        public DbSet<Meeting> Meetings { get; set; }

        public DbSet<MeetingParticipant> MeetingParticipants { get; set; }


        public MeetlyDbContext(DbContextOptions<MeetlyDbContext>options):base(options)
        {
            
        }


    }
}
