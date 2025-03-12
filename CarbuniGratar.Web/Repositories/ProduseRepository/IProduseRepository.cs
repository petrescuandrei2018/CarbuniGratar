using CarbuniGratar.Web.Models;

namespace CarbuniGratar.Web.Repositories.ProduseRepository
{
    public interface IProduseRepository
    {
        Task<List<Produs>> ObtineToateProduseleAsync();
    }
}
