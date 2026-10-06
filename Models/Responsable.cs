using System.ComponentModel.DataAnnotations;

namespace Training_tunisie_telecome.Models
{
    public class Responsable
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Matricule { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;
    }
}