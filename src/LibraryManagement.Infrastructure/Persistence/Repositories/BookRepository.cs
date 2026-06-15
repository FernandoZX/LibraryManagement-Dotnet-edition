using LibraryManagement.Application.Common.Ports;
using LibraryManagement.Domain.Books;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Infrastructure.Persistence.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly LibraryDbContext _db;
        public BookRepository(LibraryDbContext db) => _db = db;

        // Tracked: lo usaremos para Update/Delete
        public Task<Book?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
            _db.Books.FirstOrDefaultAsync(b => b.Id == id, ct);

        public Task<bool> ExistsByIsbnAsync(string isbn, CancellationToken ct = default) =>
            _db.Books.AnyAsync(b => b.Isbn == isbn, ct);

        public async Task<IReadOnlyList<Book>> SearchAsync(string? title, string? author, string? genre, CancellationToken ct = default)
        {
            var query = _db.Books.AsNoTracking().AsQueryable();

            if ( !string.IsNullOrWhiteSpace(title) )
                query = query.Where(b => b.Title.Contains(title));
            if ( !string.IsNullOrWhiteSpace(author) )
                query = query.Where(b => b.Author.Contains(author));
            if ( !string.IsNullOrWhiteSpace(genre) )
                query = query.Where(b => b.Genre.Contains(genre));

            return await query.ToListAsync(ct);
        }

        public async Task AddAsync(Book book, CancellationToken ct = default) =>
            await _db.Books.AddAsync(book, ct);

        public void Remove(Book book) => _db.Books.Remove(book);
    }
}
