using FluentValidation;
using webapi.Models;

namespace webapi.Validators
{
    public class CreateVisitTypeDtoValidator : AbstractValidator<CreateVisitTypeDto>
    {
        public CreateVisitTypeDtoValidator()
        {
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