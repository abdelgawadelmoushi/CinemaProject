using FluentValidation;

namespace CinemaProject.Validations
{
    public class CategoryValidator : AbstractValidator<Category>
    {

        public CategoryValidator()
        {
            RuleFor(e => e.Name)
                .NotEmpty()
                .Length(5, 30)
                .WithMessage("The filed {PropertyName}, Length Must be {MinLength} and {MaxLength}");
        }

    }
}
