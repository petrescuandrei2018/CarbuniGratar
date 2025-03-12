using CarbuniGratar.Web.Models;

namespace CarbuniGratar.Web.Services
{
    public interface ICosService
    {
        Task<bool> ExistaProduseInCosAsync(int clientId);
        Task<bool> ExistaProdusInCosCumparaturiAsync(int produsId, CosDeCumparaturi cosDeCumparaturi);
        Task<decimal?> CalculeazaTotalAsync(CosDeCumparaturi cosCumparaturi);
        Task<CosDeCumparaturi> ModificaCantitateProdusAsync(int clientId, int produsId, int cantitate, CosDeCumparaturi cosDeCumparaturi);
        Task<CosDeCumparaturi> AdaugaProdusInCosAsync(int clientId, int produsId, int cantitate);

        Task<CosDeCumparaturi> AdaugaProdusCareNuEInCosAsync(int clientId, CosDeCumparaturi cosDeCumparaturi, int produsId, int cantitate);

        Task<CosDeCumparaturi> StergeProdusDinCosAsync(int clientId, int produsId, CosDeCumparaturi cosDeCumparaturi);

    }
}
