

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

        public Task ActualizeazaCacheRedis(int clientId)
        {
            throw new NotImplementedException();
        }
    }
}
