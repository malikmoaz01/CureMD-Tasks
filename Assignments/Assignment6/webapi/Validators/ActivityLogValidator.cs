using FluentValidation;
using webapi.Models;

namespace webapi.Validators
{
    public class ActivityLogValidator : AbstractValidator<ActivityLog>
    {
        public ActivityLogValidator()
        {
            RuleFor(x => x.Action)
                .NotEmpty().WithMessage("Action is required")
                .MaximumLength(100).WithMessage("Action cannot exceed 100 characters");

            RuleFor(x => x.Details)
                .MaximumLength(500).WithMessage("Details cannot exceed 500 characters")
                .When(x => !string.IsNullOrEmpty(x.Details));

            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("User ID must be greater than 0")
                .When(x => x.UserId.HasValue);
        }
    }
}