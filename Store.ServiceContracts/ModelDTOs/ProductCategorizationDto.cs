using System;

namespace Store.ServiceContracts.ModelDTOs
{
    public class ProductCategorizationDto
    {
        public string Id { get; set; }

        public Guid CategoryId { get; set; }

        public Guid ProductId { get; set; }
    }
}