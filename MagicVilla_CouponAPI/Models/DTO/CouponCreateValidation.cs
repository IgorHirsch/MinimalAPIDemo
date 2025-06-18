using FluentValidation;

namespace MagicVilla_CouponAPI.Models.DTO
{
    public class CouponCreateValidation : AbstractValidator<CouponCreateDTO>
    {
        public CouponCreateValidation()
        {
            // Name darf nicht leer sein
            RuleFor(model => model.Name)
                .NotEmpty().WithMessage("Name darf nicht leer sein.");

            // Percent muss zwischen 1 und 100 liegen
            RuleFor(model => model.Percent)
                .InclusiveBetween(1, 100)
                .WithMessage("Percent muss zwischen 1 und 100 liegen.");
        }
    }
}