using apifinal.BaseDados;
using apifinal.BaseDados.Models;
using apifinal.Services.DTOs;
using apifinal.Services.Exceptions;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace apifinal.Services
{
    public class StockLogService
    {
        private readonly TfDbContext _dbContext;
        private readonly IMapper _mapper;

        public StockLogService(TfDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public void LogStockChange(int productId, int changeAmount)
        {
            var log = new TbStockLog
            {
                Productid = productId,
                Qty = changeAmount,
                Createdat = DateTime.UtcNow
            };

            _dbContext.TbStockLogs.Add(log);
            _dbContext.SaveChanges();
        }

        public IEnumerable<StockLogDTO> GetStockLogsByProduct(string barcode)
        {
            var logs = _dbContext.TbStockLogs
                .Include(log => log.Product)
                .Where(log => log.Product.Barcode == barcode)
                .Select(log => new StockLogDTO
                {
                    CreatedAt = log.Createdat,
                    Barcode = log.Product.Barcode,
                    Description = log.Product.Description,
                    Quantity = log.Qty
                })
                .ToList();

            if (!logs.Any())
            {
                throw new NotFoundException("Nenhum log de estoque encontrado para o produto especificado.");
            }

            return logs;
        }
    }
}