using System;
using System.Collections.Generic;
using System.Linq;
using DentalClinic.Models;

namespace DentalClinic.Data
{
    public class PatientRepository : IRepository<Patient>
    {
        private readonly List<Patient> _patients = new List<Patient>();
        private int _nextId = 1;

        public void Add(Patient patient)
        {
            patient.Id = _nextId++;
            _patients.Add(patient);
        }

        public Patient GetById(int id) =>
            _patients.FirstOrDefault(p => p.Id == id);

        public List<Patient> GetAll() => new List<Patient>(_patients);

        public void Update(Patient updated)
        {
            var index = _patients.FindIndex(p => p.Id == updated.Id);
            if (index >= 0)
                _patients[index] = updated;
        }

        public void Delete(int id) =>
            _patients.RemoveAll(p => p.Id == id);

        // Пошук за ПІБ або телефоном
        public List<Patient> Search(string query)
        {
            query = query.ToLower();
            return _patients
                .Where(p => p.FullName.ToLower().Contains(query) ||
                            p.Phone.Contains(query))
                .ToList();
        }
    }
}
