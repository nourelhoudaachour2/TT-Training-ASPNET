using System;

namespace Training_tunisie_telecome.Models.ViewModels
{
    public class FormationReportViewModel
    {

        public string FormationName { get; set; } = "";

        public string Domain { get; set; } = "";


        public int Sessions { get; set; }


        public int Participants { get; set; }


        public int Hours { get; set; }


        public decimal Cost { get; set; }


        public DateTime? LastSession { get; set; }

    }
}