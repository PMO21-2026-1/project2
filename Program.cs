using System;
using System.Collections.Generic;
using DentalClinic.Data;
using DentalClinic.Models;
using DentalClinic.Services;
using DentalClinic.UI;

namespace DentalClinic
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // ── Репозиторії ──────────────────────────────────────────────────
            var patientRepo       = new PatientRepository();
            var doctorRepo        = new DoctorRepository();
            var serviceRepo       = new ServiceRepository();
            var visitRepo         = new VisitRepository();
            var dentalRecordRepo  = new DentalRecordRepository();
            var invoiceRepo       = new InvoiceRepository();

            // ── Сервіси ──────────────────────────────────────────────────────
            var visitService      = new VisitService(visitRepo, doctorRepo, patientRepo, serviceRepo);
            var invoiceService    = new InvoiceService(invoiceRepo, visitRepo, serviceRepo);
            var dentalService     = new DentalRecordService(dentalRecordRepo, patientRepo);
            var reportService     = new ReportService(visitRepo, invoiceRepo, doctorRepo, serviceRepo);

            // ── Меню ─────────────────────────────────────────────────────────
            var patientMenu  = new PatientMenu(patientRepo, dentalService);
            var doctorMenu   = new DoctorMenu(doctorRepo);
            var visitMenu    = new VisitMenu(visitService, visitRepo, doctorRepo, patientRepo, serviceRepo);
            var financeMenu  = new FinanceMenu(invoiceService, invoiceRepo, visitRepo, patientRepo, reportService);
            var toothMenu    = new ToothMenu(dentalService, patientRepo);
            var serviceMenu  = new ServiceMenu(serviceRepo);

            // ── Тестові дані ─────────────────────────────────────────────────
            SeedData(patientRepo, doctorRepo, serviceRepo);

            // ── Головний цикл ─────────────────────────────────────────────────
            bool running = true;
            while (running)
            {
                Console.Clear();
                Console.WriteLine("╔══════════════════════════════════════════╗");
                Console.WriteLine("       DentalFlow — Стоматологія           ");
                Console.WriteLine("╚══════════════════════════════════════════╝");
                Console.WriteLine("  1. Пацієнти");
                Console.WriteLine("  2. Лікарі");
                Console.WriteLine("  3. Послуги / Прайс-лист");
                Console.WriteLine("  4. Записи / Візити");
                Console.WriteLine("  5. Зубна картка");
                Console.WriteLine("  6. Фінанси та звіти");
                Console.WriteLine("  0. Вихід");
                Console.WriteLine();

                var choice = ConsoleHelper.ReadInt("Вибір");
                switch (choice)
                {
                    case 1: patientMenu.Show(); break;
                    case 2: doctorMenu.Show(); break;
                    case 3: serviceMenu.Show(); break;
                    case 4: visitMenu.Show(); break;
                    case 5: toothMenu.Show(); break;
                    case 6: financeMenu.Show(); break;
                    case 0: running = false; break;
                    default: ConsoleHelper.PrintError("Невірний вибір."); break;
                }
            }

            Console.WriteLine("  До побачення!");
        }

        // Тестові дані для демонстрації
        static void SeedData(
            PatientRepository patientRepo,
            DoctorRepository doctorRepo,
            ServiceRepository serviceRepo)
        {
            // Лікарі
            doctorRepo.Add(new Doctor
            {
                FullName = "Іванов Іван Іванович",
                Phone = "0501234567",
                Specialization = "Ортодонт",
                Status = DoctorStatus.Working
            });
            doctorRepo.Add(new Doctor
            {
                FullName = "Коваленко Марія Петрівна",
                Phone = "0671112233",
                Specialization = "Терапевт",
                Status = DoctorStatus.Working
            });

            // Пацієнти
            patientRepo.Add(new Patient
            {
                FullName = "Петренко Олеся Василівна",
                Phone = "0671234567",
                Gender = Gender.Female,
                BirthDate = new DateTime(1995, 3, 15),
                Allergies = "Лідокаїн",
                ChronicDiseases = "Немає"
            });
            patientRepo.Add(new Patient
            {
                FullName = "Мельник Дмитро Олегович",
                Phone = "0991234567",
                Gender = Gender.Male,
                BirthDate = new DateTime(1988, 7, 22),
                Allergies = "Немає",
                ChronicDiseases = "Цукровий діабет 2 типу"
            });

            // Послуги
            serviceRepo.Add(new Service { Name = "Чищення зубів",         Price = 800m,  DefaultDurationMinutes = 45 });
            serviceRepo.Add(new Service { Name = "Лікування карієсу",     Price = 1200m, DefaultDurationMinutes = 60 });
            serviceRepo.Add(new Service { Name = "Видалення зуба",        Price = 900m,  DefaultDurationMinutes = 30 });
            serviceRepo.Add(new Service { Name = "Встановлення коронки",  Price = 4500m, DefaultDurationMinutes = 90 });
            serviceRepo.Add(new Service { Name = "Рентген",               Price = 200m,  DefaultDurationMinutes = 10 });
            serviceRepo.Add(new Service { Name = "Консультація",          Price = 300m,  DefaultDurationMinutes = 20 });
        }
    }
}
