using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Training_tunisie_telecome.Models
{
    public class Trainer
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le matricule est obligatoire")]
        [StringLength(20, ErrorMessage = "Le matricule ne peut pas dépasser 20 caractères")]
        public string Matricule { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le prénom est obligatoire")]
        [StringLength(50, ErrorMessage = "Le prénom ne peut pas dépasser 50 caractères")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le nom est obligatoire")]
        [StringLength(50, ErrorMessage = "Le nom ne peut pas dépasser 50 caractères")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "L'email est obligatoire")]
        [EmailAddress(ErrorMessage = "Email invalide")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le téléphone est obligatoire")]
        [RegularExpression(@"^[0-9]{8}$",
            ErrorMessage = "Le numéro doit contenir 8 chiffres")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "L'adresse est obligatoire")]
        [StringLength(200, ErrorMessage = "L'adresse ne peut pas dépasser 200 caractères")]
        public string Address { get; set; } = string.Empty;

        public string? ImagePath { get; set; }

        // Type interne / externe
        [Required(ErrorMessage = "Veuillez sélectionner le type de formateur")]
        public string TrainerType { get; set; } = "Interne"; // "Interne" ou "Externe"

        // Si externe
        public string? Organization { get; set; }
        public decimal? Fee { get; set; }

        // Relation avec Sessions
        public ICollection<SessionFormation> Sessions { get; set; }
    = new List<SessionFormation>();
    }
}