namespace Training_tunisie_telecome.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string Matricule { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; } 
        public int ServiceId { get; set; }
        public Service Service { get; set; }

    }
}
