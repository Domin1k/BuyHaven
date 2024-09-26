namespace BuyHaven.Common.Services
{
    using BuyHaven.Common.Data.Models;

    public interface IDataService<in TEntity>
        where TEntity : class
    {
        Task MarkMessageAsPublished(int id);

        Task Save(TEntity entity, params Message[] messages);
    }
}
