namespace CarbuniGratar.Web.Services
{
    public interface ICosService
    {
        Task ActualizeazaCacheRedis(int clientId);
    }
}
