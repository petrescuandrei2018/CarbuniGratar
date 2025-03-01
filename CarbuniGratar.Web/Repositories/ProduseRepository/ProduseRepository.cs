using CarbuniGratar.Web.Data;
using CarbuniGratar.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace CarbuniGratar.Web.Repositories.ProduseRepository
{
    public class ProduseRepository : IProduseRepository
    {
        private readonly NepalezBazaDate _nepalezBazaDate;
        public ProduseRepository(NepalezBazaDate nepalezBazaDate)
        {
            _nepalezBazaDate = nepalezBazaDate;
        }

        public async Task<List<Produs>> ObtineToateProdusele()
        {
            return await _nepalezBazaDate.Produse.ToListAsync();
        }
    }
}
