using FluentValidation;
using webapi.Models;

namespace webapi.Validators
{
    public class FeeRateValidator : AbstractValidator<FeeRate>
    {
        public FeeRateValidator()
        {
            RuleFor(x => x.FeeRateId)
                .GreaterThan(0).WithMessage("Fee rate ID must be greater than 0")
                .When(x => x.FeeRateId > 0);

            RuleFor(x => x.BaseRate)
                .GreaterThanOrEqualTo(0).WithMessage("Base rate must be greater than or equal to 0");

            RuleFor(x => x.ExtraTimeRate)
                .InclusiveBetween(0, 1).WithMessage("Extra time rate must be between 0 and 1");

            RuleFor(x => x.ExtraTimeThreshold)
                .GreaterThan(0).WithMessage("Extra time threshold must be greater than 0");
        }
    }
}
