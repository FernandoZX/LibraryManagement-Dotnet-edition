using FluentValidation;
using LibraryManagement.Application.Borrowings.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Application.Borrowings.Validators
{
    public class BorrowBookRequestValidator : AbstractValidator<BorrowBookRequest>
    {
        public BorrowBookRequestValidator() => RuleFor(x => x.BookId).NotEmpty();
    }
}
