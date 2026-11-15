using Microsoft.AspNetCore.Mvc;
using RGIS_Gledalisce.Data;
using RGIS_Gledalisce.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace RGIS_Gledalisce.Controllers
{
    [ApiController]
    [Route("api/registracija")]
    public class RegistracijaController : ControllerBase
    {
        private readonly BazaContext _baza;

        public RegistracijaController(BazaContext baza)
        {
            _baza = baza;
        }


        [HttpPost]
        public async Task<IActionResult> Registriraj(RegistracijaZahteva zahteva)
        {
            // 1. Preverimo, ali so izpolnjena vsa polja.
            if (string.IsNullOrWhiteSpace(zahteva.Ime) ||
                string.IsNullOrWhiteSpace(zahteva.Priimek) ||
                string.IsNullOrWhiteSpace(zahteva.Email) ||
                string.IsNullOrWhiteSpace(zahteva.Geslo))
            {
                return BadRequest("Vsa polja so obvezna.");
            }

            // 2. Preverimo pravilnost e-postnega naslova.
            string email = zahteva.Email.Trim().ToLowerInvariant();

            if (!new EmailAddressAttribute().IsValid(email))
            {
                return BadRequest("E-postni naslov ni veljaven.");
            }

            // 3. Preverimo, ali uporabnik ze obstaja.
            bool obstaja = await _baza.Uporabniki
                .AnyAsync(u => u.Email == email);

            if (obstaja)
            {
                return Conflict("Uporabnik s tem e-postnim naslovom ze obstaja.");
            }

            // 4. Ustvarimo novega uporabnika.
            var uporabnik = new Uporabnik
            {
                Ime = zahteva.Ime.Trim(),
                Priimek = zahteva.Priimek.Trim(),
                Email = email
            };

            // 5. Geslo varno pretvorimo v hash.
            var hasher = new PasswordHasher<Uporabnik>();

            uporabnik.GesloHash = hasher.HashPassword(
                uporabnik, zahteva.Geslo);

            // 6. Uporabnika shranimo v SQLite.
            _baza.Uporabniki.Add(uporabnik);
            await _baza.SaveChangesAsync();

            // 7. Vrnemo odgovor brez gesla ali hasha.
            return StatusCode(201, new
            {
                uporabnik.Id,
                uporabnik.Ime,
                uporabnik.Priimek,
                uporabnik.Email,
                Sporocilo = "Registracija uspesna."
            });
        }

    }
}
