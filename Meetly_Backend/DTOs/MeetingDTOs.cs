namespace Meetly_Backend.DTOs
{
    public record BasicMeetingResponse(string Id, string Title, DateTime Date);
    public record MeetingResponse(
        string Id,
        string Title,
        string? Description,
        string Location,
        DateTime Date,
        int RemainingSlots,
        List<MeetingParticipantResponse> Participants,
        List<string> Categories,
        UserResponse Host
    );

    public record MeetingParticipantResponse(string? Review, int? Grade, UserResponse User);

    public record MeetingSearchRequest(
        string? SearchTerm, // matches title or description
        string? Location,
        DateOnly? Date,
        string? Category
    );
}
