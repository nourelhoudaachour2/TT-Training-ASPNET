using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Training_tunisie_telecome.Models;

namespace Training_tunisie_telecome.Models.ViewModels
{

    // =========================
    // DOMAIN + NOMBRE EMPLOYES
    // =========================

    public class DomainParticipantViewModel
    {

        public int Id { get; set; }


        public string Name { get; set; } = string.Empty;


        public int EmployeeCount { get; set; }

    }






    public class FormationCreateViewModel
    {


        // =========================
        // INFORMATIONS FORMATION
        // =========================


        [Required(ErrorMessage = "Le code formation est obligatoire")]
        public string Code { get; set; } = string.Empty;



        [Required(ErrorMessage = "Le titre est obligatoire")]
        public string Title { get; set; } = string.Empty;



        public string Module { get; set; } = string.Empty;



        public string Description { get; set; } = string.Empty;



        public int Duration { get; set; }



        public string? Objectives { get; set; }







        // =========================
        // POPULATION CIBLE
        // =========================


        public List<int> SelectedDomains { get; set; }
            = new List<int>();



        public int NumberOfParticipants { get; set; }







        // =========================
        // SESSION
        // =========================


        [Required(ErrorMessage = "La date début est obligatoire")]
        public DateTime DateStart { get; set; }





        [Required(ErrorMessage = "La date fin est obligatoire")]
        public DateTime DateEnd { get; set; }





        public int NumberOfDays
        {
            get
            {
                if (DateEnd >= DateStart)
                {
                    return (DateEnd - DateStart).Days + 1;
                }

                return 0;
            }
        }






        [Required(ErrorMessage = "Le type est obligatoire")]
        public string Type { get; set; } = string.Empty;





        public string Location { get; set; } = string.Empty;





        [Required(ErrorMessage = "Le formateur est obligatoire")]
        public int TrainerId { get; set; }







        // =========================
        // COUTS
        // =========================


        public decimal TrainerCost { get; set; }



        public decimal HotelNightPrice { get; set; }



        public decimal PenPrice { get; set; }



        public decimal NotebookPrice { get; set; }



        public decimal ToteBagPrice { get; set; }



        public decimal RestaurantTicketPrice { get; set; }



        public decimal LunchPrice { get; set; }







        // =========================
        // CALCUL AUTOMATIQUE
        // =========================


        public decimal HotelTotal
        {
            get
            {
                return HotelNightPrice * NumberOfDays;
            }
        }





        public decimal KitTotal
        {
            get
            {
                return NumberOfParticipants *
                       (PenPrice +
                        NotebookPrice +
                        ToteBagPrice);
            }
        }





        public decimal FoodTotal
        {
            get
            {
                return NumberOfParticipants *
                       NumberOfDays *
                       (RestaurantTicketPrice +
                        LunchPrice);
            }
        }





        public decimal TotalCost
        {
            get
            {
                return TrainerCost
                       + HotelTotal
                       + KitTotal
                       + FoodTotal;
            }
        }







        // =========================
        // LISTES
        // =========================


        public List<DomainParticipantViewModel> Domains { get; set; }
            = new List<DomainParticipantViewModel>();





        public List<Trainer> Trainers { get; set; }
            = new List<Trainer>();





        public List<string> InternalLocations { get; set; }
            = new List<string>();





        public List<string> Hotels { get; set; }
            = new List<string>();



    }

}