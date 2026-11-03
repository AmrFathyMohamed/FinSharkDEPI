using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FinShark.Dtos.Stock
{
    public class UpdateStockRequestDto : StockRequestDto
    {
        [Range(1, 5000000000)]
        public long MarketCap { get; set; }
    }
}