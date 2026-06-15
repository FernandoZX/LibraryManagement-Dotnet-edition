using LibraryManagement.Application.Borrowings.Dtos;
using LibraryManagement.Application.Common;
using LibraryManagement.Application.Common.Ports;
using LibraryManagement.Domain.Borrowings;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Application.Borrowings
{
    public class BorrowingService : IBorrowingService
    {
        private readonly IBorrowingRepository _borrowings;
        private readonly IBookRepository _books;
        private readonly IUnitOfWork _uow;
        private readonly TimeProvider _clock;

        public BorrowingService(
            IBorrowingRepository borrowings,
            IBookRepository books,
            IUnitOfWork uow,
            TimeProvider clock)
        {
            _borrowings = borrowings;
            _books = books;
            _uow = uow;
            _clock = clock;
        }

        public async Task<Result<BorrowingDto>> BorrowAsync(Guid userId, BorrowBookRequest request, CancellationToken ct = default)
        {
            var book = await _books.GetByIdAsync(request.BookId, ct);
            if ( book is null )
                return Result.Failure<BorrowingDto>(Error.NotFound("Book not found."));

            if ( !book.IsAvailable )
                return Result.Failure<BorrowingDto>(Error.Conflict("No copies available to borrow."));

            if ( await _borrowings.HasActiveBorrowingAsync(userId, book.Id, ct) )
                return Result.Failure<BorrowingDto>(Error.Conflict("You already have this book borrowed."));

            var now = _clock.GetUtcNow().UtcDateTime;

            book.Borrow();                                   // descuenta copia (invariante en el dominio)
            var borrowing = new Borrowing(book.Id, userId, now);
            await _borrowings.AddAsync(borrowing, ct);

            await _uow.SaveChangesAsync(ct);                 // copia descontada + préstamo creado, atómico

            return Result.Success(borrowing.ToDto(book.Title));
        }

        public async Task<Result<BorrowingDto>> ReturnAsync(Guid borrowingId, CancellationToken ct = default)
        {
            var borrowing = await _borrowings.GetByIdAsync(borrowingId, ct);
            if ( borrowing is null )
                return Result.Failure<BorrowingDto>(Error.NotFound("Borrowing not found."));

            if ( borrowing.IsReturned )
                return Result.Failure<BorrowingDto>(Error.Conflict("This borrowing was already returned."));

            var book = await _books.GetByIdAsync(borrowing.BookId, ct);
            if ( book is null )
                return Result.Failure<BorrowingDto>(Error.NotFound("Book not found."));

            var now = _clock.GetUtcNow().UtcDateTime;

            borrowing.MarkAsReturned(now);
            book.Return();                                   // devuelve la copia al inventario

            await _uow.SaveChangesAsync(ct);

            return Result.Success(borrowing.ToDto(book.Title));
        }
    }
}
