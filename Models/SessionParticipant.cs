using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Training_tunisie_telecome.Models
{
    public class SessionParticipant
    {
        public int Id { get; set; }



        // Agent concerné
        [Required]
        public int EmployeeId { get; set; }

        public Employee? Employee { get; set; }



        // Session concernée
        [Required]
        public int SessionFormationId { get; set; }

        public SessionFormation? SessionFormation { get; set; }



        // Confirmation participation : Oui / Non
        public bool? Confirmed { get; set; }



        // Remplaçant proposé
        public int? ReplacementEmployeeId { get; set; }

        public Employee? ReplacementEmployee { get; set; }



        // Présence actuelle
        // Conservé pour compatibilité avec l'existant
        public bool? Present { get; set; }



        // Statut absence
        // En attente / Justifiée / Non justifiée
        public string? AbsenceStatus { get; set; }



        // Justificatif uploadé
        public string? JustificationFile { get; set; }

        public DateTime? JustificationUploadedAt { get; set; }

        // Historique des présences
        public ICollection<Attendance> Attendances { get; set; }
            = new List<Attendance>();
    }
}