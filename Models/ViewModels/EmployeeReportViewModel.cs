namespace Training_tunisie_telecome.Models.ViewModels
{
    public class EmployeeReportViewModel
    {

        public string FullName { get; set; } = "";


        public string Matricule { get; set; } = "";


        public string Domain { get; set; } = "";


        public int FormationCount { get; set; }


        public int TrainingHours { get; set; }


        public double AttendanceRate { get; set; }


        public DateTime? LastFormation { get; set; }

    }
}