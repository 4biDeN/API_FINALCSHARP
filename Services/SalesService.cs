using apifinal.BaseDados.Models;
using apifinal.BaseDados;
using apifinal.Services.DTOs;
using apifinal.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace apifinal.Services
{
    public class SalesService
    {
        private readonly TfDbContext _dbContext;
        private readonly ProductService _productsService;
        private readonly StockLogService _stockLogService;
        private readonly PromotionService _promotionService;
        private readonly IMapper _mapper;

        public SalesService(TfDbContext dbContext, ProductService productsService, StockLogService stockLogService, PromotionService promotionService, IMapper mapper)
        {
            _dbContext = dbContext;
            _productsService = productsService;
            _stockLogService = stockLogService;
            _promotionService = promotionService;
            _mapper = mapper;
        }

        public TbSale GetById(string id)
        {
            return _dbContext.TbSales.FirstOrDefault(s => s.Code == id);
        }

        public async Task<TbSale> CreateSaleAsync(SalesDTO saleDto)
        {
            var product = await _dbContext.TbProducts.FindAsync(saleDto.ProductId);

            if (product.Stock < saleDto.Qty)
                throw new Exception("Estoque insuficiente.");

            product.Stock -= saleDto.Qty;
            await _dbContext.SaveChangesAsync();

            var activePromotions = await _dbContext.TbPromotions
                .Where(p => p.Productid == product.Id && p.Startdate <= DateTime.Now && p.Enddate >= DateTime.Now)
                .OrderBy(p => p.Promotiontype)
                .ToListAsync();

            decimal finalPrice = product.Price;
            decimal discount = 0;

            foreach (var promotion in activePromotions)
            {
                if (promotion.Promotiontype == 0)
                {
                    discount += finalPrice * (promotion.Value / 100);
                }
                else if (promotion.Promotiontype == 1)
                {
                    discount += promotion.Value;
                }
            }

            var sale = _mapper.Map<TbSale>(saleDto);
            sale.Price = finalPrice;
            sale.Discount = discount;
            sale.Code = GenerateUniqueCode();
            sale.Createat = DateTime.Now;

            _stockLogService.LogStockChange(product.Id, saleDto.Qty);

            await _dbContext.TbSales.AddAsync(sale);
            await _dbContext.SaveChangesAsync();

            return sale;
        }

        private string GenerateUniqueCode()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 8);
        }

        public List<SalesReportDTO> GetSalesReportByPeriod(DateTime startDate, DateTime endDate)
        {
            if (startDate == default || endDate == default)
            {
                throw new BadRequestException("As datas de início e fim são obrigatórias.");
            }

            var query = from sale in _dbContext.TbSales
                        join product in _dbContext.TbProducts on sale.Productid equals product.Id
                        where sale.Createat >= startDate && sale.Createat < endDate.AddDays(1)
                        group new { sale, product } by new { sale.Code, sale.Createat } into saleGroup
                        select new SalesReportDTO
                        {
                            SaleCode = saleGroup.Key.Code,
                            SaleDate = saleGroup.Key.Createat,
                            Products = saleGroup.Select(g => new SaleProductDTO
                            {
                                ProductDescription = g.product.Description,
                                Price = g.sale.Price,
                                Quantity = g.sale.Qty
                            }).ToList()
                        };

            var result = query.ToList();

            if (!result.Any())
            {
                throw new NotFoundException("Nenhuma venda encontrada para o período.");
            }

            return result;
        }
    }
}