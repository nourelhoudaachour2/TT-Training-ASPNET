using System;
using System.ComponentModel.DataAnnotations;

namespace Training_tunisie_telecome.Models
{
    public class Notification
    {
        public int Id { get; set; }


        // Agent qui reçoit la notification
        [Required]
        public int EmployeeId { get; set; }

        public Employee? Employee { get; set; }


        // Type notification
        [Required]
        public string Type { get; set; } = string.Empty;
        // Convocation / Absence / Evaluation / Remplacement


        // Contenu du message
        [Required]
        public string Message { get; set; } = string.Empty;


        // Date d'envoi
        public DateTime SentDate { get; set; } = DateTime.Now;


        // Statut
        public string Status { get; set; } = "Non lu";
        // Lu / Non lu / Envoyé
    }
}