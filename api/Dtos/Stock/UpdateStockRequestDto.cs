using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Stock
{
    public class UpdateStockRequestDto
    {
        [Required]
        [MaxLength(10, ErrorMessage = "Symbol must be at most 10 characters")]
        public string Symbol { get; set; } = string.Empty;

        [Required]
        [MaxLength(20, ErrorMessage = "Company must be at most 10 characters")]
        public string CompanyName { get; set; } = string.Empty;

        [Required]
        [Range(1, 10000000000)]
        public decimal Price { get; set; }

        [Required]
        [Range(0.001, 100)]
        public decimal LastDiv { get; set; }

        [Required]
        [MaxLength(10, ErrorMessage = "Industry must be at most 10 characters")]
        public string Industry { get; set; } = string.Empty;

        [Range(1, 10000000000)]
        public long MarketCap { get; set; }
    }
}
