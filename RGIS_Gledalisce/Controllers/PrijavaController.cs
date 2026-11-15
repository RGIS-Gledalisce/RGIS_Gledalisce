using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using RGIS_Gledalisce.Data;
using RGIS_Gledalisce.Models;
using Microsoft.AspNetCore.Authorization;


namespace RGIS_Gledalisce.Controllers
{
    [ApiController]
    [Route("api/prijava")]
    public class PrijavaController : ControllerBase
    {
        private readonly BazaContext _baza;

        public PrijavaController(BazaContext baza)
        {
            _baza = baza;
        }


        [HttpPost]
        public async Task<IActionResult> Prijava([FromBody] PrijavaZahteva zahteva)
        {
            if (string.IsNullOrWhiteSpace(zahteva.Email) ||
                string.IsNullOrWhiteSpace(zahteva.Geslo))
            {
                return BadRequest(new { sporocilo = "E-pošta in geslo sta obvezna." });
            }

            string email = zahteva.Email.Trim().ToLowerInvariant();

            var uporabnik = await _baza.Uporabniki
                .FirstOrDefaultAsync(u => u.Email == email);

            if (uporabnik == null)
            {
                return Unauthorized(new { sporocilo = "Napačna e-pošta ali geslo." });
            }

            var preverjanje = new PasswordHasher<Uporabnik>()
                .VerifyHashedPassword(uporabnik, uporabnik.GesloHash, zahteva.Geslo);

            if (preverjanje == PasswordVerificationResult.Failed)
            {
                return Unauthorized(new { sporocilo = "Napačna e-pošta ali geslo." });
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, uporabnik.Id.ToString()),
                new Claim(ClaimTypes.Name, uporabnik.Ime),
                new Claim(ClaimTypes.Email, uporabnik.Email)
            };

            var identiteta = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identiteta);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal);

            return Ok(new
            {
                sporocilo = "Prijava je uspešna.",
                uporabnik.Id,
                uporabnik.Ime,
                uporabnik.Priimek,
                uporabnik.Email
            });
        }


        [Authorize]
        [HttpGet("profil")]
        public IActionResult Profil()
        {
            return Ok(new
            {
                sporocilo = "Dostop do zaščitenih podatkov je dovoljen.",
                id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                ime = User.FindFirst(ClaimTypes.Name)?.Value,
                email = User.FindFirst(ClaimTypes.Email)?.Value
            });
        }


        [Authorize]
        [HttpPost("odjava")]
        public async Task<IActionResult> Odjava()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return Ok(new
            {
                sporocilo = "Odjava je uspešna."
            });
        }
    }
}
