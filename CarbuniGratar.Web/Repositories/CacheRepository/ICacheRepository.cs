using CarbuniGratar.Web.Models;

namespace CarbuniGratar.Web.Repositories.CacheRepository
{
    public interface ICacheRepository
    {
        Task SalveazaCosInRedisAsync(int clientId, CosDeCumparaturi cos);

        Task<CosDeCumparaturi> ObtineCosDinRedisAsync(int clientId);
        Task<CosDeCumparaturi> ObtineCosDinRedisAsync(string clientAnonimId);

        Task SalveazaCosInRedisAsync(string cacheKey, string cosJson);

        Task<string> StergeCosDinRedisAsync(int clientId);
    }
}
