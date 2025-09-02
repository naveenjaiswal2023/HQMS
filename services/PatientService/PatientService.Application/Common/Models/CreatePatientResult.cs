using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientService.Application.Common.Models
{
    public record CreatePatientResult(Guid PatientId, string Message);

}
