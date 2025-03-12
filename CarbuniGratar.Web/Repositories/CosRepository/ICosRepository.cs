using CarbuniGratar.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarbuniGratar.Web.Repositories.CosRepository
{
    public interface ICosRepository
    {
        Task<CosDeCumparaturi> ObtineCosDinSqlAsync(int clientId);


        Task<CosDeCumparaturi> CreeazaCosNouAsync(int clientId);

        Task<List<Produs>> ObtineListaProduseDupaIdAsync(List<int> listaProduse);

        Task<Produs> ObtineProdusDinBdAsync(int produsId);

        Task ActualizeazaCosAsync(CosDeCumparaturi cosDeCumparaturi);

        Task<CosDeCumparaturi> AdaugaCosNouSauActualizeazaCosSqlAsync(CosDeCumparaturi cosDeCumparaturi);

        Task<string> StergeCosDinSqlAsync(CosDeCumparaturi cosDeCumparaturi);


    }
}
