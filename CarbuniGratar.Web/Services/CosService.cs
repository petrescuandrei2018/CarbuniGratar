

using CarbuniGratar.Web.Data;
using CarbuniGratar.Web.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace CarbuniGratar.Web.Services
{
    public class CosService : ICosService
    {
        private readonly NepalezBazaDate _nepalezBazaDate;
        private readonly IDatabase _cacheRedis;

        public CosService(NepalezBazaDate nepalezBazaDate, IConnectionMultiplexer cacheRedis)
        {
            _nepalezBazaDate = nepalezBazaDate;
            _cacheRedis = cacheRedis.GetDatabase();
        }

        public async Task ActualizeazaCacheRedis(int clientId)
        {
            var produseInCos = await _nepalezBazaDate.Produse
                .Join(_nepalezBazaDate.ComenziProduse,
                p => p.Id,
                cp => cp.ProdusId,
                (p, cp) => new { p, cp })
                .Where(x => x.cp.Comanda.ClientId == clientId)
                .Select(x => new ProdusCos
                {
                    ClientId = x.cp.Comanda.ClientId,
                    Cantitate = x.cp.Cantitate,
                    ProdusId = x.cp.ProdusId,
                    NumeProdus = x.p.Nume,
                    ImageUrl = x.p.ImagineUrl,
                    Pret = x.p.Pret,

                }).ToListAsync();

            await _cacheRedis.StringSetAsync($"cos:{clientId}", JsonConvert.SerializeObject(produseInCos));

            throw new NotImplementedException();
        }
    }
}
