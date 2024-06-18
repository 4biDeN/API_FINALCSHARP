using FluentValidation;
using apifinal.Services.DTOs;
using apifinal.BaseDados;
using System.Linq;
using System;

namespace apifinal.Services.Validate
{
    public class SaleDTOValidator : AbstractValidator<SalesDTO>
    {
        private readonly TfDbContext _dbContext;

        public SaleDTOValidator(TfDbContext dbContext)
        {
            _dbContext = dbContext;

            RuleFor(sale => sale.ProductId)
                .NotEmpty().WithMessage("O ID do produto é obrigatório.")
                .Must(ProductExists).WithMessage("O produto não está cadastrado.");

            RuleFor(sale => sale.Qty)
                .NotEmpty().WithMessage("A quantidade é obrigatória.")
                .GreaterThan(0).WithMessage("A quantidade deve ser maior que zero.")
                .Must((sale, quantity) => HaveStockAvailable(sale.ProductId, quantity))
                .WithMessage("Estoque insuficiente para a venda.");
        }

        private bool ProductExists(int productId)
        {
            return _dbContext.TbProducts.Any(p => p.Id == productId);
        }

        private bool HaveStockAvailable(int productId, int quantity)
        {
            var product = _dbContext.TbProducts.FirstOrDefault(p => p.Id == productId);
            return product != null && product.Stock >= quantity;
        }
    }
}
