using System;
using System.Collections.Generic;
using System.Linq;
using DentalClinic.Models;

namespace DentalClinic.Data
{
    public class VisitRepository : IRepository<Visit>
    {
        private readonly List<Visit> _visits = new List<Visit>();
        private int _nextId = 1;

        public void Add(Visit visit)
        {
            visit.Id = _nextId++;
            _visits.Add(visit);
        }

        public Visit GetById(int id) =>
            _visits.FirstOrDefault(v => v.Id == id);

        public List<Visit> GetAll() => new List<Visit>(_visits);

        public void Update(Visit updated)
        {
            var index = _visits.FindIndex(v => v.Id == updated.Id);
            if (index >= 0)
                _visits[index] = updated;
        }

        public void Delete(int id) =>
            _visits.RemoveAll(v => v.Id == id);

        public List<Visit> GetByPatient(int patientId) =>
            _visits.Where(v => v.PatientId == patientId).ToList();

        public List<Visit> GetByDoctor(int doctorId) =>
            _visits.Where(v => v.DoctorId == doctorId).ToList();

        public List<Visit> GetByDate(DateTime date) =>
            _visits.Where(v => v.StartDateTime.Date == date.Date).ToList();

        // Повертає всі НЕ скасовані візити лікаря в заданий день
        public List<Visit> GetActiveDoctorVisitsOnDay(int doctorId, DateTime date) =>
            _visits.Where(v =>
                v.DoctorId == doctorId &&
                v.StartDateTime.Date == date.Date &&
                v.Status != VisitStatus.Canceled)
            .ToList();

        public List<Visit> GetByMonth(int year, int month) =>
            _visits.Where(v =>
                v.StartDateTime.Year == year &&
                v.StartDateTime.Month == month)
            .ToList();
    }
}
