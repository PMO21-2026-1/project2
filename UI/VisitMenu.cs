using System;
using System.Collections.Generic;
using DentalClinic.Data;
using DentalClinic.Models;
using DentalClinic.Services;

namespace DentalClinic.UI
{
    public class VisitMenu
    {
        private readonly VisitService _visitService;
        private readonly VisitRepository _visitRepo;
        private readonly DoctorRepository _doctorRepo;
        private readonly PatientRepository _patientRepo;
        private readonly ServiceRepository _serviceRepo;

        public VisitMenu(
            VisitService visitService,
            VisitRepository visitRepo,
            DoctorRepository doctorRepo,
            PatientRepository patientRepo,
            ServiceRepository serviceRepo)
        {
            _visitService = visitService;
            _visitRepo = visitRepo;
            _doctorRepo = doctorRepo;
            _patientRepo = patientRepo;
            _serviceRepo = serviceRepo;
        }

        public void Show()
        {
            bool running = true;
            while (running)
            {
                ConsoleHelper.PrintHeader("ЗАПИСИ / ВІЗИТИ");
                Console.WriteLine("  1. Записати пацієнта");
                Console.WriteLine("  2. Розклад на день");
                Console.WriteLine("  3. Завершити візит");
                Console.WriteLine("  4. Скасувати візит");
                Console.WriteLine("  5. Перенести візит");
                Console.WriteLine("  6. Історія пацієнта");
                Console.WriteLine("  0. Назад");

                var choice = ConsoleHelper.ReadInt("Вибір");
                switch (choice)
                {
                    case 1: BookVisit(); break;
                    case 2: ShowSchedule(); break;
                    case 3: CompleteVisit(); break;
                    case 4: CancelVisit(); break;
                    case 5: RescheduleVisit(); break;
                    case 6: PatientHistory(); break;
                    case 0: running = false; break;
                    default: ConsoleHelper.PrintError("Невірний вибір."); break;
                }
            }
        }

        private void BookVisit()
        {
            ConsoleHelper.PrintHeader("НОВИЙ ЗАПИС");

            // Вибір пацієнта
            var query = ConsoleHelper.ReadLine("Пошук пацієнта (ПІБ/телефон)");
            var patients = _patientRepo.Search(query);
            if (patients.Count == 0) { ConsoleHelper.PrintError("Пацієнта не знайдено."); ConsoleHelper.Pause(); return; }
            foreach (var p in patients)
                ConsoleHelper.PrintInfo($"[{p.Id}] {p.FullName}  тел: {p.Phone}");
            var patientId = ConsoleHelper.ReadInt("ID пацієнта");

            // Вибір лікаря
            var doctors = _doctorRepo.GetWorkingDoctors();
            ConsoleHelper.PrintInfo("Доступні лікарі:");
            foreach (var d in doctors)
                ConsoleHelper.PrintInfo($"[{d.Id}] {d.FullName}  ({d.Specialization})");
            var doctorId = ConsoleHelper.ReadInt("ID лікаря");

            // Вибір послуг
            var services = _serviceRepo.GetAll();
            ConsoleHelper.PrintInfo("Доступні послуги:");
            foreach (var s in services)
                ConsoleHelper.PrintInfo($"[{s.Id}] {s.Name}  —  {s.Price:0.00} грн  ({s.DefaultDurationMinutes} хв)");

            var serviceIds = new List<int>();
            ConsoleHelper.PrintInfo("Введіть ID послуг через кому (напр. 1,2,3):");
            var serviceInput = ConsoleHelper.ReadLine("Послуги");
            foreach (var part in serviceInput.Split(','))
                if (int.TryParse(part.Trim(), out int sid))
                    serviceIds.Add(sid);

            // Дата і час
            var dateStr = ConsoleHelper.ReadLine("Дата (дд.мм.рррр)");
            var timeStr = ConsoleHelper.ReadLine("Час (гг:хх)");
            if (!DateTime.TryParseExact($"{dateStr} {timeStr}", "dd.MM.yyyy HH:mm", null,
                    System.Globalization.DateTimeStyles.None, out var startDt))
            {
                ConsoleHelper.PrintError("Невірний формат дати або часу.");
                ConsoleHelper.Pause();
                return;
            }

            var duration = ConsoleHelper.ReadInt("Тривалість (хвилин)", 30);

            var (success, message, visit) = _visitService.BookVisit(patientId, doctorId, serviceIds, startDt, duration);
            if (success)
                ConsoleHelper.PrintSuccess($"{message} (ID візиту: {visit.Id})");
            else
                ConsoleHelper.PrintError(message);

            ConsoleHelper.Pause();
        }

