using System.Collections.Generic;
using System.Linq;
using DentalClinic.Models;

namespace DentalClinic.Data
{
    public class ServiceRepository : IRepository<Service>
    {
        private readonly List<Service> _services = new List<Service>();
        private int _nextId = 1;

        public void Add(Service service)
        {
            service.Id = _nextId++;
            _services.Add(service);
        }

        public Service GetById(int id) =>
            _services.FirstOrDefault(s => s.Id == id);

        public List<Service> GetAll() => new List<Service>(_services);

        public void Update(Service updated)
        {
            var index = _services.FindIndex(s => s.Id == updated.Id);
            if (index >= 0)
                _services[index] = updated;
        }

        public void Delete(int id) =>
            _services.RemoveAll(s => s.Id == id);

        public List<Service> GetByIds(List<int> ids) =>
            _services.Where(s => ids.Contains(s.Id)).ToList();
    }
}
