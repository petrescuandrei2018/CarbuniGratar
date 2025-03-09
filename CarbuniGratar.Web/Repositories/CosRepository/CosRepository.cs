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
            var cosDeCumparaturi = await ObtineCosDinRedisAsync(clientId);
            if(cosDeCumparaturi == null)
            {
                cosDeCumparaturi = await ObtineCosDinSqlAsync(clientId);
                if(cosDeCumparaturi == null)
                {
                    return false; // ❌ Nu există coș
                }

                else
                {
                    if (cosDeCumparaturi.Total > 0) // ✅ Dacă am găsit coș cu total pozitiv în SQL, îl salvăm în Redis
                    {     
                        if(cosDeCumparaturi.Status == StatusCosDeCumparaturi.CosFaraProduse)
                        {
                            cosDeCumparaturi.Status = StatusCosDeCumparaturi.CosCuProduse;
                        }
                        var cacheKey = $"{CachePrefix}{clientId}";
                        await _cache.StringSetAsync(cacheKey, JsonConvert.SerializeObject(cosDeCumparaturi));
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (cosDeCumparaturi.Total > 0)
                {
                    if (cosDeCumparaturi.Status == StatusCosDeCumparaturi.CosFaraProduse)
                    {
                        cosDeCumparaturi.Status = StatusCosDeCumparaturi.CosCuProduse;
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }



        public async Task<CosDeCumparaturi> ObtineCosDinRedisAsync(int clientId)
        {
            var cacheKey = $"{CachePrefix}{clientId}";
            var cacheRedis = await _cache.StringGetAsync(cacheKey);
            if (cacheRedis.IsNullOrEmpty)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<CosDeCumparaturi>(cacheRedis);
        }



        public async Task<CosDeCumparaturi> ObtineCosDinSqlAsync(int clientId)
        {
            return await _nepalezBazaDate.CosuriDeCumparaturi
                .FirstOrDefaultAsync(c => c.ClientId == clientId);
        }



        public async Task<CosDeCumparaturi> AdaugaProdusInCos(int clientId, int produsId, int cantitate)
        {
            var cacheKey = $"{CachePrefix}{clientId}";
            bool existaProduseInCos = await ExistaProduseInCosAsync(clientId);
            if (existaProduseInCos == false)
            {
                var cosDeCumparaturi = await CreeazaCosNouAsync(clientId);
                cosDeCumparaturi = await AdaugaProdusCareNuEInCosAsync(clientId, cosDeCumparaturi, produsId, cantitate);
                await _cache.StringSetAsync(cacheKey, cosDeCumparaturi.ProduseJson, TimeSpan.FromDays(30));
                cosDeCumparaturi.Status = StatusCosDeCumparaturi.CosCuProduse;
                return cosDeCumparaturi;
            }
            else
            {
                var cos = await _cache.StringGetAsync(cacheKey);
                var cosDeCumparaturi = await _nepalezBazaDate.CosuriDeCumparaturi.FirstOrDefaultAsync(cos => cos.ClientId == clientId);
                bool existaProdusInCosCumaparturi = await ExistaProdusInCosCumparaturi(produsId, cosDeCumparaturi);
                if (existaProdusInCosCumaparturi)
                {
                    cosDeCumparaturi = await ModificaCantitateProdusAsync(clientId, produsId, cantitate, cosDeCumparaturi);
                    if(cosDeCumparaturi.Total == 0)
                    {
                        cosDeCumparaturi.Status = StatusCosDeCumparaturi.CosFaraProduse;
                    }
                    return cosDeCumparaturi;
                }
                else
                {
                    cosDeCumparaturi = await AdaugaProdusCareNuEInCosAsync(clientId, cosDeCumparaturi, produsId, cantitate);
                    return cosDeCumparaturi;
                }
            }
        }

        public async Task<bool> ExistaProdusInCosCumparaturi (int produsId, CosDeCumparaturi cosDeCumparaturi)
        {
            var listaProduse = await ConvertesteProduseJsonLaListaProduse(cosDeCumparaturi.ProduseJson);
            if (listaProduse.FirstOrDefault(p => p.Id == produsId) != null)
            {
                return true;
            }
            else {  return false; } 
        }

        public async Task<CosDeCumparaturi> CreeazaCosNouAsync(int clientId)
        {
            CosDeCumparaturi cosNouDeCumparaturi = new CosDeCumparaturi();
            cosNouDeCumparaturi.ClientId = clientId;
            return cosNouDeCumparaturi;
        }



        public async Task<CosDeCumparaturi> AdaugaProdusCareNuEInCosAsync(int clientId, CosDeCumparaturi cosDeCumparaturi, int produsId, int cantitate)
        {
            Produs produs = await ObtineProdusDinBd(produsId);
            produs.CantitatePentruCosCumparaturi = cantitate;
            List<Produs> listaProduseDinCosulDeCumparturi = await ConvertesteProduseJsonLaListaProduse(cosDeCumparaturi.ProduseJson);
            listaProduseDinCosulDeCumparturi.Add(produs);
            cosDeCumparaturi.ProduseJson = JsonConvert.SerializeObject(listaProduseDinCosulDeCumparturi);
            cosDeCumparaturi.Total = cosDeCumparaturi.Total + cantitate * produs.Pret;
            
            return cosDeCumparaturi;
        }

        public async Task<Produs> ObtineProdusDinBd (int produsId)
        {
            var produs = await _nepalezBazaDate.Produse.FirstOrDefaultAsync(p => p.Id == produsId);
            return produs;
        }

        public async Task<List<Produs>> ConvertesteProduseJsonLaListaProduse (string produseJson)
        {
            List<Produs> listaProduse = JsonConvert.DeserializeObject<List<Produs>>(produseJson);
            return listaProduse;

        }


        public async Task<CosDeCumparaturi> ModificaCantitateProdusAsync(int clientId, int produsId, int cantitate, CosDeCumparaturi cosDeCumparaturi)
        {
            List<Produs> listaProduse = await ConvertesteProduseJsonLaListaProduse(cosDeCumparaturi.ProduseJson);
            Produs produs = listaProduse.FirstOrDefault(lp => lp.Id == produsId);
            produs.CantitatePentruCosCumparaturi += cantitate;
            cosDeCumparaturi.Total = cosDeCumparaturi.Total + cantitate * produs.Pret;
            if(produs.CantitatePentruCosCumparaturi <= 0)
            {
                listaProduse.Remove(produs);
            }
            cosDeCumparaturi.ProduseJson = JsonConvert.SerializeObject(listaProduse);
            return cosDeCumparaturi;
        }



        public async Task<string> SincronizeazaCosRedisCuSqlAsync(string codRedis)
        {
            // 🔹 Obținem coșurile din Redis și SQL
            string cosRedisJson = await _cache.StringGetAsync(codRedis);
            var cosDeCumparaturi = !string.IsNullOrEmpty(cosRedisJson)
                ? JsonConvert.DeserializeObject<CosDeCumparaturi>(cosRedisJson)
                : null;

            var cosSql = await _nepalezBazaDate.CosuriDeCumparaturi
                .FirstOrDefaultAsync(c => c.ClientId == cosDeCumparaturi.ClientId);

            // 🔹 Dacă există doar în Redis, salvăm în SQL
            if (cosDeCumparaturi != null && cosSql == null)
            {
                var nouCos = new CosDeCumparaturi
                {
                    ClientId = cosDeCumparaturi.ClientId,
                    DataCreare = cosDeCumparaturi.DataCreare,
                    Total = cosDeCumparaturi.Total,
                    Status = cosDeCumparaturi.Status,
                    ProduseJson = cosDeCumparaturi.ProduseJson
                };

                _nepalezBazaDate.CosuriDeCumparaturi.Add(nouCos);
                await _nepalezBazaDate.SaveChangesAsync();
                return $"✅ Coșul din Redis a fost salvat în SQL.";
            }

            // 🔹 Dacă există doar în SQL, salvăm în Redis
            if (cosDeCumparaturi == null && cosSql != null)
            {
                await _cache.StringSetAsync(codRedis, JsonConvert.SerializeObject(cosSql));
                return $"✅ Coșul din SQL a fost salvat în Redis.";
            }

            // 🔹 Dacă există în ambele, combinăm produsele
            if (cosDeCumparaturi != null && cosSql != null)
            {
                var produseRedis = JsonConvert.DeserializeObject<List<Produs>>(cosDeCumparaturi.ProduseJson);
                var produseSql = JsonConvert.DeserializeObject<List<Produs>>(cosSql.ProduseJson);

                foreach (var produs in produseRedis)
                {
                    var produsExistent = produseSql.FirstOrDefault(p => p.Id == produs.Id);
                    if (produsExistent != null)
                    {
                        produsExistent.CantitatePentruCosCumparaturi += produs.CantitatePentruCosCumparaturi;
                    }
                    else
                    {
                        produseSql.Add(produs);
                    }
                }

                // 🔹 Salvăm coșul actualizat în ambele locuri
                string cosFinalJson = JsonConvert.SerializeObject(new CosDeCumparaturi
                {
                    ClientId = cosSql.ClientId,
                    DataCreare = cosSql.DataCreare,
                    Total = produseSql.Sum(p => p.Pret * p.CantitatePentruCosCumparaturi),
                    Status = cosDeCumparaturi.Status,
                    ProduseJson = JsonConvert.SerializeObject(produseSql),
                });

                await _cache.StringSetAsync(codRedis, cosFinalJson);
                cosSql.ProduseJson = JsonConvert.SerializeObject(produseSql);
                cosSql.Total = produseSql.Sum(p => p.Pret * p.CantitatePentruCosCumparaturi);
                await _nepalezBazaDate.SaveChangesAsync();

                return $"✅ Coșurile din Redis și SQL au fost combinate și sincronizate.";
            }

            return $"⚠ Nu există date în Redis sau SQL pentru acest coș.";
        }



        public async Task<string> StergeCosDinRedisAsync(int clientId)
        {
            var cacheKey = $"{CachePrefix}{clientId}";
            bool exista = await _cache.KeyExistsAsync(cacheKey);
            if(exista)
            {
                await _cache.KeyDeleteAsync(cacheKey);
                return $"✅ Coșul clientului {clientId} a fost șters din Redis.";
            }
            else
            {
                return $"❌ Coșul pentru clientul {clientId} nu există în Redis."; 
            }
        }



        public async Task<string> StergeCosDinSqlAsync(CosDeCumparaturi cosDeCumparaturi)
        {
            _nepalezBazaDate.CosuriDeCumparaturi.Remove(cosDeCumparaturi);
            await _nepalezBazaDate.SaveChangesAsync();
            var mesaj = $"CosDeCumparaturi {cosDeCumparaturi} sters";
            return mesaj;
        }



        public async Task<CosDeCumparaturi> StergeProdusDinCosAsync(CosDeCumparaturi cosDeCumparaturi, int produsId)
        {
            var listaProduse = JsonConvert.DeserializeObject<List<Produs>>(cosDeCumparaturi.ProduseJson);
            var produsDeSters = listaProduse.FirstOrDefault(p => p.Id == produsId);

            if (produsDeSters == null)
            {
                throw new InvalidOperationException($"❌ Produsul cu ID {produsId} nu există în coș.");
            }

            listaProduse.Remove(produsDeSters);

            // 🔹 Actualizăm totalul coșului
            cosDeCumparaturi.Total = listaProduse.Sum(p => p.Pret * p.CantitatePentruCosCumparaturi);
            cosDeCumparaturi.ProduseJson = JsonConvert.SerializeObject(listaProduse);

            return cosDeCumparaturi;
        }
    }
}
