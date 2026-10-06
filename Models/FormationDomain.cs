using System.ComponentModel.DataAnnotations;

namespace Training_tunisie_telecome.Models
{
    public class FormationDomain
    {
        public int Id { get; set; }



        [Required]
        public int FormationId { get; set; }

        public Formation? Formation { get; set; }



        [Required]
        public int DomainId { get; set; }

        public Domain? Domain { get; set; }
    }
}