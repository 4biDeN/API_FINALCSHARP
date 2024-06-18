using System;
using System.Text.Json.Serialization;

namespace apifinal.Services.DTOs
{
    public class SalesDTO
    {

        public int ProductId { get; set; }
        public int Qty { get; set; }
        [JsonIgnore]
        public DateTime CreateAt { get; set; }

    }
}
