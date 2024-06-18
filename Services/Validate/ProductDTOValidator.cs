using apifinal.Services.DTOs;
using FluentValidation;

namespace apifinal.Services.Validate
{
    public class ProductDTOValidator : ProductBaseValidator<ProductDTO>
    {
        public ProductDTOValidator()
        {
            RuleFor(x => x.Stock)
                .GreaterThanOrEqualTo(0).WithMessage("O Campo Stock não pode ser negativo.");
        }
    }
}
