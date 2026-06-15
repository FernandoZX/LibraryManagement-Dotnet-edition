using LibraryManagement.Application.Auth.Dtos;
using LibraryManagement.Application.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Application.Auth
{
    public interface IAuthService
    {
        Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken ct = default);
        Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken ct = default);
    }
}
