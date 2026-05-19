using System;
using DentalClinic.Data;
using DentalClinic.Models;
using DentalClinic.Services;

namespace DentalClinic.UI
{
    public class FinanceMenu
    {
        private readonly InvoiceService _invoiceService;
        private readonly InvoiceRepository _invoiceRepo;
        private readonly VisitRepository _visitRepo;
        private readonly PatientRepository _patientRepo;
        private readonly ReportService _reportService;

        public FinanceMenu(
            InvoiceService invoiceService,
            InvoiceRepository invoiceRepo,
            VisitRepository visitRepo,
            PatientRepository patientRepo,
            ReportService reportService)
        {
            _invoiceService = invoiceService;
            _invoiceRepo = invoiceRepo;
            _visitRepo = visitRepo;
            _patientRepo = patientRepo;
            _reportService = reportService;
        }

        public void Show()
        {
            bool running = true;
            while (running)
            {
                ConsoleHelper.PrintHeader("ФІНАНСИ");
                Console.WriteLine("  1. Сформувати рахунок для візиту");
                Console.WriteLine("  2. Прийняти оплату");
                Console.WriteLine("  3. Список неоплачених рахунків");
                Console.WriteLine("  4. Місячний звіт");
                Console.WriteLine("  0. Назад");

                var choice = ConsoleHelper.ReadInt("Вибір");
                switch (choice)
                {
                    case 1: GenerateInvoice(); break;
                    case 2: MarkPaid(); break;
                    case 3: ListUnpaid(); break;
                    case 4: MonthlyReport(); break;
                    case 0: running = false; break;
                    default: ConsoleHelper.PrintError("Невірний вибір."); break;
                }
            }
        }

        private void GenerateInvoice()
        {
            var visitId = ConsoleHelper.ReadInt("ID візиту");
            var (success, message, invoice) = _invoiceService.GenerateInvoice(visitId);
            if (success)
                ConsoleHelper.PrintSuccess($"{message}  (ID рахунку: {invoice.Id})");
            else
                ConsoleHelper.PrintError(message);
            ConsoleHelper.Pause();
        }

        private void MarkPaid()
        {
            var invoiceId = ConsoleHelper.ReadInt("ID рахунку");
            Console.WriteLine("  Спосіб оплати:  1. Готівка   2. Картка");
            var methodChoice = ConsoleHelper.ReadInt("Вибір");
            var method = methodChoice == 2 ? PaymentMethod.Card : PaymentMethod.Cash;

            var (success, message) = _invoiceService.MarkAsPaid(invoiceId, method);
            if (success) ConsoleHelper.PrintSuccess(message);
            else ConsoleHelper.PrintError(message);
            ConsoleHelper.Pause();
        }

        private void ListUnpaid()
        {
            ConsoleHelper.PrintHeader("НЕОПЛАЧЕНІ РАХУНКИ");
            var invoices = _invoiceRepo.GetUnpaid();
            if (invoices.Count == 0)
            {
                ConsoleHelper.PrintInfo("Всі рахунки оплачено.");
            }
            else
            {
                foreach (var inv in invoices)
                {
                    var visit = _visitRepo.GetById(inv.VisitId);
                    var patient = visit != null ? _patientRepo.GetById(visit.PatientId) : null;
                    ConsoleHelper.PrintInfo(
                        $"[{inv.Id}] {inv.IssuedAt:dd.MM.yyyy}  пацієнт: {patient?.FullName,-25}  " +
                        $"сума: {inv.TotalAmount:0.00} грн");
                }
            }
            ConsoleHelper.Pause();
        }

        private void MonthlyReport()
        {
            var year = ConsoleHelper.ReadInt("Рік", DateTime.Today.Year);
            var month = ConsoleHelper.ReadInt("Місяць (1-12)", DateTime.Today.Month);
            _reportService.PrintMonthlyReport(year, month);
            ConsoleHelper.Pause();
        }
    }
}
