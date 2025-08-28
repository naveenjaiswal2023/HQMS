using AuthService.Application.Commands;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Validators
{
    public class UserRegisterCommandValidator : AbstractValidator<UserRegisterCommand>
    {
        public UserRegisterCommandValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Username is required")
                .MinimumLength(4).WithMessage("Username must be at least 4 characters long");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email address");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required")
                .Matches(@"^\d{10}$").WithMessage("Phone number must be 10 digits");

            RuleFor(x => x.DateOfBirth)
                .LessThan(DateTime.Now).WithMessage("Date of Birth must be in the past")
                .GreaterThan(DateTime.Now.AddYears(-120)).WithMessage("Date of Birth is not valid");
        }
    }
}
