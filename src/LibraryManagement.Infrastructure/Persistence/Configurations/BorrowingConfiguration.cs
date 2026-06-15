using LibraryManagement.Domain.Books;
using LibraryManagement.Domain.Borrowings;
using LibraryManagement.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Infrastructure.Persistence.Configurations
{
    public class BorrowingConfiguration : IEntityTypeConfiguration<Borrowing>
    {
        public void Configure(EntityTypeBuilder<Borrowing> builder)
        {
            builder.ToTable("Borrowings");
            builder.HasKey(b => b.Id);

            builder.Property(b => b.BorrowedAt).IsRequired();
            builder.Property(b => b.DueAt).IsRequired();
            builder.Property(b => b.ReturnedAt);

            builder.HasOne<Book>().WithMany()
                   .HasForeignKey(b => b.BookId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne<User>().WithMany()
                   .HasForeignKey(b => b.UserId).OnDelete(DeleteBehavior.Restrict);

            // Refuerza a nivel DB la regla "no puede pedir el mismo libro dos veces":
            // un solo préstamo ACTIVO (ReturnedAt NULL) por usuario+libro.
            builder.HasIndex(b => new { b.UserId, b.BookId })
                   .HasFilter("[ReturnedAt] IS NULL")
                   .IsUnique();
        }
    }
}
