using System;
using System.ComponentModel.DataAnnotations;

namespace Training_tunisie_telecome.Models
{
    public class Evaluation
    {
        public int Id { get; set; }

        [Required]
        public int SessionParticipantId { get; set; }
        public SessionParticipant? SessionParticipant { get; set; }

        [Required]
        public string Type { get; set; } = string.Empty;
        // "À chaud" = avant formation, "À froid" = après formation

        public int? Rating { get; set; }

        public string? Comment { get; set; }

        public DateTime? CreatedAt { get; set; } = DateTime.Now;
    }
}