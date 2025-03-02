using CarbuniGratar.Web.Models;

namespace CarbuniGratar.Web.Repositories.CosRepository
{
    public interface ICosRepository
    {
        Task<List<ProdusCos>> ObtineCosBdAsync(int clientId);
        Task AdaugaInCosAsync(int clientId, int produsId, int cantitate);
        Task StergeCompletDinCosAsync(int clientId, int produsId);
        Task ActualizeazaCosAsync(int clientId, int produsId, int cantitate);
        Task GolesteCosDupaComandaAsync(int clientId);
    }

}
