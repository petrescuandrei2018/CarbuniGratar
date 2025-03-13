using CarbuniGratar.Web.Models;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace CarbuniGratar.Web.Repositories.CacheRepository
{
    public class CacheRepository
    {
        private readonly IDatabase _cache;
        private const string CachePrefix = "Cos_";


        public CacheRepository(IConnectionMultiplexer redis)
        {
            _cache = redis.GetDatabase();
        }


        public async Task SalveazaCosInRedisAsync(int clientId,CosDeCumparaturi cos)
        {
            var cacheKey = $"{CachePrefix}{clientId}";
            await _cache.StringSetAsync(cacheKey, JsonConvert.SerializeObject(cos));
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


        public async Task<CosDeCumparaturi?> ObtineCosDinRedisAsync(string clientAnonimId)
        {
            var cosJson = await _cache.StringGetAsync($"Cos_{clientAnonimId}");
            return cosJson.IsNullOrEmpty ? null : JsonConvert.DeserializeObject<CosDeCumparaturi>(cosJson);
        }


        public async Task SalveazaCosInRedisAsync(string cacheKey, string cosJson)
        {
            await _cache.StringSetAsync(cacheKey, cosJson);
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

    }
}
