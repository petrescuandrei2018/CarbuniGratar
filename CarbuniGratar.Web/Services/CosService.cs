

using CarbuniGratar.Web.Data;
using CarbuniGratar.Web.Models;
using CarbuniGratar.Web.Repositories.CacheRepository;
using CarbuniGratar.Web.Repositories.CosRepository;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace CarbuniGratar.Web.Services
{
    public class CosService : ICosService
    {
        private readonly IDatabase _cacheRedis;
        private readonly ICosRepository _cosRepository;
        private readonly IDatabase _cache;
        private const string CachePrefix = "Cos_";
        private readonly ICacheRepository _cacheRepository;

        public CosService(IConnectionMultiplexer cacheRedis, IConnectionMultiplexer redis, ICacheRepository cacheRepository)
        {
            _cacheRedis = cacheRedis.GetDatabase();
            _cache = redis.GetDatabase();
            _cacheRepository = cacheRepository;
        }


        public async Task<bool> ExistaProduseInCosAsync(int clientId)
        {
            // 🔹 1️⃣ Încercăm să obținem coșul mai întâi din Redis
            var cosDeCumparaturi = await _cacheRepository.ObtineCosDinRedisAsync(clientId);

            // 🔹 2️⃣ Dacă coșul nu este în Redis, îl căutăm în SQL
            if (cosDeCumparaturi == null)
            {
                cosDeCumparaturi = await _cosRepository.ObtineCosDinSqlAsync(clientId);
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
                await _cosRepository.ActualizeazaCosAsync(cosDeCumparaturi); // ✅ Salvăm doar dacă statusul s-a schimbat
            }

            // 🔹 8️⃣ Salvăm coșul în Redis dacă a fost luat din SQL
            var cacheKey = $"{CachePrefix}{clientId}";
            await _cache.StringSetAsync(cacheKey, JsonConvert.SerializeObject(cosDeCumparaturi));

            return true;
        }



        public async Task<bool> ExistaProdusInCosCumparaturiAsync(int produsId, CosDeCumparaturi cosDeCumparaturi)
        {
            if (cosDeCumparaturi.ListaCantitatiProduseDinCosCumparaturi == null)
            {
                return false; // Dacă lista este null, produsul nu poate exista
            }

            return cosDeCumparaturi.ListaCantitatiProduseDinCosCumparaturi
                .Any(p => p.ProdusId == produsId);
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
            var listaIdProduse = dictionarProduseIDCantitati.Keys.ToList();
            var listaProduse = await _cosRepository.ObtineListaProduseDupaIdAsync(listaIdProduse);


            // 🔹 5️⃣ Calculăm totalul coșului
            decimal totalCos = 0m;
            foreach (var produs in listaProduse)
            {
                totalCos += produs.Pret * dictionarProduseIDCantitati[produs.Id];
            }

            return totalCos;
        }


        public async Task<CosDeCumparaturi> AdaugaProdusInCosAsync(int clientId, int produsId, int cantitate)
        {
            var cacheKey = $"{CachePrefix}{clientId}";

            if (cantitate <= 0)
            {
                throw new ArgumentException("❌ Cantitatea trebuie să fie mai mare decât 0.");
            }

            try
            {
                CosDeCumparaturi cosDeCumparaturi;

                // 🔹 1️⃣ Verificăm dacă există produse în coș pentru acest client
                bool existaProduseInCos = await ExistaProduseInCosAsync(clientId);

                if (existaProduseInCos == false) // 🔹 Dacă nu există, creăm un coș nou și adăugăm produsul
                {
                    cosDeCumparaturi = await _cosRepository.CreeazaCosNouAsync(clientId);
                    cosDeCumparaturi = await AdaugaProdusCareNuEInCosAsync(clientId, cosDeCumparaturi, produsId, cantitate);
                    cosDeCumparaturi.Status = StatusCosDeCumparaturi.CosCuProduse;
                }
                else
                {
                    // 🔹 2️⃣ Dacă există produse, încercăm să obținem coșul din cache (Redis) sau din baza de date
                    cosDeCumparaturi = await _cacheRepository.ObtineCosDinRedisAsync(clientId)
                                       ?? await _cosRepository.ObtineCosDinSqlAsync(clientId); // 🔥 `??` pentru performanță

                    // 🔹 3️⃣ Verificăm dacă produsul este deja în coș
                    bool existaProdusInCos = await ExistaProdusInCosCumparaturiAsync(produsId, cosDeCumparaturi);

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

                // 🔹 6️⃣ Salvăm modificările în baza de date (acum gestionate de `CosRepository`)
                cosDeCumparaturi = await _cosRepository.AdaugaCosNouSauActualizeazaCosSqlAsync(cosDeCumparaturi);

                // 🔹 7️⃣ Dacă salvarea în SQL a fost reușită, actualizăm și Redis
                await _cacheRepository.SalveazaCosInRedisAsync(clientId, cosDeCumparaturi);

                return cosDeCumparaturi;
            }
            catch (Exception ex)
            {
                await _cacheRepository.StergeCosDinRedisAsync(clientId); // 🔥 Ștergem Redis dacă SQL a eșuat
                Console.WriteLine($"Eroare la adăugarea produsului în coș: {ex.Message}");
                throw;
            }
        }



        public async Task<CosDeCumparaturi> ModificaCantitateProdusAsync(int clientId, int produsId, int cantitate, CosDeCumparaturi cosDeCumparaturi)
        {
            // 🔹 1️⃣ Verificăm dacă produsul există în coș
            var produs = cosDeCumparaturi.ListaCantitatiProduseDinCosCumparaturi
                .FirstOrDefault(p => p.ProdusId == produsId);

            if (produs == null)
            {
                throw new InvalidOperationException($"❌ Produsul cu ID {produsId} nu există în coș.");
            }

            // 🔹 2️⃣ Modificăm cantitatea produsului
            produs.Cantitate += cantitate;

            // 🔹 3️⃣ Dacă cantitatea ajunge la 0 sau mai puțin, eliminăm produsul din coș
            if (produs.Cantitate <= 0)
            {
                cosDeCumparaturi.ListaCantitatiProduseDinCosCumparaturi.Remove(produs);
            }

            // 🔹 4️⃣ Recalculăm totalul coșului
            cosDeCumparaturi.Total = await CalculeazaTotalAsync(cosDeCumparaturi) ?? 0m;

            // 🔹 5️⃣ Salvăm modificările în baza de date prin `CosRepository`
            await _cosRepository.AdaugaCosNouSauActualizeazaCosSqlAsync(cosDeCumparaturi);

            // 🔹 6️⃣ Sincronizăm Redis cu coșul actualizat
            await _cacheRepository.SalveazaCosInRedisAsync(clientId, cosDeCumparaturi);

            return cosDeCumparaturi;
        }


        public async Task<CosDeCumparaturi> AdaugaProdusCareNuEInCosAsync(int clientId, CosDeCumparaturi cosDeCumparaturi, int produsId, int cantitate)
        {
            Produs produs = await _cosRepository.ObtineProdusDinBdAsync(produsId);
            cosDeCumparaturi.ListaCantitatiProduseDinCosCumparaturi.Add(new ListaCuCantitatileProduselorDinCosCumparaturi
            {
                CosDeCumparaturiId = cosDeCumparaturi.Id,
                ProdusId = produsId,
                Cantitate = cantitate
            });
            return cosDeCumparaturi;
        }


        public async Task<CosDeCumparaturi> StergeProdusDinCosAsync(int clientId, int produsId, CosDeCumparaturi cosDeCumparaturi)
        {
            if (cosDeCumparaturi == null)
            {
                throw new ArgumentNullException(nameof(cosDeCumparaturi), "❌ Coșul nu există.");
            }

            // 🔹 1️⃣ Verificăm dacă produsul există în coș
            var produsDeSters = cosDeCumparaturi.ListaCantitatiProduseDinCosCumparaturi
                .FirstOrDefault(p => p.ProdusId == produsId);

            if (produsDeSters == null)
            {
                throw new InvalidOperationException($"❌ Produsul cu ID {produsId} nu există în coș.");
            }

            // 🔹 2️⃣ Eliminăm produsul din listă
            cosDeCumparaturi.ListaCantitatiProduseDinCosCumparaturi.Remove(produsDeSters);

            // 🔹 3️⃣ Recalculăm totalul coșului
            cosDeCumparaturi.Total = await CalculeazaTotalAsync(cosDeCumparaturi) ?? 0m;

            // 🔹 4️⃣ Salvăm modificările în baza de date prin `CosRepository`
            await _cosRepository.AdaugaCosNouSauActualizeazaCosSqlAsync(cosDeCumparaturi);

            // 🔹 5️⃣ Sincronizăm Redis cu coșul actualizat
            await _cacheRepository.SalveazaCosInRedisAsync(clientId, cosDeCumparaturi);

            return cosDeCumparaturi;
        }

    }
}
