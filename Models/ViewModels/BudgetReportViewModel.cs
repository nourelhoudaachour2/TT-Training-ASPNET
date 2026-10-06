using System;

namespace Training_tunisie_telecome.Models.ViewModels
{
    public class BudgetReportViewModel
    {
        public string Formation { get; set; } = string.Empty;

        public decimal TrainerCost { get; set; }

        public decimal HotelCost { get; set; }

        public decimal MealCost { get; set; }

        public decimal KitCost { get; set; }

        public decimal OtherCost { get; set; }

        public decimal Total { get; set; }

        public int ParticipantCount { get; set; }

        public DateTime DateStart { get; set; }

        public DateTime DateEnd { get; set; }

        public string TrainerName { get; set; } = string.Empty;

        public string Location { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;
    }
}