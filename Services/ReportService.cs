using System;
using System.Collections.Generic;
using System.Linq;
using DentalClinic.Data;
using DentalClinic.Models;

namespace DentalClinic.Services
{
    public class MonthlyReport
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal PaidRevenue { get; set; }
        public decimal UnpaidRevenue { get; set; }
        public int TotalVisits { get; set; }
        public int CompletedVisits { get; set; }
        public int CanceledVisits { get; set; }
        public List<DoctorStat> DoctorStats { get; set; } = new();
    }

    public class DoctorStat
    {
        public string DoctorName { get; set; }
        public int VisitCount { get; set; }
        public decimal Revenue { get; set; }
    }

    public class ReportService
    {
        private readonly VisitRepository _visitRepo;
        private readonly InvoiceRepository _invoiceRepo;
        private readonly DoctorRepository _doctorRepo;
        private readonly ServiceRepository _serviceRepo;

        public ReportService(
            VisitRepository visitRepo,
            InvoiceRepository invoiceRepo,
            DoctorRepository doctorRepo,
            ServiceRepository serviceRepo)
        {
            _visitRepo = visitRepo;
            _invoiceRepo = invoiceRepo;
            _doctorRepo = doctorRepo;
            _serviceRepo = serviceRepo;
        }

        public MonthlyReport GetMonthlyReport(int year, int month)
        {
            var visits = _visitRepo.GetByMonth(year, month);
            var invoices = _invoiceRepo.GetByMonth(year, month);

            var report = new MonthlyReport
            {
                Year = year,
                Month = month,
                TotalVisits = visits.Count,
                CompletedVisits = visits.Count(v => v.Status == VisitStatus.Completed),
                CanceledVisits = visits.Count(v => v.Status == VisitStatus.Canceled),
                TotalRevenue = invoices.Sum(i => i.TotalAmount),
                PaidRevenue = invoices.Where(i => i.Status == PaymentStatus.Paid).Sum(i => i.TotalAmount),
                UnpaidRevenue = invoices.Where(i => i.Status == PaymentStatus.Unpaid).Sum(i => i.TotalAmount)
            };

            // Статистика по кожному лікарю
            var doctors = _doctorRepo.GetAll();
            foreach (var doctor in doctors)
            {
                var doctorVisits = visits.Where(v => v.DoctorId == doctor.Id).ToList();
                if (doctorVisits.Count == 0) continue;

                var visitIds = doctorVisits.Select(v => v.Id).ToHashSet();
                var doctorRevenue = invoices
                    .Where(i => visitIds.Contains(i.VisitId) && i.Status == PaymentStatus.Paid)
                    .Sum(i => i.TotalAmount);

                report.DoctorStats.Add(new DoctorStat
                {
                    DoctorName = doctor.FullName,
                    VisitCount = doctorVisits.Count,
                    Revenue = doctorRevenue
                });
            }

            report.DoctorStats = report.DoctorStats.OrderByDescending(d => d.Revenue).ToList();
            return report;
        }

        public void PrintMonthlyReport(int year, int month)
        {
            var report = GetMonthlyReport(year, month);
            var monthName = new DateTime(year, month, 1).ToString("MMMM yyyy");

            Console.WriteLine();
            Console.WriteLine($"╔══════════════════════════════════════════╗");
            Console.WriteLine($"  ЗВІТ ЗА {monthName.ToUpper()}");
            Console.WriteLine($"╚══════════════════════════════════════════╝");
            Console.WriteLine($"  Всього візитів:     {report.TotalVisits}");
            Console.WriteLine($"  Завершено:          {report.CompletedVisits}");
            Console.WriteLine($"  Скасовано:          {report.CanceledVisits}");
            Console.WriteLine($"  Загальний дохід:    {report.TotalRevenue:0.00} грн");
            Console.WriteLine($"  Оплачено:           {report.PaidRevenue:0.00} грн");
            Console.WriteLine($"  Борг:               {report.UnpaidRevenue:0.00} грн");

            if (report.DoctorStats.Count > 0)
            {
                Console.WriteLine();
                Console.WriteLine("  Рейтинг лікарів:");
                Console.WriteLine("  ─────────────────────────────────────");
                foreach (var stat in report.DoctorStats)
                    Console.WriteLine($"  {stat.DoctorName,-30} {stat.VisitCount} вiз.   {stat.Revenue:0.00} грн");
            }
            Console.WriteLine();
        }
    }
}
