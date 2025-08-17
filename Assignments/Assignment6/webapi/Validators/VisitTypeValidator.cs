using FluentValidation;
using webapi.Models;

namespace webapi.Validators
{
    public class VisitTypeValidator : AbstractValidator<VisitType>
    {
        public VisitTypeValidator()
        {
            RuleFor(x => x.VisitTypeId)
                .GreaterThan(0).WithMessage("Visit type ID must be greater than 0")
                .When(x => x.VisitTypeId > 0);

            RuleFor(x => x.VisitTypeName)
                .NotEmpty().WithMessage("Visit type name is required")
                .MaximumLength(50).WithMessage("Visit type name cannot exceed 50 characters");

            RuleFor(x => x.BaseRate)
                .GreaterThanOrEqualTo(0).WithMessage("Base rate must be greater than or equal to 0");

            RuleFor(x => x.Description)
                .MaximumLength(255).WithMessage("Description cannot exceed 255 characters")
                .When(x => !string.IsNullOrEmpty(x.Description));
        }
    }
}