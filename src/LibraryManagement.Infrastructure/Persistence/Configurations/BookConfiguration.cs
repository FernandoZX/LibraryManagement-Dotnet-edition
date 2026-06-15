using LibraryManagement.Domain.Books;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Infrastructure.Persistence.Configurations
{
    public class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.ToTable("Books");
            builder.HasKey(b => b.Id);

            builder.Property(b => b.Title).IsRequired().HasMaxLength(300);
            builder.Property(b => b.Author).IsRequired().HasMaxLength(200);
            builder.Property(b => b.Genre).HasMaxLength(100);
            builder.Property(b => b.Isbn).IsRequired().HasMaxLength(20);
            builder.HasIndex(b => b.Isbn).IsUnique();

            builder.Property(b => b.TotalCopies).IsRequired();
            builder.Property(b => b.AvailableCopies).IsRequired();
        }
    }
}
