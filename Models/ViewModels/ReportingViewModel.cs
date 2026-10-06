using System;
using System.Collections.Generic;

namespace Training_tunisie_telecome.Models.ViewModels
{
    public class ReportingViewModel
    {


        // ================= KPI =================


        public int TotalFormations { get; set; }


        public int TotalEmployees { get; set; }


        public int TotalSessions { get; set; }


        public int TotalHours { get; set; }


        public decimal TotalBudget { get; set; }


        public double AttendanceRate { get; set; }


        public double SatisfactionRate { get; set; }



        // ================= DOMAIN =================


        public List<string> DomainLabels { get; set; }
            = new();


        public List<int> DomainValues { get; set; }
            = new();



        // ================= MONTH EVOLUTION =================


        public List<string> MonthLabels { get; set; }
            = new();


        public List<int> MonthValues { get; set; }
            = new();




        // ================= ATTENDANCE =================


        public int PresentCount { get; set; }


        public int AbsentCount { get; set; }


        public int ExcusedCount { get; set; }





        // ================= AI =================


        public List<string> AIRecommendations { get; set; }
            = new();



        public string MostPopularDomain { get; set; }
            = "";



        public string RecommendedTraining { get; set; }
            = "";




        // ================= COST =================


        public decimal TrainerCost { get; set; }


        public decimal HotelCost { get; set; }


        public decimal MealCost { get; set; }


        public decimal KitCost { get; set; }


        public decimal OtherCost { get; set; }



    }
}