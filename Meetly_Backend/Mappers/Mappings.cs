using System.Linq.Expressions;
using Meetly_Backend.DTOs;
using Meetly_Backend.Models;

namespace Meetly_Backend.Mappers
{
    public static class Mappings
    {
        // ---- Expressions: used inside EF queries (translated to SQL) ----

        public static readonly Expression<Func<User, UserResponse>> UserExpr =
            u => new UserResponse(
                u.Id,
                u.UserName ?? string.Empty,
                u.Email ?? string.Empty);

        public static readonly Expression<Func<Meeting, BasicMeetingResponse>> BasicMeetingExpr =
            m => new BasicMeetingResponse(
                m.Id,
                m.Title,
                m.Date);

        public static readonly Expression<Func<Meeting, MeetingResponse>> MeetingExpr =
            m => new MeetingResponse(
                m.Id,
                m.Title,
                m.Description,
                m.Location,
                m.Date,
                m.MaxAttendants - m.Participants.Count,
                m.Participants
                    .Select(p => new MeetingParticipantResponse(
                        p.Review,
                        p.Grade,
                        p.User.UserName ?? ""))
                    .ToList(),
                m.Categories.Select(c => c.Title).ToList(),
                new UserResponse(
                    m.Host.Id,
                    m.Host.UserName ?? string.Empty,
                    m.Host.Email ?? string.Empty)
            );

        public static Expression<Func<Meeting, SignUpInfo>> SignUpInfoExpr(string userId) =>
                m => new SignUpInfo(
                m.Date,
                m.MaxAttendants - m.Participants.Count,
                m.Participants.Any(p => p.UserId == userId));

        // ---- Extension methods: for mapping objects already in memory ----
        // (declared after the expressions so static initialization order works)

        private static readonly Func<User, UserResponse> _user = UserExpr.Compile();
        private static readonly Func<Meeting, BasicMeetingResponse> _basicMeeting = BasicMeetingExpr.Compile();
        private static readonly Func<Meeting, MeetingResponse> _meeting = MeetingExpr.Compile();

        public static UserResponse ToResponse(this User u) => _user(u);
        public static BasicMeetingResponse ToBasicResponse(this Meeting m) => _basicMeeting(m);
        public static MeetingResponse ToResponse(this Meeting m) => _meeting(m);
    }
}