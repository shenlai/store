using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Domain.Model.ModelConfigurations
{
    public class OrderTypeConfiguration : EntityTypeConfiguration<Order>
    {
        public OrderTypeConfiguration()
        {
            HasKey<Guid>(s => s.Id);
            Property(s => s.Id)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);//Id 自增长
            Ignore(p => p.Subtotal);
        }
    }
}
