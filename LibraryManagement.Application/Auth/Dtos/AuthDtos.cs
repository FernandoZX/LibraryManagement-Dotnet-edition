using LibraryManagement.Domain.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Application.Auth.Dtos
{
    public record RegisterRequest(string Email, string Password, string FullName, UserRole Role);
    public record LoginRequest(string Email, string Password);
    public record AuthResponse(string Token, string Email, string Role);
}
