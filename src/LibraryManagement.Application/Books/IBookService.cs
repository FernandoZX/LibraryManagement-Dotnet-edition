using LibraryManagement.Application.Books.Dtos;
using LibraryManagement.Application.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Application.Books
{
    public interface IBookService
    {
        Task<Result<BookDto>> CreateAsync(CreateBookRequest request, CancellationToken ct = default);
        Task<Result<BookDto>> UpdateAsync(Guid id, UpdateBookRequest request, CancellationToken ct = default);
        Task<Result> DeleteAsync(Guid id, CancellationToken ct = default);
        Task<Result<BookDto>> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<IReadOnlyList<BookDto>> SearchAsync(string? title, string? author, string? genre, CancellationToken ct = default);
    }
}
