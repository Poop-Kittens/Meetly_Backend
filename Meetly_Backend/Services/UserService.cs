using Meetly_Backend.Common;
using Meetly_Backend.Interfaces;
using Meetly_Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace Meetly_Backend.Services
{
    // Service som hanterar användarrelaterad logik.
    // Implementerar interfacet IUserService.
    public class UserService : IUserService
    {
        // Identity-klasser som hanterar användare och inloggning.
        // readonly innebär att fälten inte kan tilldelas nya värden
        // efter att konstruktorn har körts.
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        // Konstruktor som tar emot UserManager och SignInManager
        // via Dependency Injection.
        public UserService(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            // Sparar referenserna så att de kan användas i servicen.
            _signInManager = signInManager;
            _userManager = userManager;

        }

        // Asynkron metod som registrerar en ny användare.
        // Tar emot RegisterRequest med e-postadress och lösenord.
        // Returnerar ServiceResult<string> med resultatet av registreringen.
        public async Task<ServiceResult<string>> RegisterAsync(RegisterRequest request)
        {
            // Skapar ett nytt User-objekt.
            // Användarens e-post används även som användarnamn.
            var user = new User
            {
                UserName = request.Email,
                Email = request.Email
            };

            // ASP.NET Core Identity försöker skapa användaren.
            // Identity validerar lösenordet, hashar det och sparar
            // användaren i databasen via sin konfigurerade UserStore.
            // await väntar asynkront på att operationen ska slutföras.
            var result = await _userManager.CreateAsync(user, request.Password);

            // Kontrollerar om registreringen misslyckades.
            if (!result.Succeeded)
            {
                // Returnerar ett misslyckat ServiceResult med felmeddelande.
                return ServiceResult.Invalid<string>("Registreringen misslyckades!");
            }
            else
            {

                // Returnerar ett lyckat ServiceResult med bekräftelse.
                return ServiceResult.Ok("Du är registrerad");
            }
        }

        public async Task<ServiceResult<string>> LogonAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)//letar efter användare ifall dom redan finns eller ej
            {

                return ServiceResult.Invalid<string>("Användaren hittades inte");
            }
            var result = await _signInManager.PasswordSignInAsync(user, request.Password, isPersistent:true, lockoutOnFailure:true) ;//skapar och sätter cookie automatiskt och signar in
            if (!result.Succeeded)
            {
                // Returnerar ett misslyckat ServiceResult med felmeddelande.
                return ServiceResult.Invalid<string>("Fel användarnamn eller lösenord!");
            }
            else
            {

                // Returnerar ett lyckat ServiceResult med bekräftelse.
                return ServiceResult.Ok("Inloggning lyckades");
            }


        }

      
        public async Task<ServiceResult<string>> LogoutAsync()//Använder sig av cookien som skapades innan. Därför behöver man inte ange en användare här
        {
             await _signInManager.SignOutAsync();
            return ServiceResult.Ok("Utloggad");
        }

    }
}