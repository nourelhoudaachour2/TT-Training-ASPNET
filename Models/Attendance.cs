namespace Training_tunisie_telecome.Models
{
    public class Attendance
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public int FormationId { get; set; }
        public Formation Formation { get; set; }

        public string Status { get; set; }
    }
}
