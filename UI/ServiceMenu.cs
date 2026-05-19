using System;
using DentalClinic.Data;
using DentalClinic.Models;

namespace DentalClinic.UI
{
    public class ServiceMenu
    {
        private readonly ServiceRepository _serviceRepo;

        public ServiceMenu(ServiceRepository serviceRepo)
        {
            _serviceRepo = serviceRepo;
        }

        public void Show()
        {
            bool running = true;
            while (running)
            {
                ConsoleHelper.PrintHeader("ПОСЛУГИ / ПРАЙС-ЛИСТ");
                Console.WriteLine("  1. Додати послугу");
                Console.WriteLine("  2. Список послуг");
                Console.WriteLine("  3. Видалити послугу");
                Console.WriteLine("  0. Назад");

                var choice = ConsoleHelper.ReadInt("Вибір");
                switch (choice)
                {
                    case 1: AddService(); break;
                    case 2: ListServices(); break;
                    case 3: DeleteService(); break;
                    case 0: running = false; break;
                    default: ConsoleHelper.PrintError("Невірний вибір."); break;
                }
            }
        }

        private void AddService()
        {
            ConsoleHelper.PrintHeader("НОВА ПОСЛУГА");
            var name = ConsoleHelper.ReadLine("Назва");
            var priceStr = ConsoleHelper.ReadLine("Ціна (грн)");
            var duration = ConsoleHelper.ReadInt("Тривалість (хвилин)", 30);

            if (!decimal.TryParse(priceStr, out var price))
            {
                ConsoleHelper.PrintError("Невірний формат ціни.");
                ConsoleHelper.Pause();
                return;
            }

            var service = new Service
            {
                Name = name,
                Price = price,
                DefaultDurationMinutes = duration
            };

            _serviceRepo.Add(service);
            ConsoleHelper.PrintSuccess($"Послугу '{name}' додано. ID: {service.Id}");
            ConsoleHelper.Pause();
        }

        private void ListServices()
        {
            ConsoleHelper.PrintHeader("ПРАЙС-ЛИСТ");
            var services = _serviceRepo.GetAll();
            if (services.Count == 0)
            {
                ConsoleHelper.PrintInfo("Список послуг порожній.");
            }
            else
            {
                foreach (var s in services)
                    ConsoleHelper.PrintInfo(
                        $"[{s.Id}] {s.Name,-35}  {s.Price,8:0.00} грн  ({s.DefaultDurationMinutes} хв)");
            }
            ConsoleHelper.Pause();
        }

        private void DeleteService()
        {
            var id = ConsoleHelper.ReadInt("ID послуги для видалення");
            var service = _serviceRepo.GetById(id);
            if (service == null)
            {
                ConsoleHelper.PrintError("Послугу не знайдено.");
            }
            else
            {
                _serviceRepo.Delete(id);
                ConsoleHelper.PrintSuccess($"Послугу '{service.Name}' видалено.");
            }
            ConsoleHelper.Pause();
        }
    }
}
