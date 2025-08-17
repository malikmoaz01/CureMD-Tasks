using FluentValidation;
using webapi.Models;

namespace webapi.Validators
{
    public class UpdatePatientDtoValidator : AbstractValidator<UpdatePatientDto>
    {
        public UpdatePatientDtoValidator()
        {
            RuleFor(x => x.PatientId)
                .NotEmpty().WithMessage("Patient ID is required")
                .GreaterThan(0).WithMessage("Patient ID must be greater than 0");

            RuleFor(x => x.PatientName)
                .NotEmpty().WithMessage("Patient name is required")
                .MaximumLength(100).WithMessage("Patient name cannot exceed 100 characters");

            RuleFor(x => x.DateOfBirth)
                .LessThan(DateTime.Today).WithMessage("Date of birth must be in the past")
                .When(x => x.DateOfBirth.HasValue);

            RuleFor(x => x.Gender)
                .Must(gender => new[] { "Male", "Female", "Other" }.Contains(gender))
                .WithMessage("Gender must be Male, Female, or Other")
                .When(x => !string.IsNullOrEmpty(x.Gender));

            RuleFor(x => x.ContactNumber)
                .MaximumLength(15).WithMessage("Contact number cannot exceed 15 characters")
                .Matches(@"^\+?[\d\s\-\(\)]+$").WithMessage("Invalid phone number format")
                .When(x => !string.IsNullOrEmpty(x.ContactNumber));

            RuleFor(x => x.Email)
                .MaximumLength(100).WithMessage("Email cannot exceed 100 characters")
                .EmailAddress().WithMessage("Invalid email format")
                .When(x => !string.IsNullOrEmpty(x.Email));

            RuleFor(x => x.Address)
                .MaximumLength(255).WithMessage("Address cannot exceed 255 characters")
                .When(x => !string.IsNullOrEmpty(x.Address));

            RuleFor(x => x.EmergencyContact)
                .MaximumLength(15).WithMessage("Emergency contact cannot exceed 15 characters")
                .Matches(@"^\+?[\d\s\-\(\)]+$").WithMessage("Invalid emergency contact format")
                .When(x => !string.IsNullOrEmpty(x.EmergencyContact));
        }
    }
}
