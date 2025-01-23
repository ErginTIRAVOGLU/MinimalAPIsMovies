using FluentValidation;
using MinimalAPIsMovies.DTOs;

namespace MinimalAPIsMovies.Validations
{
    public class EditClaimDtoValidator : AbstractValidator<EditClaimDto>
    {
        public EditClaimDtoValidator()
        {
            RuleFor(RuleFor => RuleFor.Email)
                .EmailAddress()
                .WithMessage(ValidationUtilities.EmailAddressMessage);
        }
    }
}
