using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using GestionareAngajatiAPI.Data;
using GestionareAngajatiAPI.Models;

namespace GestionareAngajatiAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _context;

        public AuthController(IConfiguration config, ApplicationDbContext context)
        {
            _config = config;
            _context = context;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            string hash = ComputeSha256Hash(request.Password);

            var user = _context.Utilizatori.FirstOrDefault(u =>
                u.Email == request.Email && u.ParolaHash == hash);

            if (user == null)
                return Unauthorized("Email sau parolă incorectă.");

            var token = GenerateJwtToken(user.Email, user.Rol);
            return Ok(new { token });
        }
		
		
		// [Authorize(Roles = "admin,hr")]
		// [HttpPost("register")]
		// public IActionResult Register([FromBody] RegisterRequest request)
		// {
			// var creatorEmail = User.Identity?.Name;
			// var creator = _context.Utilizatori.FirstOrDefault(u => u.Email == creatorEmail);

			// if (creator == null)
				// return Unauthorized();

			// // Doar admin poate crea admin/hr
			// if (creator.Rol == "hr" && request.Rol != "user")
				// return Forbid("HR poate crea doar utilizatori cu rol 'user'.");

			// if (_context.Utilizatori.Any(u => u.Email == request.Email))
				// return Conflict("Email deja înregistrat.");

			// var hash = ComputeSha256Hash(request.Password);

			// var user = new Utilizator
			// {
				// Email = request.Email,
				// ParolaHash = hash,
				// Rol = request.Rol,
				// IdAngajat = request.IdAngajat
			// };

			// _context.Utilizatori.Add(user);
			// _context.SaveChanges();

			// return Ok("Utilizator înregistrat cu succes.");
		// }
		
		[Authorize(Roles = "admin")]
		[HttpPut("change-role")]
		public IActionResult ChangeRole([FromBody] ChangeRoleRequest request)
		{
			var user = _context.Utilizatori.FirstOrDefault(u => u.Email == request.Email);
			if (user == null)
				return NotFound("Utilizatorul nu a fost găsit.");

			user.Rol = request.RolNou;
			_context.SaveChanges();

			return Ok($"Rolul utilizatorului {user.Email} a fost actualizat la '{user.Rol}'.");
		}


        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            var email = User.Identity?.Name;
            var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            return Ok(new
            {
                Email = email,
                Rol = role
            });
        }

        private string GenerateJwtToken(string email, string role)
        {
            var key = Encoding.UTF8.GetBytes(_config["JwtSettings:Key"]!);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, email),
                    new Claim(ClaimTypes.Role, role)
                }),
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(_config["JwtSettings:ExpiresInMinutes"]!)),
                Issuer = _config["JwtSettings:Issuer"],
                Audience = _config["JwtSettings:Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
        }

        private string ComputeSha256Hash(string rawData)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    // public class RegisterRequest
    // {
        // public string Email { get; set; } = string.Empty;
        // public string Password { get; set; } = string.Empty;
        // public string Rol { get; set; } = "user";
        // public int? IdAngajat { get; set; }
    // }
	

	public class ChangeRoleRequest
	{
		public string Email { get; set; } = string.Empty;
		public string RolNou { get; set; } = "user";
	}
}
