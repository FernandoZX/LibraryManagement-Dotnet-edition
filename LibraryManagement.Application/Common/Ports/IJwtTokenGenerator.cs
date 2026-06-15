using LibraryManagement.Domain.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Application.Common.Ports
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user);
    }
}
