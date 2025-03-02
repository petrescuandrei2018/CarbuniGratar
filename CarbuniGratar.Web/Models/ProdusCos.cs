namespace CarbuniGratar.Web.Models
{
    public class ProdusCos
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int ProdusId { get; set; }
        public int Cantitate { get; set; }
        
        public string NumeProdus { get; set; }
        public decimal Pret {  get; set; }
        public string ImageUrl { get; set; }
    }
}
