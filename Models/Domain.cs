using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Training_tunisie_telecome.Models
{
    public class Domain
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;


        public ICollection<Employee> Employees { get; set; }
            = new List<Employee>();


        public ICollection<FormationDomain> FormationDomains { get; set; }
            = new List<FormationDomain>();
    }
}