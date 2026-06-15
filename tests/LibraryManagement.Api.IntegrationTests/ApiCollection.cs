using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Api.IntegrationTests
{
    [CollectionDefinition("Api")]
    public class ApiCollection : ICollectionFixture<LibraryApiFactory> { }
}
