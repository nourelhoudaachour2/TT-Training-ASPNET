using System;
using System.Collections.Generic;

namespace Training_tunisie_telecome.Models.ViewModels
{
    public class DashboardViewModel
    {

        // ================= KPI =================

        public int TotalFormations { get; set; }

        public int TotalSessions { get; set; }

        public int TotalParticipants { get; set; }

        public int TotalEmployees { get; set; }

        public int TotalTrainers { get; set; }


        public decimal TotalBudget { get; set; }
        public double TotalTrainingHours { get; set; }



        // ================= INDICATORS =================


        public decimal AverageSessionCost { get; set; }


        public double AverageParticipants { get; set; }


        public string MostPopularDomain { get; set; } = "";


        public string MostUsedTrainer { get; set; } = "";



        // ================= CHART DOMAIN =================


        public List<string> DomainLabels { get; set; }
            = new();


        public List<int> DomainValues { get; set; }
            = new();



        // ================= CHART EVOLUTION =================


        public List<string> MonthlyLabels { get; set; }
            = new();


        public List<int> MonthlyValues { get; set; }
            = new();



        // ================= RECENT FORMATIONS =================


        public List<RecentFormationViewModel> RecentFormations { get; set; }
            = new();



        // ================= UPCOMING SESSION =================


        public List<UpcomingSessionViewModel> UpcomingSessions { get; set; }
            = new();


    }





    public class RecentFormationViewModel
    {

        public string Title { get; set; } = "";

        public DateTime Date { get; set; }

        public int Participants { get; set; }

    }





    public class UpcomingSessionViewModel
    {

        public string Formation { get; set; } = "";

        public DateTime Date { get; set; }

        public string Location { get; set; } = "";

        public int Participants { get; set; }

    }

}