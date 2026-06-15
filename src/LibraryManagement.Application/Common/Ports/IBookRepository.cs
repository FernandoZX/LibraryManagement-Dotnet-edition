using LibraryManagement.Domain.Books;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Application.Common.Ports
{
    public interface IBookRepository
    {
        Task<Book?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<bool> ExistsByIsbnAsync(string isbn, CancellationToken ct = default);
        Task<IReadOnlyList<Book>> SearchAsync(string? title, string? author, string? genre, CancellationToken ct = default);
        Task AddAsync(Book book, CancellationToken ct = default);
        void Remove(Book book);
    }
}
