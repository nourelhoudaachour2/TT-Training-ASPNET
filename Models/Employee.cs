using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Training_tunisie_telecome.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le matricule est obligatoire")]
        [StringLength(20, ErrorMessage = "Le matricule ne doit pas dépasser 20 caractères")]
        public string Matricule { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le nom est obligatoire")]
        [StringLength(50)]
        [RegularExpression(@"^[a-zA-ZÀ-ÿ\s]+$",
            ErrorMessage = "Le nom doit contenir uniquement des lettres")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le prénom est obligatoire")]
        [StringLength(50)]
        [RegularExpression(@"^[a-zA-ZÀ-ÿ\s]+$",
            ErrorMessage = "Le prénom doit contenir uniquement des lettres")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "L'email est obligatoire")]
        [EmailAddress(ErrorMessage = "Format email invalide")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le téléphone est obligatoire")]
        [RegularExpression(@"^[0-9]{8}$",
            ErrorMessage = "Le numéro doit contenir exactement 8 chiffres")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le grade est obligatoire")]
        public string Grade { get; set; } = string.Empty;

        [Required(ErrorMessage = "La résidence est obligatoire")]
        public string Residence { get; set; } = string.Empty;

        public string? ImagePath { get; set; }

        // Compte d'authentification employé
        public bool AuthenticationEnabled { get; set; } = false;

        public string? PasswordHash { get; set; }

        public bool MustChangePassword { get; set; } = false;

        // Domaine de l'agent
        [Required(ErrorMessage = "Veuillez sélectionner un domaine")]
        public int DomainId { get; set; }
        public Domain? Domain { get; set; }

        // Relation avec les sessions et participations
        public ICollection<SessionParticipant>? SessionParticipants { get; set; }
    }
}