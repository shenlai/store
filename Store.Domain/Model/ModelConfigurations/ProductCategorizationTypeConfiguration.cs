using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Store.Domain.Model;

namespace Store.Domain.Model.ModelConfigurations
{
    public class ProductCategorizationTypeConfiguration: EntityTypeConfiguration<ProductCategorization>
    {
        public ProductCategorizationTypeConfiguration()
        {
            HasKey<Guid>(c => c.Id);
            Property(c => c.Id)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(c => c.ProductId)
                .IsRequired();
            Property(c => c.CategoryId)
                .IsRequired();
        }
    }
}
