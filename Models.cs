using System;
using System.Collections.Generic;

namespace DentalClinic
{
	

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

	

	public class Patient
	{
		public int Id { get; set; }
		public string FullName { get; set; }      
		public string Phone { get; set; }        
		public Gender Gender { get; set; }        
		public DateTime BirthDate { get; set; }   
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
	}

	public class DentalRecord
	{
		public int Id { get; set; }
		public int PatientId { get; set; }          
		public int ToothNumber { get; set; }        
		public string ToothCondition { get; set; }  
		public string WorkDescription { get; set; } 
	}

	public class Invoice
	{
		public int Id { get; set; }
		public int VisitId { get; set; }          
		public decimal TotalAmount { get; set; }  
		public PaymentStatus Status { get; set; } 
	}
}