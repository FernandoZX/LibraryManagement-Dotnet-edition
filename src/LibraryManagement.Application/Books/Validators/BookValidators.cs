using FluentValidation;
using LibraryManagement.Application.Books.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Application.Books.Validators
{
    public class CreateBookRequestValidator : AbstractValidator<CreateBookRequest>
    {
        public CreateBookRequestValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(300);
            RuleFor(x => x.Author).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Genre).MaximumLength(100);
            RuleFor(x => x.Isbn).NotEmpty().MaximumLength(20);
            RuleFor(x => x.TotalCopies).GreaterThanOrEqualTo(0);
        }
    }

    public class UpdateBookRequestValidator : AbstractValidator<UpdateBookRequest>
    {
        public UpdateBookRequestValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(300);
            RuleFor(x => x.Author).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Genre).MaximumLength(100);
            RuleFor(x => x.Isbn).NotEmpty().MaximumLength(20);
            RuleFor(x => x.TotalCopies).GreaterThanOrEqualTo(0);
        }
    }
}
