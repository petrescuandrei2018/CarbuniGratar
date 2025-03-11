using CarbuniGratar.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarbuniGratar.Web.Repositories.CosRepository
{
    public interface ICosRepository
    {
        Task<bool> ExistaProduseInCosAsync(int clientId);

        Task<CosDeCumparaturi> ObtineCosDinRedisAsync(int clientId);

        Task<CosDeCumparaturi> ObtineCosDinSqlAsync(int clientId);

        Task<CosDeCumparaturi> AdaugaProdusInCos(int clientId, int produsId, int cantitate);

        Task<bool> ExistaProdusInCosCumparaturi(int produsId, CosDeCumparaturi cosDeCumparaturi);

        Task<CosDeCumparaturi> CreeazaCosNouAsync(int clientId);

        Task<decimal?> CalculeazaTotalAsync(CosDeCumparaturi cosCumparaturi);

        Task<CosDeCumparaturi> AdaugaProdusCareNuEInCosAsync(int clientId, CosDeCumparaturi cosDeCumparaturi, int produsId, int cantitate);

        Task<Produs> ObtineProdusDinBd(int produsId);

        Task<CosDeCumparaturi> ModificaCantitateProdusAsync(int clientId, int produsId, int cantitate, CosDeCumparaturi cosDeCumparaturi);


        Task<string> StergeCosDinRedisAsync(int clientId);


        Task<string> StergeCosDinSqlAsync(CosDeCumparaturi cosDeCumparaturi);


        Task<CosDeCumparaturi> StergeProdusDinCosAsync(CosDeCumparaturi cosDeCumparaturi, int produsId);
    }
}
