using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Domain.Common
{
    public abstract class Entity
    {
        public Guid Id { get; protected set; } = Guid.NewGuid();
    }
}
