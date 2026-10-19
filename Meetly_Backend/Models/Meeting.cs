using System.ComponentModel.DataAnnotations;

namespace Meetly_Backend.Models
{
    public class Meeting
    {
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(300)]
        public string? Description { get; set; }

        [Required]
        public string Location { get; set; } = string.Empty;
        public string HostId { get; set; } = string.Empty;

        [Required]//vi skapar en egen regel för datumhantering
        public DateTime Date { get; set; }
        public int MaxAttendants { get; set; }


        //Navigationproperties för att länka samman våra tabeller
        public List<MeetingParticipant> Participants { get; set; } = [];
        public List<Category> Categories { get; set; } = [];
        public User Host { get; set; } = null!;
    }
}
