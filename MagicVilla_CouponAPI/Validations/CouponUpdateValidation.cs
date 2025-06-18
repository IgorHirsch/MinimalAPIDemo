using FluentValidation;
using MagicVilla_CouponAPI.Models.DTO;

namespace MagicVilla_CouponAPI.Validations
{
    public class CouponUpdateValidation : AbstractValidator<CouponUpdateDTO>
    {
        public CouponUpdateValidation()
        {

            RuleFor(model => model.Id)
                .GreaterThan(0).WithMessage("Id muss größer als 0 sein.");

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