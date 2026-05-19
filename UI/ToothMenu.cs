using System;
using DentalClinic.Data;
using DentalClinic.Models;
using DentalClinic.Services;

namespace DentalClinic.UI
{
    public class ToothMenu
    {
        private readonly DentalRecordService _dentalService;
        private readonly PatientRepository _patientRepo;

        public ToothMenu(DentalRecordService dentalService, PatientRepository patientRepo)
        {
            _dentalService = dentalService;
            _patientRepo = patientRepo;
        }

        public void Show()
        {
            bool running = true;
            while (running)
            {
                ConsoleHelper.PrintHeader("ЗУБНА КАРТКА");
                Console.WriteLine("  1. Переглянути зубну формулу");
                Console.WriteLine("  2. Оновити стан зуба");
                Console.WriteLine("  3. Історія зуба");
                Console.WriteLine("  0. Назад");

                var choice = ConsoleHelper.ReadInt("Вибір");
                switch (choice)
                {
                    case 1: ShowFormula(); break;
                    case 2: UpdateTooth(); break;
                    case 3: ToothHistory(); break;
                    case 0: running = false; break;
                    default: ConsoleHelper.PrintError("Невірний вибір."); break;
                }
            }
        }

        private void ShowFormula()
        {
            var id = ConsoleHelper.ReadInt("ID пацієнта");
            _dentalService.PrintToothFormula(id);
            ConsoleHelper.Pause();
        }

        private void UpdateTooth()
        {
            var patientId = ConsoleHelper.ReadInt("ID пацієнта");
            var tooth = ConsoleHelper.ReadInt("Номер зуба (1-32)");

            Console.WriteLine("  Стан:");
            Console.WriteLine("  1. Здоровий  2. Карієс  3. Пломба  4. Коронка  5. Відсутній  6. Лікується");
            var condChoice = ConsoleHelper.ReadInt("Вибір");
            var condition = condChoice switch
            {
                1 => ToothCondition.Healthy,
                2 => ToothCondition.Caries,
                3 => ToothCondition.Filling,
                4 => ToothCondition.Crown,
                5 => ToothCondition.Missing,
                6 => ToothCondition.UnderTreatment,
                _ => ToothCondition.Healthy
            };

            var description = ConsoleHelper.ReadLine("Опис роботи (або Enter)");

            var (success, message) = _dentalService.SetToothCondition(patientId, tooth, condition, description);
            if (success) ConsoleHelper.PrintSuccess(message);
            else ConsoleHelper.PrintError(message);
            ConsoleHelper.Pause();
        }

        private void ToothHistory()
        {
            var patientId = ConsoleHelper.ReadInt("ID пацієнта");
            var tooth = ConsoleHelper.ReadInt("Номер зуба (1-32)");
            var history = _dentalService.GetToothHistory(patientId, tooth);

            if (history.Count == 0)
            {
                ConsoleHelper.PrintInfo("Записів для цього зуба немає.");
            }
            else
            {
                foreach (var r in history)
                    ConsoleHelper.PrintInfo(
                        $"{r.RecordDate:dd.MM.yyyy}  [{r.Condition}]  {r.WorkDescription}");
            }
            ConsoleHelper.Pause();
        }
    }
}
