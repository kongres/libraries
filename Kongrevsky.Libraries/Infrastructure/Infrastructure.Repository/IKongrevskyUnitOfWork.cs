namespace Kongrevsky.Infrastructure.Repository
{
    using System.Data.Entity;
    using System.Threading.Tasks;

    public interface IKongrevskyUnitOfWork<T> where T : KongrevskyDbContext
    {
        void Commit();

        Task CommitAsync();
    }
}