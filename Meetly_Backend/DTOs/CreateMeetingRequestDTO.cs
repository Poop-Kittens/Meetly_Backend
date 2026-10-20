using System.ComponentModel.DataAnnotations;

namespace Meetly_Backend.DTOs
{
    public record CreateMeetingRequestDTO(
        [Required, MaxLength(100)]
        string Title,

        [MaxLength(350)]
        string? Description,

        [Required]
        string Location,

        DateTime Date,

        [Range(1, int.MaxValue)]
        int MaxAttendants,

        [Required, MinLength(1)]
        List<int> CategoryIds
    );
}
