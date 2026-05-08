using DentalClinic;
namespace project2
{
    internal class Program
    {
        static void Main(string[] args)
        {

            
            var doctor = new Doctor
            {
                Id = 1,
                FullName = "Iванов Iван Iванович",
                Phone = "0501234567",
                Specialization = "Ортодонт",
                Status = DoctorStatus.Working
            };

            
            var patient = new Patient
            {
                Id = 1,
                FullName = "Петренко Олеся Василiвна",
                Phone = "0671234567",
                Gender = Gender.Female,
                BirthDate = new DateTime(1995, 3, 15)
            };

           
            var service = new Service
            {
                Id = 1,
                Name = "Чищення зубiв",
                Price = 800.00m
            };

            
            Console.WriteLine($"Лiкар: {doctor.FullName}, статус: {doctor.Status}");
            Console.WriteLine($"Пацiєнт: {patient.FullName}, тел: {patient.Phone}");
            Console.WriteLine($"Послуга: {service.Name}, цiна: {service.Price} грн");
        }
    }
}
