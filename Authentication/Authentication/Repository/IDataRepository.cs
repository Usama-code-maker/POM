using Authentication.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Authentication.Repository
{
    public interface IDataRepository<TEntity>
    {
        IEnumerable<TEntity> GetAll();
        TEntity Get(long id);
        void Add(TEntity entity);
        void Update(TEntity dbEntity, TEntity entity);
        void Delete(TEntity entity);
        Response Verification(Login login);
    }
}