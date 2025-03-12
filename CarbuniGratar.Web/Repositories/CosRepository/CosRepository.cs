using CarbuniGratar.Web.Data;
using CarbuniGratar.Web.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using IDatabase = StackExchange.Redis.IDatabase;

namespace CarbuniGratar.Web.Repositories.CosRepository
{
    public class CosRepository : ICosRepository
    {
        private readonly NepalezBazaDate _nepalezBazaDate;

        public CosRepository(NepalezBazaDate nepalezBazaDate)
        {
            _nepalezBazaDate = nepalezBazaDate;

        }

        public async Task<CosDeCumparaturi> AdaugaCosNouSauActualizeazaCosSqlAsync(CosDeCumparaturi cosDeCumparaturi)
        {
            using var tranzactie = await _nepalezBazaDate.Database.BeginTransactionAsync();
            try
            {
                // 🔹 Dacă ID-ul coșului este 0, înseamnă că este un coș nou, care nu a fost încă salvat în baza de date.
                // 🔹 Entity Framework atribuie ID-ul doar după apelarea SaveChangesAsync().
                if (cosDeCumparaturi.Id == 0)
                {
                    await _nepalezBazaDate.CosuriDeCumparaturi.AddAsync(cosDeCumparaturi); // 🔹 Adăugăm un coș NOU în SQL
                }
                else
                {
                    // 🔹 Dacă ID-ul este diferit de 0, înseamnă că acest coș există deja în baza de date și trebuie actualizat.
                    _nepalezBazaDate.CosuriDeCumparaturi.Update(cosDeCumparaturi); // 🔹 Actualizăm un coș EXISTENT în SQL
                }

                await _nepalezBazaDate.SaveChangesAsync(); // 🔹 Entity Framework salvează modificările și setează ID-ul pentru coșurile noi
                await tranzactie.CommitAsync(); // ✅ Confirmăm tranzacția

                return cosDeCumparaturi;
            }
            catch (Exception ex)
            {
                await tranzactie.RollbackAsync(); // 🚨 Anulăm modificările în caz de eroare
                Console.WriteLine($"Eroare la salvarea coșului în SQL: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Produs>> ObtineListaProduseDupaIdAsync(List<int> listaProduse)
        {
            // 🔹 4️⃣ Obținem produsele din baza de date
            return await _nepalezBazaDate.Produse
                .Where(produs => listaProduse.Contains(produs.Id))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<CosDeCumparaturi> ObtineCosDinSqlAsync(int clientId)
        {
            try
            {
                return await _nepalezBazaDate.CosuriDeCumparaturi
                .Include(listaCantitati => listaCantitati.ListaCantitatiProduseDinCosCumparaturi)
                .ThenInclude(listaProduse => listaProduse.Produs)
                .FirstOrDefaultAsync(cosDeCumparaturi => cosDeCumparaturi.ClientId == clientId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Eroare la obținerea coșului din SQL pentru client {clientId}: {ex.Message}");
                return null;
            }
        }


        public async Task ActualizeazaCosAsync(CosDeCumparaturi cosDeCumparaturi)
        {
            if(cosDeCumparaturi == null)
            {
                throw new ArgumentNullException(nameof(cosDeCumparaturi), "Cosul de cumparaturi nu poate fi null");
            }
            _nepalezBazaDate.CosuriDeCumparaturi.Update(cosDeCumparaturi);
            await _nepalezBazaDate.SaveChangesAsync();
        }



        public async Task<CosDeCumparaturi> CreeazaCosNouAsync(int clientId)
        {
            CosDeCumparaturi cosNouDeCumparaturi = new CosDeCumparaturi();
            cosNouDeCumparaturi.ClientId = clientId;

            await _nepalezBazaDate.CosuriDeCumparaturi.AddAsync(cosNouDeCumparaturi);
            await _nepalezBazaDate.SaveChangesAsync();
            return cosNouDeCumparaturi;
        }



        public async Task<Produs> ObtineProdusDinBdAsync(int produsId)
        {
            return await _nepalezBazaDate.Produse.FirstOrDefaultAsync(p => p.Id == produsId)
                ?? throw new KeyNotFoundException($"Produsul {produsId} nu a fost gasit");
        }




        public async Task<string> StergeCosDinSqlAsync(CosDeCumparaturi cosDeCumparaturi)
        {
            if (cosDeCumparaturi == null)
            {
                return "❌ Coșul nu a fost găsit în SQL.";
            }

            _nepalezBazaDate.CosuriDeCumparaturi.Remove(cosDeCumparaturi);
            await _nepalezBazaDate.SaveChangesAsync();

            return $"✅ Coșul clientului {cosDeCumparaturi.ClientId} a fost șters din SQL.";
        }



    }
}
