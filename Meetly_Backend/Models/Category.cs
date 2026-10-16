using System.ComponentModel.DataAnnotations;

namespace Meetly_Backend.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required, MinLength(2)]
        public string Title { get; set; } = string.Empty;
    }
}
