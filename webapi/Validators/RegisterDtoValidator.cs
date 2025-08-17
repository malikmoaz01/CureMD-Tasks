using FluentValidation;
using webapi.Models;

namespace webapi.Validators
{
    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(50).WithMessage("Email cannot exceed 50 characters");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long")
                .MaximumLength(255).WithMessage("Password cannot exceed 255 characters");

            RuleFor(x => x.UserRole)
                .NotEmpty().WithMessage("User role is required")
                .Must(role => new[] { "Admin", "Receptionist", "Doctor" }.Contains(role))
                .WithMessage("User role must be Admin, Receptionist, or Doctor");
        }
    }
}