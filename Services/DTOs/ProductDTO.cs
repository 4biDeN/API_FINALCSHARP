using apifinal.BaseDados.Models;
using System.Collections.Generic;
using System;

namespace apifinal.Services.DTOs
{
    public class ProductDTO : ProductBaseDTO
    {
        /// <summary>
        /// Quantidade a ser incrementada ou decrementada no estoque
        /// </summary>
        public int Stock { get; set; }
    }
}
