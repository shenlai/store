using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Store.Domain.Model.ModelConfigurations
{
    public class ProductTypeConfiguration : EntityTypeConfiguration<Product>
    {
        public ProductTypeConfiguration() 
        {
            HasKey<Guid>(l => l.Id);

            Property(p => p.Id).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);  //ID自增长
            Property(p => p.Description)
                .IsRequired();
            Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(40);
            Property(p => p.ImageUrl)
                .HasMaxLength(255);

        }
    }
}
