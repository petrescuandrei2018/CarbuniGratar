using CarbuniGratar.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace CarbuniGratar.Web.Data
{
    public class NepalezBazaDate : DbContext
    {
        public NepalezBazaDate(DbContextOptions<NepalezBazaDate> instructiuni) : base(instructiuni) { }

        public DbSet<Produs> Produse { get; set; }
        public DbSet<Comanda> Comenzi { get; set; }
        public DbSet<Client> Clienti { get; set; }
        public DbSet<ComandaProdus> ComenziProduse { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ComandaProdus>()
                .HasKey(cp => new { cp.ComandaId, cp.ProdusId });

            modelBuilder.Entity<ComandaProdus>()
                .HasOne(cp => cp.Comanda)
                .WithMany(cp => cp.ProduseComandate)
                .HasForeignKey(cp => cp.ComandaId);

            modelBuilder.Entity<ComandaProdus>()
                .HasOne(cp => cp.Produs)
                .WithMany(cp => cp.ComenziProduse)
                .HasForeignKey(cp => cp.ProdusId);
        }
    }
}
