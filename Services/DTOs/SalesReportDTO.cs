using System.Collections.Generic;
using System;

namespace apifinal.Services.DTOs
{
    public class SalesReportDTO
    {
        public string SaleCode { get; set; }
        public DateTime SaleDate { get; set; }
        public List<SaleProductDTO> Products { get; set; }
    }
}