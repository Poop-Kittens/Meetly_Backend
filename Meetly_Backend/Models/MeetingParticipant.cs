using System.ComponentModel.DataAnnotations;

namespace Meetly_Backend.Models
{
    public class MeetingParticipant
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;
        public int MeetingId { get; set; }

        [MaxLength(1000)]
        public string? Review { get; set; }
        public int? Grade { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;
        public Meeting Meeting { get; set; } = null!;
    }
}
