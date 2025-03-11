using CarbuniGratar.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace CarbuniGratar.Web.Data
{
    public class NepalezBazaDate : DbContext
    {
        public NepalezBazaDate(DbContextOptions<NepalezBazaDate> instructiuni) : base(instructiuni) { }

        public DbSet<Produs> Produse { get; set; }
        public DbSet<CosDeCumparaturi> CosuriDeCumparaturi { get; set; } // 🔥 Înlocuim `Comanda`
        public DbSet<UtilizatorInactiv> UtilizatoriInactivi { get; set; }

        public DbSet<Client> Clienti { get; set; }

        public DbSet<ListaCuCantitatileProduselorDinCosCumparaturi> ListeCuCantitatileProduselorDinCosCumparaturi { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CosDeCumparaturi>()
                .Property(c => c.Status)
                .HasConversion<string>(); // ✅ Salvăm enum-ul ca text în SQL

            modelBuilder.Entity<ListaCuCantitatileProduselorDinCosCumparaturi>()
                .HasOne(listaCantitatiProduse => listaCantitatiProduse.CosDeCumparaturi)
                .WithMany(produs => produs.ListaCantitatiProduseDinCosCumparaturi)
                .HasForeignKey(cosDeCumparaturi => cosDeCumparaturi.CosDeCumparaturiId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ListaCuCantitatileProduselorDinCosCumparaturi>()
                .HasOne(produs => produs.Produs)
                .WithMany()
                .HasForeignKey(listaProdus => listaProdus.ProdusId)
                .OnDelete(DeleteBehavior.Cascade);  // ✅ Dacă ștergem un produs, îl ștergem și din coșurile active


            base.OnModelCreating(modelBuilder);
        }
    }
}
