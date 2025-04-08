using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using GestionareAngajatiAPI.Data;
using GestionareAngajatiAPI.Models;

namespace GestionareAngajatiAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AngajatiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AngajatiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "admin,hr")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var angajati = await _context.Angajati.ToListAsync();
            return Ok(angajati);
        }

        [Authorize(Roles = "admin,hr,user")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var angajat = await _context.Angajati.FindAsync(id);
            if (angajat == null) return NotFound();
            return Ok(angajat);
        }


		[Authorize(Roles = "admin,hr")]
		[HttpPost]
		public async Task<IActionResult> Create([FromBody] Angajat angajat)
		{
			if (_context.Angajati.Any(a => a.Email == angajat.Email))
				return Conflict("Există deja un angajat cu acest email.");

			_context.Angajati.Add(angajat);
			await _context.SaveChangesAsync(); // aici se generează Id-ul

			// Creăm automat utilizator cu parola hash: nume_yyyyMMdd
			var parola = $"{angajat!.Nume!.ToLower()}_{angajat!.DataNasterii!:yyyyMMdd}";
			var hash = ComputeSha256Hash(parola);

			if (!_context.Utilizatori.Any(u => u.Email == angajat.Email))
			{
				var user = new Utilizator
				{
					Email = angajat.Email!,
					ParolaHash = hash,
					Rol = "user",
					IdAngajat = angajat.Id
				};

				_context.Utilizatori.Add(user);
				await _context.SaveChangesAsync();
			}

			return CreatedAtAction(nameof(Get), new { id = angajat.Id }, angajat);
		}


        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Angajat updated)
        {
            var angajat = await _context.Angajati.FindAsync(id);
            if (angajat == null) return NotFound();

            angajat.Nume = updated.Nume;
            angajat.Prenume = updated.Prenume;
            angajat.Email = updated.Email;
            angajat.Telefon = updated.Telefon;
            angajat.DataNasterii = updated.DataNasterii;
            angajat.DataAngajare = updated.DataAngajare;
            angajat.IdDepartament = updated.IdDepartament;
            angajat.IdPozitie = updated.IdPozitie;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var angajat = await _context.Angajati.FindAsync(id);
            if (angajat == null) return NotFound();

            _context.Angajati.Remove(angajat);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // Utilitar: hash SHA-256
        private string ComputeSha256Hash(string rawData)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }
}
