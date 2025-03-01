using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarbuniGratar.Web.Models
{
    public class Produs
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Nume { get; set; }

        [Required]
        public string Descriere { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Pret { get; set; }

        [Required]
        public string ImagineUrl { get; set; }

        public bool InStoc { get; set; }

        // Relație: Un produs poate fi inclus în mai multe comenzi si by default lista este valida si goala, nu null cu erori
        public List<ComandaProdus> ComenziProduse { get; set; } = new List<ComandaProdus>();
    }
}
