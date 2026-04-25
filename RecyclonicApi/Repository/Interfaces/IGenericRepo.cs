namespace RecyclonicApi.Repository.Interfaces
{
    public interface IGenericRepo<T> where T : class
    {
        Task CreateAsync(T entity);
        void UpdateAsync(T entity);
        void DeleteAsync(T entity);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(Guid id);
        Task Save();
        Task CreateRangeAsync(IEnumerable<T> entities);
    }
}
