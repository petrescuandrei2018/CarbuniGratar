using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarbuniGratar.Web.Models
{
    public class CosDeCumparaturi
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ClientId { get; set; } // ID-ul clientului

        [Required]
        public DateTime DataCreare { get; set; } = DateTime.Now;

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Total { get; set; } = 0; // Totalul coșului

        [Required]
        public StatusCosDeCumparaturi Status { get; set; } = StatusCosDeCumparaturi.CosFaraProduse;


        // Câmp care conține produsele în format JSON
        public string ProduseJson { get; set; }
    }
}
