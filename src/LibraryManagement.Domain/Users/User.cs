using LibraryManagement.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Domain.Users
{
    public class User : Entity
    {
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public string FullName { get; private set; }
        public UserRole Role { get; private set; }

        private User() { } // requerido por EF Core

        public User(string email, string passwordHash, string fullName, UserRole role)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new DomainException("Email is required.");
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new DomainException("Password hash is required.");

            Email = email.Trim().ToLowerInvariant();
            PasswordHash = passwordHash;
            FullName = fullName?.Trim() ?? string.Empty;
            Role = role;
        }

        public bool IsLibrarian => Role == UserRole.Librarian;
    }
}
