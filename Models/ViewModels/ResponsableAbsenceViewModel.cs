namespace Training_tunisie_telecome.Models.ViewModels
{
    public class ResponsableAbsenceItemViewModel
    {
        public int ParticipantId { get; set; }

        public string EmployeeName { get; set; } = string.Empty;

        public string Matricule { get; set; } = string.Empty;

        public string FormationTitle { get; set; } = string.Empty;

        public DateTime Date { get; set; }

        public string? JustificationFile { get; set; }

        public string AbsenceStatus { get; set; } = string.Empty;
    }


    public class ResponsableAbsencesPageViewModel
    {
        public List<ResponsableAbsenceItemViewModel> Pending { get; set; }
            = new();
    }
}