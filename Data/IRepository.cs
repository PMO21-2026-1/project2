using System.Collections.Generic;

namespace DentalClinic.Data
{
    public interface IRepository<T>
    {
        void Add(T entity);
        T GetById(int id);
        List<T> GetAll();
        void Update(T entity);
        void Delete(int id);
    }
}
