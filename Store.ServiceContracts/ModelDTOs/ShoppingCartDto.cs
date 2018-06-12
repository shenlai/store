﻿using System.Collections.Generic;

namespace Store.ServiceContracts.ModelDTOs
{
    public class ShoppingCartDto
    {
        public string Id { get; set; }

        public string CustomerId { get; set; }

        public IList<ShoppingCartItemDto> Items { get; set; }

        public decimal? Subtotal { get; set; }
    }
}