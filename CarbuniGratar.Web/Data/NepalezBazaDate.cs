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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CosDeCumparaturi>()
                .Property(c => c.Status)
                .HasConversion<string>(); // ✅ Salvăm enum-ul ca text în SQL

            base.OnModelCreating(modelBuilder);
        }
    }
}
