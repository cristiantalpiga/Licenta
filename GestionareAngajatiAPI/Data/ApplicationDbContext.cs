// Data/ApplicationDbContext.cs
using Microsoft.EntityFrameworkCore;
using GestionareAngajatiAPI.Models;

namespace GestionareAngajatiAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

        public DbSet<Angajat> Angajati { get; set; }
		public DbSet<Utilizator> Utilizatori { get; set; }

    }
}
