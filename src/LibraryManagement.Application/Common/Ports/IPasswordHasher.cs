using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Application.Common.Ports
{
    public interface IPasswordHasher
    {
        string Hash(string password);
        bool Verify(string password, string passwordHash);
    }
}
