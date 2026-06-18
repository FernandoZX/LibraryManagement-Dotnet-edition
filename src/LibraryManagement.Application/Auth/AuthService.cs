using LibraryManagement.Application.Auth.Dtos;
using LibraryManagement.Application.Common;
using LibraryManagement.Application.Common.Ports;
using LibraryManagement.Domain.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Application.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _users;
        private readonly IPasswordHasher _hasher;
        private readonly IJwtTokenGenerator _tokens;
        private readonly IUnitOfWork _uow;

        public AuthService(
            IUserRepository users,
            IPasswordHasher hasher,
            IJwtTokenGenerator tokens,
            IUnitOfWork uow)
        {
            _users = users;
            _hasher = hasher;
            _tokens = tokens;
            _uow = uow;
        }

        public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
        {
            var email = request.Email.Trim().ToLowerInvariant();

            if (await _users.ExistsByEmailAsync(email, ct))
                return Result.Failure<AuthResponse>(Error.Conflict("Email is already registered."));

            var hash = _hasher.Hash(request.Password);
            var user = new User(email, hash, request.FullName, request.Role);

            await _users.AddAsync(user, ct);
            await _uow.SaveChangesAsync(ct);

            var token = _tokens.GenerateToken(user);
            return Result.Success(new AuthResponse(token, user.Email, user.Role.ToString()));
        }

        public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken ct = default)
        {
            var email = request.Email.Trim().ToLowerInvariant();
            var user = await _users.GetByEmailAsync(email, ct);

            if (user is null || !_hasher.Verify(request.Password, user.PasswordHash))
                return Result.Failure<AuthResponse>(Error.Unauthorized("Invalid credentials."));

            var token = _tokens.GenerateToken(user);
            return Result.Success(new AuthResponse(token, user.Email, user.Role.ToString()));
        }
    }
}
