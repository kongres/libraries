namespace Infrastructure.Repository
{
    using System.Data.Entity;
    using System.Threading.Tasks;

    public interface IUnitOfWork<T> where T : DbContext
    {
        void Commit();

        Task CommitAsync();
    }
}