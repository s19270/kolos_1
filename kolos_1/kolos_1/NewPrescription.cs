using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kolos_1
{
    public class NewPrescription
    {
        public string Date { get; set; }

        public string DueDate { get; set; }

        public int IdDoctor { get; set; }

        public int IdPatient { get; set; }
    }
}
