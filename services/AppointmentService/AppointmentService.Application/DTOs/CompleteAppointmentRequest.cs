﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Application.DTOs
{
    public class CompleteAppointmentRequest
    {
        public int Id { get; set; }
        public string Notes { get; set; }
    }
}
