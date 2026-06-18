using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Application.Common
{
    public static class RecordExtensions
    {
        public static Result EnsureFound<T>(T? value, string entityName = "Record")
        where T : class => value is null
            ? Result.Failure(Error.NotFound($"{entityName} not found."))
            : Result.Success();

    }
}
