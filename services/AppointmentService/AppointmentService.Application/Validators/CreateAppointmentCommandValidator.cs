using AppointmentService.Application.Commands;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Application.Validators
{
    public class CreateAppointmentCommandValidator : AbstractValidator<CreateAppointmentCommand>
    {
        public CreateAppointmentCommandValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.Description)
                .MaximumLength(1000);

            RuleFor(x => x.StartDateTime)
                .NotEmpty()
                .Must(BeInFuture)
                .WithMessage("Start date must be in the future");

            RuleFor(x => x.EndDateTime)
                .NotEmpty()
                .Must((command, endDateTime) => endDateTime > command.StartDateTime)
                .WithMessage("End date must be after start date");

            RuleFor(x => x.PatientId)
                .NotEmpty();

            RuleFor(x => x.DoctorId)
                .NotEmpty();

            RuleFor(x => x.Location)
                .NotEmpty()
                .MaximumLength(200);
        }

        private bool BeInFuture(DateTime dateTime)
        {
            return dateTime > DateTime.UtcNow;
        }
    }
}
