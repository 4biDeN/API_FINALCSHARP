using apifinal.Services.DTOs;
using FluentValidation;

namespace apifinal.Services.Validate
{
    public class ProductStockUpdateValidator : AbstractValidator<ProductStockUpdateDTO>
    {
        public ProductStockUpdateValidator()
        {
            RuleFor(x => x.Quantity).NotNull().WithMessage("Quantidade não pode ser nula.");
        }
    }
}