        private void ShowSchedule()
        {
            ConsoleHelper.PrintHeader("РОЗКЛАД НА ДЕНЬ");

            var doctors = _doctorRepo.GetAll();
            foreach (var d in doctors)
                ConsoleHelper.PrintInfo($"[{d.Id}] {d.FullName}");

            var doctorId = ConsoleHelper.ReadInt("ID лікаря (0 = всі)");
            var dateStr = ConsoleHelper.ReadLine("Дата (дд.мм.рррр, Enter = сьогодні)");
            var date = string.IsNullOrEmpty(dateStr)
                ? DateTime.Today
                : DateTime.TryParseExact(dateStr, "dd.MM.yyyy", null,
                    System.Globalization.DateTimeStyles.None, out var d2) ? d2 : DateTime.Today;

            List<Visit> visits;
            if (doctorId == 0)
                visits = _visitRepo.GetByDate(date);
            else
                visits = _visitService.GetDoctorScheduleForDay(doctorId, date);

            if (visits.Count == 0)
            {
                ConsoleHelper.PrintInfo("Записів немає.");
            }
            else
            {
                foreach (var v in visits)
                {
                    var patient = _patientRepo.GetById(v.PatientId);
                    var doctor = _doctorRepo.GetById(v.DoctorId);
                    ConsoleHelper.PrintInfo(
                        $"[{v.Id}] {v.StartDateTime:HH:mm}-{v.EndDateTime:HH:mm}  " +
                        $"{patient?.FullName,-25}  лікар: {doctor?.FullName,-25}  [{v.Status}]");
                }
            }
            ConsoleHelper.Pause();
        }

        private void CompleteVisit()
        {
            var id = ConsoleHelper.ReadInt("ID візиту");
            var (success, message) = _visitService.CompleteVisit(id);
            if (success) ConsoleHelper.PrintSuccess(message);
            else ConsoleHelper.PrintError(message);
            ConsoleHelper.Pause();
        }

        private void CancelVisit()
        {
            var id = ConsoleHelper.ReadInt("ID візиту");
            var (success, message) = _visitService.CancelVisit(id);
            if (success) ConsoleHelper.PrintSuccess(message);
            else ConsoleHelper.PrintError(message);
            ConsoleHelper.Pause();
        }

        private void RescheduleVisit()
        {
            var id = ConsoleHelper.ReadInt("ID візиту");
            var dateStr = ConsoleHelper.ReadLine("Нова дата (дд.мм.рррр)");
            var timeStr = ConsoleHelper.ReadLine("Новий час (гг:хх)");
            if (!DateTime.TryParseExact($"{dateStr} {timeStr}", "dd.MM.yyyy HH:mm", null,
                    System.Globalization.DateTimeStyles.None, out var newDt))
            {
                ConsoleHelper.PrintError("Невірний формат.");
                ConsoleHelper.Pause();
                return;
            }

            var (success, message) = _visitService.RescheduleVisit(id, newDt);
            if (success) ConsoleHelper.PrintSuccess(message);
            else ConsoleHelper.PrintError(message);
            ConsoleHelper.Pause();
        }

        private void PatientHistory()
        {
            var id = ConsoleHelper.ReadInt("ID пацієнта");
            var visits = _visitRepo.GetByPatient(id);
            if (visits.Count == 0)
            {
                ConsoleHelper.PrintInfo("Візитів не знайдено.");
            }
            else
            {
                foreach (var v in visits)
                {
                    var doctor = _doctorRepo.GetById(v.DoctorId);
                    ConsoleHelper.PrintInfo(
                        $"[{v.Id}] {v.StartDateTime:dd.MM.yyyy HH:mm}  лікар: {doctor?.FullName,-25}  [{v.Status}]");
                }
            }
            ConsoleHelper.Pause();
        }
    }
}
