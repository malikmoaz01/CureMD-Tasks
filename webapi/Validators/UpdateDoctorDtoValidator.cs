using FluentValidation;
using webapi.Models;

namespace webapi.Validators
{
    public class UpdateDoctorDtoValidator : AbstractValidator<UpdateDoctorDto>
    {
        public UpdateDoctorDtoValidator()
        {
            RuleFor(x => x.DoctorId)
                .NotEmpty().WithMessage("Doctor ID is required")
                .GreaterThan(0).WithMessage("Doctor ID must be greater than 0");

            RuleFor(x => x.DoctorName)
                .NotEmpty().WithMessage("Doctor name is required")
                .MaximumLength(100).WithMessage("Doctor name cannot exceed 100 characters");

            RuleFor(x => x.Specialization)
                .MaximumLength(100).WithMessage("Specialization cannot exceed 100 characters")
                .When(x => !string.IsNullOrEmpty(x.Specialization));

            RuleFor(x => x.ContactNumber)
                .MaximumLength(15).WithMessage("Contact number cannot exceed 15 characters")
                .Matches(@"^\+?[\d\s\-\(\)]+$").WithMessage("Invalid phone number format")
                .When(x => !string.IsNullOrEmpty(x.ContactNumber));

            RuleFor(x => x.Email)
                .MaximumLength(100).WithMessage("Email cannot exceed 100 characters")
                .EmailAddress().WithMessage("Invalid email format")
                .When(x => !string.IsNullOrEmpty(x.Email));
        }
    }
}
