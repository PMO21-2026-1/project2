using System.Collections.Generic;
using System.Linq;
using DentalClinic.Models;

namespace DentalClinic.Data
{
    public class DoctorRepository : IRepository<Doctor>
    {
        private readonly List<Doctor> _doctors = new List<Doctor>();
        private int _nextId = 1;

        public void Add(Doctor doctor)
        {
            doctor.Id = _nextId++;
            _doctors.Add(doctor);
        }

        public Doctor GetById(int id) =>
            _doctors.FirstOrDefault(d => d.Id == id);

        public List<Doctor> GetAll() => new List<Doctor>(_doctors);

        public void Update(Doctor updated)
        {
            var index = _doctors.FindIndex(d => d.Id == updated.Id);
            if (index >= 0)
                _doctors[index] = updated;
        }

        public void Delete(int id) =>
            _doctors.RemoveAll(d => d.Id == id);

        // Тільки ті, хто зараз працює
        public List<Doctor> GetWorkingDoctors() =>
            _doctors.Where(d => d.Status == DoctorStatus.Working).ToList();
    }
}
