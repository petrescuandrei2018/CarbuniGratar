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
        private readonly IDatabase _cache;
        private const string CachePrefix = "Cos_";

        public CosRepository(NepalezBazaDate nepalezBazaDate, IConnectionMultiplexer redis)
        {
            _nepalezBazaDate = nepalezBazaDate;
            _cache = redis.GetDatabase();
        }


        // ✅ Verifică dacă există produse în coș
        public async Task<bool> ExistaProduseInCosAsync(int clientId)
        {
            // 🔹 1️⃣ Încercăm să obținem coșul mai întâi din Redis
            var cosDeCumparaturi = await ObtineCosDinRedisAsync(clientId);

            // 🔹 2️⃣ Dacă coșul nu este în Redis, îl căutăm în SQL
            if (cosDeCumparaturi == null)
            {
                cosDeCumparaturi = await ObtineCosDinSqlAsync(clientId);
            }

            // 🔹 3️⃣ Dacă coșul nu există nici în Redis, nici în SQL, returnăm `false`
            if (cosDeCumparaturi == null)
            {
                return false;
            }

            // 🔹 4️⃣ Verificăm dacă lista de produse din coș există
            bool listaNuEsteNull;
            if (cosDeCumparaturi.ListaCantitatiProduseDinCosCumparaturi != null)
            {
                listaNuEsteNull = true; // ✅ Lista există
            }
            else
            {
                listaNuEsteNull = false; // ❌ Lista este `null`
            }

            // 🔹 5️⃣ Verificăm dacă lista conține produse
            bool listaAreProduse;
            if (listaNuEsteNull)
            {
                if (cosDeCumparaturi.ListaCantitatiProduseDinCosCumparaturi.Any())
                {
                    listaAreProduse = true; // ✅ Lista conține produse
                }
                else
                {
                    listaAreProduse = false; // ❌ Lista este goală
                }
            }
            else
            {
                listaAreProduse = false; // ❌ Dacă lista este `null`, implicit este și goală
            }

            // 🔹 6️⃣ Dacă lista este `null` sau goală, coșul este gol și returnăm `false`
            if (listaNuEsteNull == false || listaAreProduse == false)
            {
                return false;
            }

            // 🔹 7️⃣ Dacă statusul coșului este incorect, îl corectăm și salvăm modificarea doar dacă este necesar
            if (cosDeCumparaturi.Status == StatusCosDeCumparaturi.CosFaraProduse)
            {
                cosDeCumparaturi.Status = StatusCosDeCumparaturi.CosCuProduse;
                await _nepalezBazaDate.SaveChangesAsync(); // ✅ Salvăm doar dacă statusul s-a schimbat
            }

            // 🔹 8️⃣ Salvăm coșul în Redis dacă a fost luat din SQL
            var cacheKey = $"{CachePrefix}{clientId}";
            await _cache.StringSetAsync(cacheKey, JsonConvert.SerializeObject(cosDeCumparaturi));

            return true;
        }



        public async Task<CosDeCumparaturi> ObtineCosDinRedisAsync(int clientId)
        {
            try
            {
                var cacheKey = $"{CachePrefix}{clientId}";
                var cacheRedis = await _cache.StringGetAsync(cacheKey);
                if (cacheRedis.IsNullOrEmpty || cacheRedis.HasValue == false)
                {
                    return null;
                }

                return JsonConvert.DeserializeObject<CosDeCumparaturi>(cacheRedis);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Eroare la obtinerea cosului din Redis pentru client {clientId}");
                return null;
            }
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



        public async Task<CosDeCumparaturi> AdaugaProdusInCos(int clientId, int produsId, int cantitate)
        {
            var cacheKey = $"{CachePrefix}{clientId}";

            using var tranzactie = await _nepalezBazaDate.Database.BeginTransactionAsync();
            try
            {
                CosDeCumparaturi cosDeCumparaturi;

                // 🔹 1️⃣ Verificăm dacă există produse în coș pentru acest client
                bool existaProduseInCos = await ExistaProduseInCosAsync(clientId);

                if (existaProduseInCos == false) // 🔹 Dacă nu există, creăm un coș nou și adăugăm produsul
                {
                    cosDeCumparaturi = await CreeazaCosNouAsync(clientId);
                    cosDeCumparaturi = await AdaugaProdusCareNuEInCosAsync(clientId, cosDeCumparaturi, produsId, cantitate);
                    cosDeCumparaturi.Status = StatusCosDeCumparaturi.CosCuProduse;
                }
                else
                {
                    // 🔹 2️⃣ Dacă există produse, încercăm să obținem coșul din cache (Redis) sau din baza de date
                    cosDeCumparaturi = await ObtineCosDinRedisAsync(clientId)
                                       ?? await ObtineCosDinSqlAsync(clientId); // 🔥 `??` pentru performanță

                    // 🔹 3️⃣ Verificăm dacă produsul este deja în coș
                    bool existaProdusInCos = await ExistaProdusInCosCumparaturi(produsId, cosDeCumparaturi);

                    if (existaProdusInCos == false) // 🔹 Dacă produsul nu există în coș, îl adăugăm
                    {
                        cosDeCumparaturi = await AdaugaProdusCareNuEInCosAsync(clientId, cosDeCumparaturi, produsId, cantitate);
                    }
                    else
                    {
                        // 🔹 4️⃣ Dacă produsul există deja în coș, modificăm doar cantitatea acestuia
                        cosDeCumparaturi = await ModificaCantitateProdusAsync(clientId, produsId, cantitate, cosDeCumparaturi);
                    }
                }

                // 🔹 5️⃣ Recalculăm totalul coșului
                cosDeCumparaturi.Total = await CalculeazaTotalAsync(cosDeCumparaturi) ?? 0m; // ✅ Folosim `?? 0m` pentru a evita erorile cu `null`

                // 🔹 6️⃣ Salvăm modificările în baza de date
                await _nepalezBazaDate.SaveChangesAsync();
                await tranzactie.CommitAsync(); // ✅ Confirmăm tranzacția doar dacă toate operațiunile au reușit

                // 🔹 7️⃣ Actualizăm coșul în cache (Redis) doar după succesul tranzacției
                await _cache.StringSetAsync(cacheKey, JsonConvert.SerializeObject(cosDeCumparaturi));

                return cosDeCumparaturi;
            }
            catch (Exception ex)
            {
                await tranzactie.RollbackAsync(); // 🚨 Dacă apare o eroare, anulăm modificările
                Console.WriteLine($"Eroare la adăugarea produsului în coș: {ex.Message}");
                throw;
            }
        }


        public async Task<bool> ExistaProdusInCosCumparaturi(int produsId, CosDeCumparaturi cosDeCumparaturi)
        {
            if (cosDeCumparaturi.ListaCantitatiProduseDinCosCumparaturi == null)
            {
                return false; // Dacă lista este null, produsul nu poate exista
            }

            return cosDeCumparaturi.ListaCantitatiProduseDinCosCumparaturi
                .Any(p => p.ProdusId == produsId);
        }

        public async Task<CosDeCumparaturi> CreeazaCosNouAsync(int clientId)
        {
            CosDeCumparaturi cosNouDeCumparaturi = new CosDeCumparaturi();
            cosNouDeCumparaturi.ClientId = clientId;

            await _nepalezBazaDate.CosuriDeCumparaturi.AddAsync(cosNouDeCumparaturi);
            await _nepalezBazaDate.SaveChangesAsync();
            return cosNouDeCumparaturi;
        }

        public async Task<decimal?> CalculeazaTotalAsync(CosDeCumparaturi cosCumparaturi)
        {
            // 🔹 1️⃣ Verificăm dacă coșul este valid
            if (cosCumparaturi == null)
            {
                throw new ArgumentNullException(nameof(cosCumparaturi), "Coșul de cumpărături nu poate fi null.");
            }

            // 🔹 2️⃣ Dacă coșul nu conține produse, returnăm `null`
            if (cosCumparaturi.Status == StatusCosDeCumparaturi.CosFaraProduse ||
                cosCumparaturi.ListaCantitatiProduseDinCosCumparaturi == null ||
                cosCumparaturi.ListaCantitatiProduseDinCosCumparaturi.Count == 0)
            {
                return null; // Coșul este resetat sau gol
            }

            // 🔹 3️⃣ Creăm un dicționar cu ID-urile produselor și cantitățile lor
            var dictionarProduseIDCantitati = cosCumparaturi.ListaCantitatiProduseDinCosCumparaturi
                .ToDictionary(produs => produs.ProdusId, produs => produs.Cantitate);

            // 🔹 4️⃣ Obținem produsele din baza de date
            var listaProduse = await _nepalezBazaDate.Produse
                .Where(produs => dictionarProduseIDCantitati.ContainsKey(produs.Id))
                .AsNoTracking()
                .ToListAsync();

            // 🔹 5️⃣ Calculăm totalul coșului
            decimal totalCos = 0m;
            foreach (var produs in listaProduse)
            {
                totalCos += produs.Pret * dictionarProduseIDCantitati[produs.Id];
            }

            return totalCos;
        }

        public async Task<CosDeCumparaturi> AdaugaProdusCareNuEInCosAsync(int clientId, CosDeCumparaturi cosDeCumparaturi, int produsId, int cantitate)
        {
            Produs produs = await ObtineProdusDinBd(produsId);
            cosDeCumparaturi.ListaCantitatiProduseDinCosCumparaturi.Add(new ListaCuCantitatileProduselorDinCosCumparaturi
            {
                CosDeCumparaturiId = cosDeCumparaturi.Id,
                ProdusId = produsId,
                Cantitate = cantitate
            });
            return cosDeCumparaturi;
        }

        public async Task<Produs> ObtineProdusDinBd (int produsId)
        {
            return await _nepalezBazaDate.Produse.FirstOrDefaultAsync(p => p.Id == produsId)
                ?? throw new KeyNotFoundException($"Produsul {produsId} nu a fost gasit");
        }


        public async Task<CosDeCumparaturi> ModificaCantitateProdusAsync(int clientId, int produsId, int cantitate, CosDeCumparaturi cosDeCumparaturi)
        {
            var produs = cosDeCumparaturi.ListaCantitatiProduseDinCosCumparaturi.First(
                p => p.ProdusId == produsId);
            produs.Cantitate += cantitate;

            if(produs.Cantitate <= 0)
            {
                cosDeCumparaturi.ListaCantitatiProduseDinCosCumparaturi.Remove(produs);
            }

            await _nepalezBazaDate.SaveChangesAsync();
            return cosDeCumparaturi;
        }


        public async Task<string> StergeCosDinRedisAsync(int clientId)
        {
            var cacheKey = $"{CachePrefix}{clientId}";

            // 🔹 Ștergem direct și verificăm dacă Redis a eliminat cheia
            bool sters = await _cache.KeyDeleteAsync(cacheKey);

            if (sters)
            {
                return $"✅ Coșul clientului {clientId} a fost șters din Redis.";
            }
            else
            {
                return $"❌ Coșul clientului {clientId} nu există în Redis.";
            }
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



        public async Task<CosDeCumparaturi> StergeProdusDinCosAsync(CosDeCumparaturi cosDeCumparaturi, int produsId)
        {
            if (cosDeCumparaturi == null)
            {
                throw new ArgumentNullException(nameof(cosDeCumparaturi), "❌ Coșul nu există.");
            }

            var produsDeSters = cosDeCumparaturi.ListaCantitatiProduseDinCosCumparaturi
                .FirstOrDefault(p => p.ProdusId == produsId);

            if (produsDeSters == null)
            {
                throw new InvalidOperationException($"❌ Produsul cu ID {produsId} nu există în coș.");
            }

            // 🔹 Eliminăm produsul din listă
            cosDeCumparaturi.ListaCantitatiProduseDinCosCumparaturi.Remove(produsDeSters);

            // 🔹 Recalculăm totalul coșului
            cosDeCumparaturi.Total = cosDeCumparaturi.ListaCantitatiProduseDinCosCumparaturi.Sum(p =>
                _nepalezBazaDate.Produse
                    .Where(pr => pr.Id == p.ProdusId)
                    .Select(pr => pr.Pret)
                    .FirstOrDefault() * p.Cantitate);

            // 🔹 Salvăm modificările în baza de date
            await _nepalezBazaDate.SaveChangesAsync();

            return cosDeCumparaturi;
        }
    }
}
