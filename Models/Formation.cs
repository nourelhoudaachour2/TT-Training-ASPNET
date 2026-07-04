using NuGet.DependencyResolver;

namespace Training_tunisie_telecome.Models
{
    public class Formation
    {
        public int Id { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }

        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }

        public string Location { get; set; }

        public int ServiceId { get; set; }
        public Service Service { get; set; }

        public int TrainerId { get; set; }
        public Trainer Trainer { get; set; }
        public string Status { get; set; } 
        public ICollection<Attendance> Attendances { get; set; }
    }
}
