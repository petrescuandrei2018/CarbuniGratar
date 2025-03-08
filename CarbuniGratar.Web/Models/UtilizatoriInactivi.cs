using System;
using System.ComponentModel.DataAnnotations;

namespace CarbuniGratar.Web.Models
{
    //pentru marketing, daca are un cos abandonat de 7 zile, ii salvam in bd si daca au email sau nrTel ii contactam
    public class UtilizatorInactiv
    {
        [Key]
        public int Id { get; set; }

        public int ClientId { get; set; } // Referință către Client
        public string Email { get; set; }
        public string Telefon { get; set; }
        public DateTime DataUltimeiActivitati { get; set; }
        public string Motiv { get; set; } = "Coș abandonat";
    }
}
