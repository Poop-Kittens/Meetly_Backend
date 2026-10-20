namespace Meetly_Backend.DTOs
{
    public record BasicMeetingResponse(int Id, string Title, DateTime Date);
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

    public record SignUpInfo(DateTime Date, int RemainingSlots, bool AlreadyJoined);
}
