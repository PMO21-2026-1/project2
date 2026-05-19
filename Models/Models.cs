using System;
using System.Collections.Generic;

namespace DentalClinic.Models
{
    // ─── Enums ───────────────────────────────────────────────────────────────

    public enum Gender
    {
        Male,
        Female
    }

    public enum DoctorStatus
    {
        Working,
        OnVacation,
        SickLeave
    }

    public enum VisitStatus
    {
        Planned,
        Completed,
        Canceled,
        Rescheduled
    }

    public enum PaymentStatus
    {
        Paid,
        Unpaid
    }

    public enum PaymentMethod
    {
        Cash,
        Card
    }

    public enum ToothCondition
    {
        Healthy,
        Caries,
        Filling,
        Crown,
        Missing,
        UnderTreatment
    }

    // ─── Entities ────────────────────────────────────────────────────────────

    public class Patient
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public Gender Gender { get; set; }
        public DateTime BirthDate { get; set; }

        // Медичний анамнез — критично для лікаря перед анестезією
        public string Allergies { get; set; } = "Немає";
        public string ChronicDiseases { get; set; } = "Немає";

        public int Age => DateTime.Today.Year - BirthDate.Year -
                          (DateTime.Today.DayOfYear < BirthDate.DayOfYear ? 1 : 0);
    }

    public class Doctor
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Specialization { get; set; }
        public DoctorStatus Status { get; set; }
    }

    public class Service
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int DefaultDurationMinutes { get; set; } = 30;
    }

    public class Visit
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public List<int> ServiceIds { get; set; } = new List<int>();
        public DateTime StartDateTime { get; set; }
        public int DurationMinutes { get; set; }
        public VisitStatus Status { get; set; }
        public string Notes { get; set; } = "";

        public DateTime EndDateTime => StartDateTime.AddMinutes(DurationMinutes);
    }

    public class DentalRecord
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int ToothNumber { get; set; }          // 1–32
        public ToothCondition Condition { get; set; }
        public string WorkDescription { get; set; }
        public DateTime RecordDate { get; set; } = DateTime.Today;
    }

    public class Invoice
    {
        public int Id { get; set; }
        public int VisitId { get; set; }
        public decimal TotalAmount { get; set; }
        public PaymentStatus Status { get; set; }
        public PaymentMethod Method { get; set; }
        public DateTime IssuedAt { get; set; } = DateTime.Now;
    }
}
