using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Training_tunisie_telecome.Models
{
    public class SessionFormation
    {
        public int Id { get; set; }


        [Required(ErrorMessage = "La formation est obligatoire")]
        public int FormationId { get; set; }

        public Formation? Formation { get; set; }



        [Required(ErrorMessage = "Le formateur est obligatoire")]
        public int TrainerId { get; set; }

        public Trainer? Trainer { get; set; }



        [Required(ErrorMessage = "La date de début est obligatoire")]
        public DateTime DateStart { get; set; }



        [Required(ErrorMessage = "La date de fin est obligatoire")]
        public DateTime DateEnd { get; set; }



        [Required(ErrorMessage = "Le type est obligatoire")]
        public string Type { get; set; } = string.Empty;



        [Required(ErrorMessage = "Le lieu est obligatoire")]
        [StringLength(200)]
        public string Location { get; set; } = string.Empty;



        public string Status { get; set; } = "À venir";



        public ICollection<SessionParticipant> Participants { get; set; }
            = new List<SessionParticipant>();



        [Range(0, double.MaxValue,
            ErrorMessage = "Le coût formateur doit être positif")]
        public decimal TrainerCost { get; set; }



        public decimal HotelNightPrice { get; set; }

        public decimal PenPrice { get; set; }

        public decimal NotebookPrice { get; set; }

        public decimal ToteBagPrice { get; set; }

        public decimal RestaurantTicketPrice { get; set; }



        [Range(0, double.MaxValue,
            ErrorMessage = "Le coût hôtel doit être positif")]
        public decimal HotelCost { get; set; }



        [Range(0, double.MaxValue,
            ErrorMessage = "Le coût repas doit être positif")]
        public decimal MealCost { get; set; }



        [Range(0, double.MaxValue,
            ErrorMessage = "Le coût kit doit être positif")]
        public decimal KitCost { get; set; }



        [Range(0, double.MaxValue,
            ErrorMessage = "Les autres coûts doivent être positifs")]
        public decimal OtherCost { get; set; }



        public decimal TotalCost
        {
            get
            {
                return TrainerCost
                     + HotelCost
                     + MealCost
                     + KitCost
                     + OtherCost;
            }
        }
    }
}