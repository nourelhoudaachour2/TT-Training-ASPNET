using System.ComponentModel.DataAnnotations;

namespace Training_tunisie_telecome.Models
{
    public class Service
    {
        public int Id { get; set; }


        [Required(ErrorMessage = "Le nom du service est obligatoire")]
        [StringLength(100, ErrorMessage = "Le nom du service ne peut pas dépasser 100 caractères")]
        public string Name { get; set; } = string.Empty;


        [StringLength(500, ErrorMessage = "La description ne peut pas dépasser 500 caractères")]
        public string? Description { get; set; }


        public List<Employee> Employees { get; set; } = new List<Employee>();
    }
}