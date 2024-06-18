using System;

namespace apifinal.Services.DTOs
{
    public class StockLogDTO
    {
        public DateTime CreatedAt { get; set; }
        public string Barcode { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
    }
}
