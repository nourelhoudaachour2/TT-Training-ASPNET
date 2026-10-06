using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Training_tunisie_telecome.Models
{
    public class Formation
    {
        public int Id { get; set; }



        [Required(ErrorMessage = "Le code formation est obligatoire")]
        [StringLength(50, ErrorMessage = "Le code ne doit pas dépasser 50 caractères")]
        public string Code { get; set; } = string.Empty;



        [Required(ErrorMessage = "Le titre est obligatoire")]
        [StringLength(150, ErrorMessage = "Le titre ne doit pas dépasser 150 caractères")]
        public string Title { get; set; } = string.Empty;



        public string Description { get; set; } = string.Empty;



        [Required(ErrorMessage = "Le module est obligatoire")]
        [StringLength(100, ErrorMessage = "Le module ne doit pas dépasser 100 caractères")]
        public string Module { get; set; } = string.Empty;



        [Required(ErrorMessage = "La durée est obligatoire")]
        [Range(1, 365, ErrorMessage = "La durée doit être supérieure à 0")]
        public int Duration { get; set; }



        public string? Objectives { get; set; }



        // Population cible
        [Required(ErrorMessage = "Veuillez sélectionner au moins un domaine")]
        public ICollection<FormationDomain> FormationDomains { get; set; }
            = new List<FormationDomain>();



        // Sessions de cette formation
        public ICollection<SessionFormation> Sessions { get; set; }
            = new List<SessionFormation>();



        // Statut catalogue
        public bool IsActive { get; set; } = true;
    }
}