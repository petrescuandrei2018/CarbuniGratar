﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarbuniGratar.Web.Models
{
    public class Client
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nume { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        public string Telefon { get; set; }

        [Required]
        [StringLength(255)]
        public string Adresa { get; set; }

        // Relație: Un client poate avea mai multe comenzi si primeste o lista goala default nu o lista null care ar da erori
        public List<Comanda> Comenzi { get; set; } = new List<Comanda>();
    }
}
