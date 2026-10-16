using System.ComponentModel.DataAnnotations;

namespace Meetly_Backend.Models
{
    public class Meeting
    {
        
        public int Id { get; set; }
        [Required, Length(2,30)]
        public string Title { get; set; } = string.Empty;
        [MaxLength(300)]
        public string? Description { get; set; }

        [Required]
        public string Location { get; set; } = string.Empty;
        public string? Host { get; set; }

        [Required]//vi skapar en egen regel för datumhantering
        public DateTime Date { get; set; }

        [Range(1,int.MaxValue)]
        public int MaxAttendants { get; set; }


        //Navigationproperties för att länka samman våra tabeller
        public List<User>? Users { get; set; }

        public List<Category>? Categories { get; set; }


    }
}
