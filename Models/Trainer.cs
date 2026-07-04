namespace Training_tunisie_telecome.Models
{
    public class Trainer
    {
        public int Id { get; set; }
        public string FirstNAme { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public ICollection<Formation> Formations { get; set; }

    }
}
