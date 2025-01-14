using FluentValidation;
using MinimalAPIsMovies.DTOs;

namespace MinimalAPIsMovies.Validations
{
    public class CreateMovieDtoValidator:AbstractValidator<CreateMovieDto>
    {
        public CreateMovieDtoValidator()
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage(ValidationUtilities.NonEmptyMessage).MaximumLength(250).WithMessage(ValidationUtilities.MaximumLengthMessage);
        }
    }
}
