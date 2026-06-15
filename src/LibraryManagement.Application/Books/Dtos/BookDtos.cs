using LibraryManagement.Domain.Books;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Application.Books.Dtos
{
    public record CreateBookRequest(string Title, string Author, string Genre, string Isbn, int TotalCopies);
    public record UpdateBookRequest(string Title, string Author, string Genre, string Isbn, int TotalCopies);
    public record BookDto(Guid Id, string Title, string Author, string Genre, string Isbn, int TotalCopies, int AvailableCopies);

    public static class BookMappings
    {
        public static BookDto ToDto(this Book b) =>
            new(b.Id, b.Title, b.Author, b.Genre, b.Isbn, b.TotalCopies, b.AvailableCopies);
    }
}
