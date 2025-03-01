using System.ComponentModel.DataAnnotations.Schema;

namespace CarbuniGratar.Web.Models
{
    public class ComandaProdus
    {
        public int ComandaId { get; set; }
        public Comanda Comanda { get; set; }

        public int ProdusId { get; set; }
        public Produs Produs { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal PretUnitate { get; set; }

        public int Cantitate { get; set; }
    }
}
