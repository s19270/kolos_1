using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kolos_1
{
    public class Prescription
    {
        public string IdPrescription { get; set; }

        public DateTime Date { get; set; }

        public DateTime DueDate { get; set; }

        public string DoctorLastName { get; set; }

        public string PatientLastName { get; set; }
    }
}
