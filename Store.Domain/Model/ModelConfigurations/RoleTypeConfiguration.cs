﻿using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Store.Domain.Model;

namespace Store.Domain.Model.ModelConfigurations
{
    public class RoleTypeConfiguration : EntityTypeConfiguration<Role>
    {
        public RoleTypeConfiguration()
        {
            HasKey<Guid>(c => c.Id);
            Property(c => c.Id)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(25);
        }
    }
}