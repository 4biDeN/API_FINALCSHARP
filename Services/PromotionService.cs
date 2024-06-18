using apifinal.BaseDados;
using apifinal.BaseDados.Models;
using apifinal.Services.DTOs;
using apifinal.Services.Exceptions;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace apifinal.Services
{
    public class PromotionService
    {
        private readonly TfDbContext _dbContext;
        private readonly IMapper _mapper;

        public PromotionService(TfDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public TbPromotion InsertPromotion(PromotionDTO promotionDto)
        {
            var entity = _mapper.Map<TbPromotion>(promotionDto);
            _dbContext.TbPromotions.Add(entity);
            _dbContext.SaveChanges();
            return entity;
        }

        public TbPromotion UpdatePromotion(int id, PromotionDTO promotionDto)
        {
            var existingPromotion = _dbContext.TbPromotions.Find(id);
            if (existingPromotion == null)
            {
                throw new NotFoundException("Promoção não encontrada.");
            }

            _mapper.Map(promotionDto, existingPromotion);
            _dbContext.SaveChanges();
            return existingPromotion;
        }

        public IEnumerable<TbPromotion> GetPromotionsByProductAndPeriod(int productId, DateTime startDate, DateTime endDate)
        {
            return _dbContext.TbPromotions
                .Where(p => p.Productid == productId && p.Enddate >= startDate && p.Startdate <= endDate)
                .ToList();
        }

        public decimal GetDiscountForProduct(int productId, DateTime currentDate)
        {
            var promotion = _dbContext.TbPromotions
                                    .FirstOrDefault(p => p.Productid == productId
                                                     && p.Startdate <= currentDate
                                                     && p.Enddate >= currentDate);

            return promotion?.Value ?? 0m;
        }
        public List<TbPromotion> GetActivePromotions(int productId)
        {
            var currentDate = DateTime.Now;

            return _dbContext.TbPromotions
                .Where(p => p.Productid == productId
                            && p.Startdate <= DateTime.Now
                            && p.Enddate >= DateTime.Now)
                .OrderBy(p => p.Promotiontype)
                .ToList();
        }
    }
}
