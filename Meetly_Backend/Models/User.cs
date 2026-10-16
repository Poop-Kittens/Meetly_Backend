using System.Data.Common;
using Microsoft.AspNetCore.Identity;

namespace Meetly_Backend.Models
{
    public class User:IdentityUser//separat paket för att hantera inlogg
    {
        //Navigationproperty för att länka samman våra tabeller
        public List<Meeting> Meetings { get; set; } = [];
    }
}
