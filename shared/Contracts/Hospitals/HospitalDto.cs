using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Hospitals
{
    public class HospitalDto
    {
        public Guid Id { get; set; }               // Unique identifier
        public string Name { get; set; }           // Hospital name
        public string Code { get; set; }           // Optional internal hospital code
        public string Address { get; set; }        // Full address
        public string City { get; set; }           // City name
        public string State { get; set; }          // State
        public string ZipCode { get; set; }        // Zip/postal code
        public string PhoneNumber { get; set; }    // Contact number
        public string Email { get; set; }          // Optional email
    }
}
