using LibraryManagement.Application.Auth;
using LibraryManagement.Application.Auth.Dtos;
using LibraryManagement.Application.Common;
using LibraryManagement.Application.Common.Ports;
using LibraryManagement.Domain.Users;
using Moq;
using Shouldly;
using Xunit;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Application.Tests.Auth
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _users = new();
        private readonly Mock<IPasswordHasher> _hasher = new();
        private readonly Mock<IJwtTokenGenerator> _tokens = new();
        private readonly Mock<IUnitOfWork> _uow = new();

        private AuthService CreateSut() => new(_users.Object, _hasher.Object, _tokens.Object, _uow.Object);

        [Fact]
        public async Task Register_with_existing_email_returns_conflict()
        {
            _users.Setup(r => r.ExistsByEmailAsync(It.IsAny<string>(), default)).ReturnsAsync(true);

            var result = await CreateSut().RegisterAsync(
            new RegisterRequest("a@a.com", "password123", "Ana", UserRole.Member));

            result.IsSuccess.ShouldBeFalse();
            result.Error!.Type.ShouldBe(ErrorType.Conflict);
        }

        [Fact]
        public async Task Login_with_wrong_password_returns_unauthorized()
        {
            var user = new User("a@a.com", "hashed", "Ana", UserRole.Member);
            _users.Setup(r => r.GetByEmailAsync("a@a.com", default)).ReturnsAsync(user);
            _hasher.Setup(h => h.Verify("wrong", "hashed")).Returns(false);

            var result = await CreateSut().LoginAsync(new LoginRequest("a@a.com", "wrong"));

            result.IsSuccess.ShouldBeFalse();
            result.Error!.Type.ShouldBe(ErrorType.Unauthorized);
        }

        [Fact]
        public async Task Login_with_valid_credentials_returns_token()
        {
            var user = new User("a@a.com", "hashed", "Ana", UserRole.Member);
            _users.Setup(r => r.GetByEmailAsync("a@a.com", default)).ReturnsAsync(user);
            _hasher.Setup(h => h.Verify("password123", "hashed")).Returns(true);
            _tokens.Setup(t => t.GenerateToken(user)).Returns("jwt-token");

            var result = await CreateSut().LoginAsync(new LoginRequest("a@a.com", "password123"));

            result.IsSuccess.ShouldBeTrue();
            result.Value!.Token.ShouldBe("jwt-token");
        }
    }
}
