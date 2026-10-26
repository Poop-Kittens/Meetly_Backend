using System.ComponentModel.DataAnnotations;

namespace Meetly_Backend.DTOs
{
    public record BasicMeetingResponse(int Id, string Title, DateTime Date, string Location, int RemainingSlots, List<string> Categories);

    public record MeetingResponse(
        int Id,
        string Title,
        string? Description,
        string Location,
        DateTime Date,
        int RemainingSlots,
        List<MeetingParticipantResponse> Participants,
        List<string> Categories,
        UserResponse Host
    );

    public record MeetingParticipantResponse(
        string? Review,
        int? Grade,
        string UserName
    );

    public record MeetingSearchRequest(
        string? SearchTerm, // matches title or description
        string? Location,
        DateOnly? Date,
        string? Category
    );

    public record MeetingReviewRequest(
        [MaxLength(1000)] string? Review,
        [Range(1, 5)] int? Grade
    );

    public record CreateMeetingRequest(
        [Required, MaxLength(50)] string Title,
        [MaxLength(300)] string? Description,
        [Required] string Location,
        DateTime Date,
        [Range(1, int.MaxValue)] int MaxAttendants,
        [Required, MinLength(1)] List<int> CategoryIds
    );

    public record SignUpInfo(
        DateTime Date,
        int RemainingSlots,
        bool AlreadyJoined,
        bool IsHost
    );

    public record CategoryResponse(
        int Id,
        string Title
     );
    public record UpdateMeetingRequest(
        [Required, MaxLength(50)] string Title,
        [MaxLength(300)] string? Description,
        [Required] string Location,
        DateTime Date,
        [Range(1, int.MaxValue)] int MaxAttendants,
        [Required, MinLength(1)] List<int> CategoryIds
    );
}
