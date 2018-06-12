using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Domain.Model.ModelConfigurations
{
    public class OrderItemTypeConfiguration:EntityTypeConfiguration<OrderItem>
    {
        public OrderItemTypeConfiguration()
        {
            HasKey<Guid>(s => s.Id);
            Property(p => p.Id).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            HasRequired(p => p.Order)
                .WithMany(p => p.OrderItems);
            Ignore(p => p.ItemAmout);

        }
    }
}
