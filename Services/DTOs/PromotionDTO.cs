﻿using System;

namespace apifinal.Services.DTOs
{
    public class PromotionDTO
    {
        public DateTime Startdate { get; set; }
        public DateTime Enddate { get; set; }
        public int Promotiontype { get; set; }
        public int Productid { get; set; }
        public decimal Value { get; set; }
    }
}
