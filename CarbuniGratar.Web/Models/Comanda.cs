using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarbuniGratar.Web.Models
{
    public class Comanda
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ClientId { get; set; }
        public Client Client { get; set; }

        [Required]
        public DateTime DataPlasare { get; set; } = DateTime.Now;

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Total { get; set; }

        public string Status { get; set; } = "In curs de plasare";

        // Relație: O comandă poate conține mai multe produse
        public List<ComandaProdus> ProduseComandate { get; set; } = new List<ComandaProdus>();
    }
}
