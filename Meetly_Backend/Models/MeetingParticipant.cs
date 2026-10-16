using System.ComponentModel.DataAnnotations;

namespace Meetly_Backend.Models
{
    public class MeetingParticipant
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public int MeetingId { get; set; }

        public string? Review { get; set; }

        [Range(1,5)]
        public int? Grade { get; set; }
    }
}
