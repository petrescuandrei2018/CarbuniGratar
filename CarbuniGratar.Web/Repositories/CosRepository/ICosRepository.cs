using CarbuniGratar.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarbuniGratar.Web.Repositories.CosRepository
{
    public interface ICosRepository
    {
        // ✅ Obținerea Coșului
        Task<CosDeCumparaturi> ObtineCosDinRedisAsync(int clientId);
        Task<CosDeCumparaturi> ObtineCosDinSqlAsync(int clientId);
        Task<bool> ExistaProduseInCosAsync(int clientId);

        // 🟡 Adăugare și Modificare Produse
        Task<CosDeCumparaturi> AdaugaProdusInCos(int clientId, int produsId, int cantitate);
        Task<CosDeCumparaturi> CreeazaCosNouAsync(int clientId);
        Task<CosDeCumparaturi> AdaugaProdusCareNuEInCosAsync(int clientId, CosDeCumparaturi cosDeCumparaturi, int produsId, int cantitate);
        Task<CosDeCumparaturi> ModificaCantitateProdusAsync(int clientId, int produsId, int cantitate, CosDeCumparaturi cosDeCumparaturi);
        Task StergeProdusDinCosAsync(int clientId, int produsId);
        Task<bool> ExistaProdusInCosCumparaturi(int produsId, CosDeCumparaturi cosDeCumparaturi);

        // 🔴 Ștergere și Sincronizare
        Task StergeCosDinRedisAsync(int clientId);
        Task StergeCosDinSqlAsync(int clientId);
        Task SincronizeazaCosRedisCuSqlAsync(int clientId);
    }
}
