using System.Collections.Generic;
using System.Linq;
using DentalClinic.Models;

namespace DentalClinic.Data
{
    public class DentalRecordRepository : IRepository<DentalRecord>
    {
        private readonly List<DentalRecord> _records = new List<DentalRecord>();
        private int _nextId = 1;

        public void Add(DentalRecord record)
        {
            record.Id = _nextId++;
            _records.Add(record);
        }

        public DentalRecord GetById(int id) =>
            _records.FirstOrDefault(r => r.Id == id);

        public List<DentalRecord> GetAll() => new List<DentalRecord>(_records);

        public void Update(DentalRecord updated)
        {
            var index = _records.FindIndex(r => r.Id == updated.Id);
            if (index >= 0)
                _records[index] = updated;
        }

        public void Delete(int id) =>
            _records.RemoveAll(r => r.Id == id);

        // Всі записи конкретного пацієнта
        public List<DentalRecord> GetByPatient(int patientId) =>
            _records.Where(r => r.PatientId == patientId).ToList();

        // Стан конкретного зуба пацієнта (останній запис)
        public DentalRecord GetToothRecord(int patientId, int toothNumber) =>
            _records
                .Where(r => r.PatientId == patientId && r.ToothNumber == toothNumber)
                .OrderByDescending(r => r.RecordDate)
                .FirstOrDefault();
    }
}
