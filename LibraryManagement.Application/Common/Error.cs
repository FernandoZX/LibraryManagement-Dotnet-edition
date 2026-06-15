using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Application.Common
{
    public enum ErrorType
    {
        Validation,
        NotFound,
        Conflict,
        Unauthorized,
        Forbidden
    }

    public sealed record Error(ErrorType Type, string Message)
    {
        public static Error Validation(string message) => new(ErrorType.Validation, message);
        public static Error NotFound(string message) => new(ErrorType.NotFound, message);
        public static Error Conflict(string message) => new(ErrorType.Conflict, message);
        public static Error Unauthorized(string message) => new(ErrorType.Unauthorized, message);
        public static Error Forbidden(string message) => new(ErrorType.Forbidden, message);
    }
}
