using CarbuniGratar.Web.Data;
using CarbuniGratar.Web.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;

namespace CarbuniGratar.Web.Repositories.CosRepository
{
    public class CosRepository : ICosRepository
    {
        private readonly NepalezBazaDate _nepalezBazaDate;
        private readonly IDistributedCache _cache; //Redis cache setat o singură dată, incapsulat și nu poate fi modificat ulterior
                                                   //persistenta si acces rapid la informații fără a interoga direct baza de date de fiecare data
        private const string CachePrefix = "Cos_";

        public CosRepository(NepalezBazaDate nepalezBazaDate, IDistributedCache cache)
        {
            _nepalezBazaDate = nepalezBazaDate;
            _cache = cache;

        }
        public Task ActualizeazaCosAsync(int clientId, int produsId, int cantitate)
        {
            throw new NotImplementedException();
        }

        public Task AdaugaInCosAsync(int clientId, int produsId, int cantitate)
        {
            throw new NotImplementedException();
        }

        public Task GolesteCosDupaComandaAsync(int clientId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<ProdusCos>> ObtineCosBdAsync(int clientId)
        {
            var cacheKey = $"{CachePrefix}{clientId}"; // cheie unică pentru cache-ul coșului de cumpărături al unui anumit client

            // 1️⃣ Încercăm să luăm datele din Redis
            var cosJson = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cosJson))
            {
                return JsonConvert.DeserializeObject<List<ProdusCos>>(cosJson);
            }

            // 2️⃣ Dacă nu există în Redis, luăm datele din SQL Server cu un JOIN
            var cosProduse = await (from comandaProdus in _nepalezBazaDate.ComenziProduse
                                    join produs in _nepalezBazaDate.Produse
                                    on comandaProdus.ProdusId equals produs.Id
                                    join comanda in _nepalezBazaDate.Comenzi
                                    on comandaProdus.ComandaId equals comanda.Id
                                    where comanda.ClientId == clientId && comanda.Status == "In cos"
                                    select new ProdusCos
                                    {
                                        ProdusId = produs.Id,
                                        NumeProdus = produs.Nume,
                                        Cantitate = comandaProdus.Cantitate,
                                        Pret = produs.Pret,
                                        ImageUrl = produs.ImagineUrl
                                    }).ToListAsync();


            if (cosProduse.Any())
            {
                await _cache.SetStringAsync(cacheKey, JsonConvert.SerializeObject(cosProduse), new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
                });
            }

            return cosProduse;
        }

        public Task StergeCompletDinCosAsync(int clientId, int produsId)
        {
            throw new NotImplementedException();
        }
    }
}
