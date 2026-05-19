using System;
using DentalClinic.Data;
using DentalClinic.Models;

namespace DentalClinic.UI
{
    public class DoctorMenu
    {
        private readonly DoctorRepository _doctorRepo;

        public DoctorMenu(DoctorRepository doctorRepo)
        {
            _doctorRepo = doctorRepo;
        }

        public void Show()
        {
            bool running = true;
            while (running)
            {
                ConsoleHelper.PrintHeader("ЛІКАРІ");
                Console.WriteLine("  1. Додати лікаря");
                Console.WriteLine("  2. Список лікарів");
                Console.WriteLine("  3. Змінити статус лікаря");
                Console.WriteLine("  0. Назад");

                var choice = ConsoleHelper.ReadInt("Вибір");
                switch (choice)
                {
                    case 1: AddDoctor(); break;
                    case 2: ListDoctors(); break;
                    case 3: ChangeStatus(); break;
                    case 0: running = false; break;
                    default: ConsoleHelper.PrintError("Невірний вибір."); break;
                }
            }
        }

        private void AddDoctor()
        {
            ConsoleHelper.PrintHeader("НОВИЙ ЛІКАР");

            var fullName = ConsoleHelper.ReadLine("ПІБ");
            var phone = ConsoleHelper.ReadLine("Телефон");
            var spec = ConsoleHelper.ReadLine("Спеціалізація");

            var doctor = new Doctor
            {
                FullName = fullName,
                Phone = phone,
                Specialization = spec,
                Status = DoctorStatus.Working
            };

            _doctorRepo.Add(doctor);
            ConsoleHelper.PrintSuccess($"Лікаря '{fullName}' додано. ID: {doctor.Id}");
            ConsoleHelper.Pause();
        }

        private void ListDoctors()
        {
            ConsoleHelper.PrintHeader("СПИСОК ЛІКАРІВ");
            var doctors = _doctorRepo.GetAll();
            if (doctors.Count == 0)
            {
                ConsoleHelper.PrintInfo("Список порожній.");
            }
            else
            {
                foreach (var d in doctors)
                    ConsoleHelper.PrintInfo(
                        $"[{d.Id}] {d.FullName,-30} {d.Specialization,-20} [{d.Status}]");
            }
            ConsoleHelper.Pause();
        }

        private void ChangeStatus()
        {
            var id = ConsoleHelper.ReadInt("ID лікаря");
            var doctor = _doctorRepo.GetById(id);
            if (doctor == null)
            {
                ConsoleHelper.PrintError("Лікаря не знайдено.");
                ConsoleHelper.Pause();
                return;
            }

            Console.WriteLine($"  Поточний статус: {doctor.Status}");
            Console.WriteLine("  1. Working (працює)");
            Console.WriteLine("  2. OnVacation (відпустка)");
            Console.WriteLine("  3. SickLeave (лікарняний)");

            var choice = ConsoleHelper.ReadInt("Вибір");
            doctor.Status = choice switch
            {
                1 => DoctorStatus.Working,
                2 => DoctorStatus.OnVacation,
                3 => DoctorStatus.SickLeave,
                _ => doctor.Status
            };

            _doctorRepo.Update(doctor);
            ConsoleHelper.PrintSuccess($"Статус оновлено: {doctor.Status}");
            ConsoleHelper.Pause();
        }
    }
}
