using System;
using System.Collections.Generic;
using DentalClinic.Data;
using DentalClinic.Models;

namespace DentalClinic.Services
{
    public class VisitService
    {
        private readonly VisitRepository _visitRepo;
        private readonly DoctorRepository _doctorRepo;
        private readonly PatientRepository _patientRepo;
        private readonly ServiceRepository _serviceRepo;

        public VisitService(
            VisitRepository visitRepo,
            DoctorRepository doctorRepo,
            PatientRepository patientRepo,
            ServiceRepository serviceRepo)
        {
            _visitRepo = visitRepo;
            _doctorRepo = doctorRepo;
            _patientRepo = patientRepo;
            _serviceRepo = serviceRepo;
        }

        // Записати пацієнта на прийом
        public (bool Success, string Message, Visit Visit) BookVisit(
            int patientId,
            int doctorId,
            List<int> serviceIds,
            DateTime startDateTime,
            int durationMinutes)
        {
            // 1. Перевірка чи пацієнт існує
            var patient = _patientRepo.GetById(patientId);
            if (patient == null)
                return (false, "Пацієнта не знайдено.", null);

            // 2. Перевірка чи лікар існує і доступний
            var doctor = _doctorRepo.GetById(doctorId);
            if (doctor == null)
                return (false, "Лікаря не знайдено.", null);

            if (doctor.Status != DoctorStatus.Working)
                return (false, $"Лікар {doctor.FullName} зараз недоступний ({doctor.Status}).", null);

            // 3. Перевірка на конфлікт у розкладі лікаря
            var conflict = HasScheduleConflict(doctorId, startDateTime, durationMinutes);
            if (conflict)
                return (false, "Цей лікар вже має запис на вказаний час.", null);

            // 4. Перевірка послуг
            if (serviceIds == null || serviceIds.Count == 0)
                return (false, "Потрібно обрати хоча б одну послугу.", null);

            // 5. Створення візиту
            var visit = new Visit
            {
                PatientId = patientId,
                DoctorId = doctorId,
                ServiceIds = serviceIds,
                StartDateTime = startDateTime,
                DurationMinutes = durationMinutes,
                Status = VisitStatus.Planned
            };

            _visitRepo.Add(visit);
            return (true, "Запис успішно створено.", visit);
        }

        // Скасувати візит
        public (bool Success, string Message) CancelVisit(int visitId)
        {
            var visit = _visitRepo.GetById(visitId);
            if (visit == null)
                return (false, "Візит не знайдено.");

            if (visit.Status == VisitStatus.Completed)
                return (false, "Не можна скасувати завершений візит.");

            visit.Status = VisitStatus.Canceled;
            _visitRepo.Update(visit);
            return (true, "Візит скасовано.");
        }

        // Завершити візит
        public (bool Success, string Message) CompleteVisit(int visitId)
        {
            var visit = _visitRepo.GetById(visitId);
            if (visit == null)
                return (false, "Візит не знайдено.");

            if (visit.Status == VisitStatus.Canceled)
                return (false, "Цей візит скасовано.");

            visit.Status = VisitStatus.Completed;
            _visitRepo.Update(visit);
            return (true, "Візит завершено.");
        }

        // Перенести візит
        public (bool Success, string Message) RescheduleVisit(int visitId, DateTime newDateTime)
        {
            var visit = _visitRepo.GetById(visitId);
            if (visit == null)
                return (false, "Візит не знайдено.");

            if (visit.Status == VisitStatus.Completed || visit.Status == VisitStatus.Canceled)
                return (false, "Не можна перенести завершений або скасований візит.");

            var conflict = HasScheduleConflict(visit.DoctorId, newDateTime, visit.DurationMinutes, excludeVisitId: visitId);
            if (conflict)
                return (false, "Лікар вже зайнятий у цей час.");

            visit.StartDateTime = newDateTime;
            visit.Status = VisitStatus.Rescheduled;
            _visitRepo.Update(visit);
            return (true, $"Візит перенесено на {newDateTime:dd.MM.yyyy HH:mm}.");
        }

        // Розклад лікаря на день
        public List<Visit> GetDoctorScheduleForDay(int doctorId, DateTime date) =>
            _visitRepo.GetActiveDoctorVisitsOnDay(doctorId, date);

        // Перевірка конфлікту часу
        private bool HasScheduleConflict(
            int doctorId,
            DateTime start,
            int durationMinutes,
            int excludeVisitId = -1)
        {
            var end = start.AddMinutes(durationMinutes);
            var existing = _visitRepo.GetActiveDoctorVisitsOnDay(doctorId, start);

            foreach (var v in existing)
            {
                if (v.Id == excludeVisitId) continue;

                // Перевірка перетину інтервалів
                if (start < v.EndDateTime && end > v.StartDateTime)
                    return true;
            }
            return false;
        }
    }
}
