using System;
using System.Collections.Generic;
using System.Linq;
using DentalClinic.Data;
using DentalClinic.Models;

namespace DentalClinic.Services
{
    public class DentalRecordService
    {
        private readonly DentalRecordRepository _recordRepo;
        private readonly PatientRepository _patientRepo;

        public DentalRecordService(
            DentalRecordRepository recordRepo,
            PatientRepository patientRepo)
        {
            _recordRepo = recordRepo;
            _patientRepo = patientRepo;
        }

        // Встановити або оновити стан зуба
        public (bool Success, string Message) SetToothCondition(
            int patientId,
            int toothNumber,
            ToothCondition condition,
            string workDescription = "")
        {
            if (_patientRepo.GetById(patientId) == null)
                return (false, "Пацієнта не знайдено.");

            if (toothNumber < 1 || toothNumber > 32)
                return (false, "Номер зуба має бути від 1 до 32.");

            var record = new DentalRecord
            {
                PatientId = patientId,
                ToothNumber = toothNumber,
                Condition = condition,
                WorkDescription = workDescription,
                RecordDate = DateTime.Today
            };

            _recordRepo.Add(record);
            return (true, $"Зуб #{toothNumber}: стан оновлено ({condition}).");
        }

        // Повна зубна формула пацієнта (32 зуби)
        public void PrintToothFormula(int patientId)
        {
            var patient = _patientRepo.GetById(patientId);
            if (patient == null)
            {
                Console.WriteLine("Пацієнта не знайдено.");
                return;
            }

            Console.WriteLine($"\n  Зубна формула: {patient.FullName}");
            Console.WriteLine("  ─────────────────────────────────────────────");

            // Верхня щелепа: зуби 1-16 (права сторона 1-8, ліва 9-16)
            Console.Write("  Верх: ");
            for (int i = 1; i <= 16; i++)
            {
                var rec = _recordRepo.GetToothRecord(patientId, i);
                Console.Write($"[{i}:{GetSymbol(rec?.Condition)}]");
            }
            Console.WriteLine();

            // Нижня щелепа: зуби 17-32
            Console.Write("  Низ:  ");
            for (int i = 17; i <= 32; i++)
            {
                var rec = _recordRepo.GetToothRecord(patientId, i);
                Console.Write($"[{i}:{GetSymbol(rec?.Condition)}]");
            }
            Console.WriteLine();

            Console.WriteLine("  Позначки: ✓=здоровий  K=карієс  П=пломба  K=коронка  X=відсутній  ~=лікується");
            Console.WriteLine();
        }

        private string GetSymbol(ToothCondition? condition)
        {
            return condition switch
            {
                null => "✓",
                ToothCondition.Healthy => "✓",
                ToothCondition.Caries => "K",
                ToothCondition.Filling => "П",
                ToothCondition.Crown => "Кр",
                ToothCondition.Missing => "X",
                ToothCondition.UnderTreatment => "~",
                _ => "?"
            };
        }

        // Повна історія лікування зуба
        public List<DentalRecord> GetToothHistory(int patientId, int toothNumber) =>
            _recordRepo.GetByPatient(patientId)
                .Where(r => r.ToothNumber == toothNumber)
                .OrderByDescending(r => r.RecordDate)
                .ToList();
    }
}
