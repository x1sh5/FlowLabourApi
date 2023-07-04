using System.Linq.Expressions;

namespace FlowLabourApi.Models.Services
{
    public interface IDbService<T> where T : class
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        void Add(T entity);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        Task AddAsync(T entity);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        void Update(T entity);

        Task UpdateAsync(T entity);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        void Delete(int id);

        Task<T> DeleteAsync(int id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="where"></param>
        //void Delete(Expression<Func<T, bool>> where);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T GetById(int id);

        Task<T> GetByIdAsync(int id);

        T GetByName(string Name);

        Task<T> GetByNameAsync(string Name);
    }
}
