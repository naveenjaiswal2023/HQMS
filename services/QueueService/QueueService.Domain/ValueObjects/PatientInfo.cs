using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueService.Domain.ValueObjects
{
    public class PatientInfo
    {
        public string Name { get; private set; }
        public int Age { get; private set; }
        public string Gender { get; private set; }

        private PatientInfo() { }

        public PatientInfo(string name, int age, string gender)
        {
            Name = name;
            Age = age;
            Gender = gender;
        }
    }
}
