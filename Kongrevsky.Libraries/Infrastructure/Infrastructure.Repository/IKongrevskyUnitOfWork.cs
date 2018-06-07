namespace Kongrevsky.Infrastructure.Repository
{
    #region << Using >>

    using System.Threading.Tasks;

    #endregion

    public interface IKongrevskyUnitOfWork<T> where T : KongrevskyDbContext
    {
        void Commit(bool fireTriggers = true);

        Task CommitAsync(bool fireTriggers = true);
    }
}