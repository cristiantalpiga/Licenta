namespace GestionareAngajatiAPI.Models
{
    public class Angajat
    {
        public int? Id { get; set; }
        public required string? Nume { get; set; }
        public required string? Prenume { get; set; }
        public required string? Email { get; set; }
        public required string? Telefon { get; set; }
        public DateTime DataNasterii { get; set; }
        public DateTime DataAngajare { get; set; }
        public int? IdDepartament { get; set; }
        public int? IdPozitie { get; set; }
    }
}
