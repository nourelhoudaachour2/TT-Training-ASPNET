namespace Training_tunisie_telecome.Models.ViewModels
{
    public class EmployeeDashboardViewModel
    {
        public int EmployeeId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Matricule { get; set; } = string.Empty;

        public string Domain { get; set; } = string.Empty;

        public string? ImagePath { get; set; }

        public double AttendanceRate { get; set; }

        public int PlannedFormations { get; set; }

        public int CompletedFormations { get; set; }

        public int PendingEvaluations { get; set; }

        public int UnreadNotifications { get; set; }

        public List<EmployeeUpcomingFormationViewModel> UpcomingFormations { get; set; }
            = new();
    }

    public class EmployeeUpcomingFormationViewModel
    {
        public int ParticipantId { get; set; }

        public int FormationId { get; set; }

        public int SessionId { get; set; }

        public string Title { get; set; } = string.Empty;

        public DateTime DateStart { get; set; }

        public DateTime DateEnd { get; set; }

        public string Location { get; set; } = string.Empty;

        public string Trainer { get; set; } = string.Empty;

        public bool? Confirmed { get; set; }
    }
}