using System.ComponentModel.DataAnnotations;

namespace Meetly_Backend.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Title { get; set; } = string.Empty;

        // Navigation properties
        public List<Meeting> Meetings { get; set; } = [];
    }
}
