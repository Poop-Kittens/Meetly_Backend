using Microsoft.AspNetCore.Identity;

namespace Meetly_Backend.Models
{
    public class User : IdentityUser //separat paket för att hantera inlogg
    {
        // Navigationproperty för att länka samman våra tabeller
        public List<MeetingParticipant> Participations { get; set; } = [];
        public List<Meeting> HostedMeetings { get; set; } = [];
    }
}
