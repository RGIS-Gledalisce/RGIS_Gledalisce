namespace RGIS_Gledalisce.Models
{
    public class Uporabnik
    {
        public int Id { get; set; }

        public string Ime { get; set; } = string.Empty;

        public string Priimek { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string GesloHash { get; set; } = string.Empty;
    }
}
