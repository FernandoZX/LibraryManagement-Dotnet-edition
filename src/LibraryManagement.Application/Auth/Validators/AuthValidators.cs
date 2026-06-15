using FluentValidation;
using LibraryManagement.Application.Auth.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Application.Auth.Validators
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
            RuleFor(x => x.FullName).NotEmpty();
            RuleFor(x => x.Role).IsInEnum();
        }
    }

    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty();
        }
    }
}
