using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarbuniGratar.Web.Models
{
    public class ListaCuCantitatileProduselorDinCosCumparaturi
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CosDeCumparaturiId { get; set; }

        [Required]
        public int ProdusId { get; set; }

        [Required]
        public int Cantitate { get; set; }

        // 🔹 Definim relațiile corect cu ForeignKey
        [ForeignKey(nameof(CosDeCumparaturiId))]
        public CosDeCumparaturi CosDeCumparaturi { get; set; }

        [ForeignKey(nameof(ProdusId))]
        public Produs Produs { get; set; }
    }
}
