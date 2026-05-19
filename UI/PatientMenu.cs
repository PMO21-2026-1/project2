using System;
using DentalClinic.Data;
using DentalClinic.Models;
using DentalClinic.Services;

namespace DentalClinic.UI
{
    public class PatientMenu
    {
        private readonly PatientRepository _patientRepo;
        private readonly DentalRecordService _dentalService;

        public PatientMenu(PatientRepository patientRepo, DentalRecordService dentalService)
        {
            _patientRepo = patientRepo;
            _dentalService = dentalService;
        }

        public void Show()
        {
            bool running = true;
            while (running)
            {
                ConsoleHelper.PrintHeader("ПАЦІЄНТИ");
                Console.WriteLine("  1. Додати пацієнта");
                Console.WriteLine("  2. Знайти пацієнта");
                Console.WriteLine("  3. Список всіх пацієнтів");
                Console.WriteLine("  4. Зубна формула пацієнта");
                Console.WriteLine("  0. Назад");

                var choice = ConsoleHelper.ReadInt("Вибір");
                switch (choice)
                {
                    case 1: AddPatient(); break;
                    case 2: SearchPatient(); break;
                    case 3: ListPatients(); break;
                    case 4: ShowToothFormula(); break;
                    case 0: running = false; break;
                    default: ConsoleHelper.PrintError("Невірний вибір."); break;
                }
            }
        }

        private void AddPatient()
        {
            ConsoleHelper.PrintHeader("НОВИЙ ПАЦІЄНТ");

            var fullName = ConsoleHelper.ReadLine("ПІБ");
            var phone = ConsoleHelper.ReadLine("Телефон");
            var genderInput = ConsoleHelper.ReadLine("Стать (ч/ж)").ToLower();
            var gender = genderInput == "ж" ? Gender.Female : Gender.Male;
            var birthDateStr = ConsoleHelper.ReadLine("Дата народження (дд.мм.рррр)");

            if (!DateTime.TryParseExact(birthDateStr, "dd.MM.yyyy", null,
                    System.Globalization.DateTimeStyles.None, out var birthDate))
            {
                ConsoleHelper.PrintError("Невірний формат дати.");
                return;
            }

            var allergies = ConsoleHelper.ReadLine("Алергії (або Enter щоб пропустити)");
            var chronic = ConsoleHelper.ReadLine("Хронічні хвороби (або Enter щоб пропустити)");

            var patient = new Patient
            {
                FullName = fullName,
                Phone = phone,
                Gender = gender,
                BirthDate = birthDate,
                Allergies = string.IsNullOrEmpty(allergies) ? "Немає" : allergies,
                ChronicDiseases = string.IsNullOrEmpty(chronic) ? "Немає" : chronic
            };

            _patientRepo.Add(patient);
            ConsoleHelper.PrintSuccess($"Пацієнта '{fullName}' додано. ID: {patient.Id}");
            ConsoleHelper.Pause();
        }

        private void SearchPatient()
        {
            ConsoleHelper.PrintHeader("ПОШУК ПАЦІЄНТА");
            var query = ConsoleHelper.ReadLine("ПІБ або телефон");
            var results = _patientRepo.Search(query);

            if (results.Count == 0)
            {
                ConsoleHelper.PrintError("Нічого не знайдено.");
            }
            else
            {
                foreach (var p in results)
                    PrintPatientRow(p);
            }
            ConsoleHelper.Pause();
        }

        private void ListPatients()
        {
            ConsoleHelper.PrintHeader("ВСІ ПАЦІЄНТИ");
            var patients = _patientRepo.GetAll();
            if (patients.Count == 0)
            {
                ConsoleHelper.PrintInfo("Список пацієнтів порожній.");
            }
            else
            {
                foreach (var p in patients)
                    PrintPatientRow(p);
            }
            ConsoleHelper.Pause();
        }

        private void ShowToothFormula()
        {
            var id = ConsoleHelper.ReadInt("ID пацієнта");
            _dentalService.PrintToothFormula(id);
            ConsoleHelper.Pause();
        }

        private void PrintPatientRow(Patient p)
        {
            ConsoleHelper.PrintInfo(
                $"[{p.Id}] {p.FullName,-30} тел: {p.Phone,-12} вік: {p.Age}  " +
                $"алергії: {p.Allergies}");
        }
    }
}
