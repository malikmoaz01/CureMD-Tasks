using FluentValidation;
using webapi.Models;

namespace webapi.Validators
{
    public class UpdatePatientVisitDtoValidator : AbstractValidator<UpdatePatientVisitDto>
    {
        public UpdatePatientVisitDtoValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Visit ID is required")
                .GreaterThan(0).WithMessage("Visit ID must be greater than 0");

            RuleFor(x => x.PatientId)
                .NotEmpty().WithMessage("Patient ID is required")
                .GreaterThan(0).WithMessage("Patient ID must be greater than 0");

            RuleFor(x => x.DoctorId)
                .GreaterThan(0).WithMessage("Doctor ID must be greater than 0")
                .When(x => x.DoctorId.HasValue);

            RuleFor(x => x.VisitTypeId)
                .NotEmpty().WithMessage("Visit Type ID is required")
                .GreaterThan(0).WithMessage("Visit Type ID must be greater than 0");

            RuleFor(x => x.VisitDate)
                .NotEmpty().WithMessage("Visit Date is required");

            RuleFor(x => x.Note)
                .MaximumLength(500).WithMessage("Note cannot exceed 500 characters")
                .When(x => !string.IsNullOrEmpty(x.Note));

            RuleFor(x => x.DurationInMinutes)
                .NotEmpty().WithMessage("Duration in minutes is required")
                .InclusiveBetween(1, 480).WithMessage("Duration must be between 1 and 480 minutes");

            RuleFor(x => x.Fee)
                .NotEmpty().WithMessage("Fee is required")
                .GreaterThanOrEqualTo(0).WithMessage("Fee must be greater than or equal to 0");
        }
    }
}