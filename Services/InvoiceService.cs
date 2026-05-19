using System;
using DentalClinic.Data;
using DentalClinic.Models;

namespace DentalClinic.Services
{
    public class InvoiceService
    {
        private readonly InvoiceRepository _invoiceRepo;
        private readonly VisitRepository _visitRepo;
        private readonly ServiceRepository _serviceRepo;

        public InvoiceService(
            InvoiceRepository invoiceRepo,
            VisitRepository visitRepo,
            ServiceRepository serviceRepo)
        {
            _invoiceRepo = invoiceRepo;
            _visitRepo = visitRepo;
            _serviceRepo = serviceRepo;
        }

        // Автоматично сформувати рахунок на основі послуг візиту
        public (bool Success, string Message, Invoice Invoice) GenerateInvoice(int visitId)
        {
            var visit = _visitRepo.GetById(visitId);
            if (visit == null)
                return (false, "Візит не знайдено.", null);

            if (visit.Status != VisitStatus.Completed)
                return (false, "Рахунок можна виставити лише після завершеного візиту.", null);

            var existing = _invoiceRepo.GetByVisit(visitId);
            if (existing != null)
                return (false, "Рахунок для цього візиту вже існує.", null);

            // Підрахунок суми
            var services = _serviceRepo.GetByIds(visit.ServiceIds);
            decimal total = 0;
            foreach (var s in services)
                total += s.Price;

            var invoice = new Invoice
            {
                VisitId = visitId,
                TotalAmount = total,
                Status = PaymentStatus.Unpaid,
                IssuedAt = DateTime.Now
            };

            _invoiceRepo.Add(invoice);
            return (true, $"Рахунок сформовано. Сума: {total:0.00} грн", invoice);
        }

        // Зафіксувати оплату
        public (bool Success, string Message) MarkAsPaid(int invoiceId, PaymentMethod method)
        {
            var invoice = _invoiceRepo.GetById(invoiceId);
            if (invoice == null)
                return (false, "Рахунок не знайдено.");

            if (invoice.Status == PaymentStatus.Paid)
                return (false, "Рахунок вже оплачено.");

            invoice.Status = PaymentStatus.Paid;
            invoice.Method = method;
            _invoiceRepo.Update(invoice);
            return (true, $"Оплата {invoice.TotalAmount:0.00} грн прийнята ({method}).");
        }
    }
}
