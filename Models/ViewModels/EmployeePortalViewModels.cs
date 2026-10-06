namespace Training_tunisie_telecome.Models.ViewModels
{
    public class EmployeeProfileViewModel
    {
        public int EmployeeId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Matricule { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Grade { get; set; } = string.Empty;
        public string Residence { get; set; } = string.Empty;
        public string Domain { get; set; } = string.Empty;
        public string? ImagePath { get; set; }
        public int CompletedFormations { get; set; }
        public int TotalTrainingHours { get; set; }
        public double AttendanceRate { get; set; }
        public int CompletedEvaluations { get; set; }
    }

    public class EmployeeFormationItemViewModel
    {
        public int ParticipantId { get; set; }
        public int FormationId { get; set; }
        public int SessionId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Module { get; set; } = string.Empty;
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public string Location { get; set; } = string.Empty;
        public string Trainer { get; set; } = string.Empty;
        public bool? Confirmed { get; set; }
        public int? ReplacementEmployeeId { get; set; }
        public string? ReplacementEmployeeName { get; set; }
        public string PresenceStatus { get; set; } = "Non renseigné";
        public string EvaluationStatus { get; set; } = "À compléter";
    }

    public class EmployeeReplacementOptionViewModel
    {
        public int EmployeeId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Matricule { get; set; } = string.Empty;
        public string Domain { get; set; } = string.Empty;
    }

    public class EmployeeFormationsPageViewModel
    {
        public List<EmployeeFormationItemViewModel> Upcoming { get; set; } = new();

        public List<EmployeeReplacementOptionViewModel> AvailableReplacements { get; set; }
            = new();
    }

    public class EmployeeHistoryPageViewModel
    {
        public List<EmployeeFormationItemViewModel> Completed { get; set; } = new();
    }

    public class EmployeeEvaluationItemViewModel
    {
        public int ParticipantId { get; set; }
        public string FormationTitle { get; set; } = string.Empty;
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    public class EmployeeEvaluationsPageViewModel
    {
        public List<EmployeeEvaluationItemViewModel> Pending { get; set; } = new();
        public List<EmployeeEvaluationItemViewModel> Completed { get; set; } = new();
    }

    public class EmployeeAbsenceItemViewModel
    {
        public int ParticipantId { get; set; }

        public string FormationTitle { get; set; } = string.Empty;

        public DateTime Date { get; set; }

        public string Status { get; set; } = string.Empty;

        public string? AbsenceStatus { get; set; }

        public string? JustificationFile { get; set; }

        public DateTime? JustificationUploadedAt { get; set; }
    }

    public class EmployeeAbsencesPageViewModel
    {
        public List<EmployeeAbsenceItemViewModel> Absences { get; set; } = new();
    }

    public class EmployeeNotificationItemViewModel
    {
        public int Id { get; set; }

        public string Type { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public DateTime SentDate { get; set; }

        public string Status { get; set; } = string.Empty;

        public string? ActionUrl { get; set; }
        public string? AbsenceStatus { get; set; }
    }

    public class EmployeeNotificationsPageViewModel
    {
        public List<EmployeeNotificationItemViewModel> Notifications { get; set; } = new();
        public int UnreadCount { get; set; }
    }
}