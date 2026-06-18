using LibraryManagement.Application.Books.Dtos;
using LibraryManagement.Application.Common;
using LibraryManagement.Application.Common.Ports;
using LibraryManagement.Domain.Books;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Application.Books
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _books;
        private readonly IUnitOfWork _uow;

        public BookService(IBookRepository books, IUnitOfWork uow)
        {
            _books = books;
            _uow = uow;
        }

        public async Task<Result<BookDto>> CreateAsync(CreateBookRequest request, CancellationToken ct = default)
        {
            if (await _books.ExistsByIsbnAsync(request.Isbn, ct))
                return Result.Failure<BookDto>(Error.Conflict("A book with this ISBN already exists."));

            var book = new Book(request.Title, request.Author, request.Genre, request.Isbn, request.TotalCopies);
            await _books.AddAsync(book, ct);
            await _uow.SaveChangesAsync(ct);

            return Result.Success(book.ToDto());
        }

        public async Task<Result<BookDto>> UpdateAsync(Guid id, UpdateBookRequest request, CancellationToken ct = default)
        {
            var book = await _books.GetByIdAsync(id, ct);


            var recordFound = RecordExtensions.EnsureFound(book, "Book");
            if (!recordFound.IsSuccess)
                return Result.Failure<BookDto>(recordFound.Error);

            if (await _books.ExistsByIsbnAsync(request.Isbn, ct))
                return Result.Failure<BookDto>(Error.Conflict("A book with this ISBN already exists."));


            book.UpdateDetails(request.Title, request.Author, request.Genre, request.Isbn, request.TotalCopies);
            await _uow.SaveChangesAsync(ct);

            return Result.Success(book.ToDto());
        }

        public async Task<Result> DeleteAsync(Guid id, CancellationToken ct = default)
        {
            var book = await _books.GetByIdAsync(id, ct);
            var recordFound = RecordExtensions.EnsureFound(book, "Book");
            if (!recordFound.IsSuccess)
                return Result.Failure(recordFound.Error);

            _books.Remove(book);
            await _uow.SaveChangesAsync(ct);
            return Result.Success();
        }

        public async Task<Result<BookDto>> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var book = await _books.GetByIdAsync(id, ct);
            return book is null
                ? Result.Failure<BookDto>(Error.NotFound("Book not found."))
                : Result.Success(book.ToDto());
        }

        public async Task<IReadOnlyList<BookDto>> SearchAsync(string? title, string? author, string? genre, CancellationToken ct = default)
        {
            var books = await _books.SearchAsync(title, author, genre, ct);
            return books.Select(b => b.ToDto()).ToList();
        }
    }
}
