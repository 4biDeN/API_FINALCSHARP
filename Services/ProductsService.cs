using apifinal.BaseDados;
using apifinal.BaseDados.Models;
using apifinal.Services.DTOs;
using apifinal.Services.Exceptions;
using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace apifinal.Services
{
    public class ProductService
    {
        private readonly TfDbContext _dbContext;
        private readonly IValidator<ProductDTO> _productDtoValidator;
        private readonly IValidator<ProductUpdateDTO> _productUpdateDtoValidator;
        private readonly IMapper _mapper;
        private readonly StockLogService _stockLogService;

        public ProductService(TfDbContext dbContext, IValidator<ProductDTO> productDtoValidator, IValidator<ProductUpdateDTO> productUpdateDtoValidator, IMapper mapper, StockLogService stockLogService)
        {
            _dbContext = dbContext;
            _productDtoValidator = productDtoValidator;
            _productUpdateDtoValidator = productUpdateDtoValidator;
            _mapper = mapper;
            _stockLogService = stockLogService;
        }

        public TbProduct InsertProduct(ProductDTO productDto)
        {
            var validationResult = _productDtoValidator.Validate(productDto);
            if (!validationResult.IsValid)
            {
                throw new InvalidEntityException(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
            }

            var entity = _mapper.Map<TbProduct>(productDto);

            _dbContext.Add(entity);
            _dbContext.SaveChanges();

            return entity;
        }

        public TbProduct GetByBarcode(string barcode)
        {
            var existingEntity = _dbContext.TbProducts.FirstOrDefault(c => c.Barcode == barcode);
            if (existingEntity == null)
            {
                throw new NotFoundException("Nenhum Produto Encontrado");
            }
            return existingEntity;
        }
        public TbProduct GetById(int id)
        {
            var existingEntity = _dbContext.TbProducts.FirstOrDefault(c => c.Id == id);
            if (existingEntity == null)
            {
                throw new NotFoundException("Nenhum Produto Encontrado");
            }
            return existingEntity;
        }
        public IEnumerable<ProductBaseDTO> GetProductsByDescription(string description)
        {
            return _dbContext.TbProducts
                             .Where(p => EF.Functions.Like(p.Description.ToLower(), $"%{description.ToLower()}%"))
                             .Select(p => _mapper.Map<ProductBaseDTO>(p))
                             .ToList();
        }
        public TbProduct UpdateProduct(string barcode, ProductUpdateDTO productUpdateDto)
        {
            var validationResult = _productUpdateDtoValidator.Validate(productUpdateDto);
            if (!validationResult.IsValid)
            {
                throw new InvalidEntityException(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
            }

            var existingProduct = _dbContext.TbProducts.FirstOrDefault(p => p.Barcode == barcode);
            if (existingProduct == null)
            {
                throw new NotFoundException("Produto não encontrado.");
            }

            _mapper.Map(productUpdateDto, existingProduct);

            _dbContext.TbProducts.Update(existingProduct);
            _dbContext.SaveChanges();

            return existingProduct;
        }

        public TbProduct UpdateStock(string barcode, int quantity)
        {
            var existingProduct = _dbContext.TbProducts.FirstOrDefault(p => p.Barcode == barcode);
            if (existingProduct == null)
            {
                throw new NotFoundException("Produto Não Encontrado.");
            }

            existingProduct.Stock += quantity;

            _stockLogService.LogStockChange(existingProduct.Id, quantity);

            _dbContext.TbProducts.Update(existingProduct);
            _dbContext.SaveChanges();

            return existingProduct;
        }
    }
}
