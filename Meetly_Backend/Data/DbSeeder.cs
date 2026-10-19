using Meetly_Backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Meetly_Backend.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MeetlyDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            if (await db.Meetings.AnyAsync()) return; // already seeded

            // ---------- Users ----------
            var users = new List<User>();
            foreach (var name in new[] { "anna", "erik", "sara", "johan", "maria", "lars" })
            {
                var user = new User
                {
                    UserName = name,
                    Email = $"{name}@meetly.test",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(user, "Test1234!");
                if (!result.Succeeded)
                    throw new Exception("Seeding user failed: " +
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                users.Add(user);
            }
            var (anna, erik, sara, johan, maria, lars) =
                (users[0], users[1], users[2], users[3], users[4], users[5]);

            // ---------- Categories ----------
            var food = new Category { Title = "Mat & dryck" };
            var sport = new Category { Title = "Sport" };
            var tech = new Category { Title = "Teknik" };
            var music = new Category { Title = "Musik" };
            var games = new Category { Title = "Spel" };
            var nature = new Category { Title = "Natur" };
            db.Categories.AddRange(food, sport, tech, music, games, nature);

            // ---------- Meetings ----------
            var now = DateTime.UtcNow;

            var meetings = new List<Meeting>
            {
                // Upcoming
                new() { Title = "Brädspelskväll", Description = "Ta med ditt favoritspel, vi har fika.",
                        Location = "Kulturhuset, Varberg", Host = anna, Date = now.AddDays(5).Date.AddHours(18),
                        MaxAttendants = 8, Categories = [games, food] },
                new() { Title = "Morgonlöpning", Description = "Lugnt tempo, ca 5 km längs strandpromenaden.",
                        Location = "Societetsparken, Varberg", Host = erik, Date = now.AddDays(3).Date.AddHours(7),
                        MaxAttendants = 15, Categories = [sport, nature] },
                new() { Title = "C# Meetup", Description = "Lightning talks om .NET och EF Core.",
                        Location = "Campus Varberg", Host = sara, Date = now.AddDays(10).Date.AddHours(17),
                        MaxAttendants = 30, Categories = [tech] },
                new() { Title = "Öppen scen", Description = "Spela, sjung eller lyssna.",
                        Location = "Café Kajen, Varberg", Host = johan, Date = now.AddDays(14).Date.AddHours(19),
                        MaxAttendants = 25, Categories = [music] },
                new() { Title = "Fika i naturen", Location = "Getterön naturreservat",
                        Host = maria, Date = now.AddDays(7).Date.AddHours(14),
                        MaxAttendants = 3, Categories = [food, nature] }, // small, easy to fill
                new() { Title = "Matlagningskväll", Description = "Vi lagar tapas tillsammans.",
                        Location = "Köksstudion, Varberg", Host = lars, Date = now.AddDays(21).Date.AddHours(18),
                        MaxAttendants = 10, Categories = [food] },

                // Past (so reviews and grades make sense)
                new() { Title = "Sommarkonsert", Description = "Livemusik i parken.",
                        Location = "Fästningen, Varberg", Host = johan, Date = now.AddDays(-20).Date.AddHours(18),
                        MaxAttendants = 40, Categories = [music] },
                new() { Title = "Padelturnering", Description = "Dubbelmatcher, alla nivåer.",
                        Location = "Padelhallen, Varberg", Host = erik, Date = now.AddDays(-35).Date.AddHours(16),
                        MaxAttendants = 12, Categories = [sport] },
            };
            db.Meetings.AddRange(meetings);

            // ---------- Participants ----------
            // Upcoming meetings: no review or grade yet
            MeetingParticipant Join(Meeting m, User u, int? grade = null, string? review = null) =>
                new() { Meeting = m, User = u, Grade = grade, Review = review };

            db.MeetingParticipants.AddRange(
                Join(meetings[0], erik), Join(meetings[0], sara), Join(meetings[0], maria),
                Join(meetings[1], anna), Join(meetings[1], lars),
                Join(meetings[2], anna), Join(meetings[2], erik), Join(meetings[2], johan), Join(meetings[2], maria),
                Join(meetings[3], anna), Join(meetings[3], sara),
                Join(meetings[4], anna), Join(meetings[4], sara), Join(meetings[4], johan), // full (3/3)
                Join(meetings[5], anna), Join(meetings[5], maria),

                // Past meetings: with grades and reviews
                Join(meetings[6], anna, 5, "Fantastisk stämning och bra ljud!"),
                Join(meetings[6], sara, 4, "Trevlig kväll, lite trångt framför scenen."),
                Join(meetings[6], lars, 5, null),
                Join(meetings[6], maria, 3, "Bra musik men jag väntade länge i baren."),
                Join(meetings[7], anna, 4, "Kul och välorganiserat."),
                Join(meetings[7], johan, 5, "Bästa turneringen hittills."),
                Join(meetings[7], lars, 2, "Bokningen var rörig.")
            );

            await db.SaveChangesAsync();
        }
    }
}