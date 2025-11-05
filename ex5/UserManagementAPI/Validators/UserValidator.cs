using FluentValidation;
using UserManagementAPI.Models;

namespace UserManagementAPI.Validators
{
    /// <summary>
    /// FluentValidation validator for CreateUserDto
    /// Ensures all user creation requests meet business requirements
    /// </summary>
    public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
    {
        public CreateUserDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .Length(2, 100).WithMessage("Name must be between 2 and 100 characters");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Email must be a valid email address");

            RuleFor(x => x.Phone)
                .MaximumLength(20).WithMessage("Phone number cannot exceed 20 characters")
                .When(x => !string.IsNullOrEmpty(x.Phone));

            RuleFor(x => x.Website)
                .MaximumLength(255).WithMessage("Website URL cannot exceed 255 characters")
                .When(x => !string.IsNullOrEmpty(x.Website));

            RuleFor(x => x.Company)
                .MaximumLength(255).WithMessage("Company name cannot exceed 255 characters")
                .When(x => !string.IsNullOrEmpty(x.Company));
        }
    }

    /// <summary>
    /// FluentValidation validator for UpdateUserDto
    /// Ensures all user update requests meet business requirements
    /// </summary>
    public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
    {
        public UpdateUserDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .Length(2, 100).WithMessage("Name must be between 2 and 100 characters");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Email must be a valid email address");

            RuleFor(x => x.Phone)
                .MaximumLength(20).WithMessage("Phone number cannot exceed 20 characters")
                .When(x => !string.IsNullOrEmpty(x.Phone));

            RuleFor(x => x.Website)
                .MaximumLength(255).WithMessage("Website URL cannot exceed 255 characters")
                .When(x => !string.IsNullOrEmpty(x.Website));

            RuleFor(x => x.Company)
                .MaximumLength(255).WithMessage("Company name cannot exceed 255 characters")
                .When(x => !string.IsNullOrEmpty(x.Company));
        }
    }
}
