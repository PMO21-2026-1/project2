using System.Collections.Generic;
using System.Linq;
using DentalClinic.Models;

namespace DentalClinic.Data
{
    public class InvoiceRepository : IRepository<Invoice>
    {
        private readonly List<Invoice> _invoices = new List<Invoice>();
        private int _nextId = 1;

        public void Add(Invoice invoice)
        {
            invoice.Id = _nextId++;
            _invoices.Add(invoice);
        }

        public Invoice GetById(int id) =>
            _invoices.FirstOrDefault(i => i.Id == id);

        public List<Invoice> GetAll() => new List<Invoice>(_invoices);

        public void Update(Invoice updated)
        {
            var index = _invoices.FindIndex(i => i.Id == updated.Id);
            if (index >= 0)
                _invoices[index] = updated;
        }

        public void Delete(int id) =>
            _invoices.RemoveAll(i => i.Id == id);

        public Invoice GetByVisit(int visitId) =>
            _invoices.FirstOrDefault(i => i.VisitId == visitId);

        public List<Invoice> GetUnpaid() =>
            _invoices.Where(i => i.Status == PaymentStatus.Unpaid).ToList();

        public List<Invoice> GetByMonth(int year, int month) =>
            _invoices.Where(i =>
                i.IssuedAt.Year == year &&
                i.IssuedAt.Month == month)
            .ToList();
    }
}
