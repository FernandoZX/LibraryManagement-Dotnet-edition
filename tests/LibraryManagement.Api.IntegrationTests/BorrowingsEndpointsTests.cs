using System.Net;
using System.Net.Http.Json;
using Shouldly;
using Xunit;

namespace LibraryManagement.Api.IntegrationTests
{
    public class BorrowingsEndpointsTests : IntegrationTestBase
    {
        public BorrowingsEndpointsTests(LibraryApiFactory factory) : base(factory) { }

        // Crea un libro como librarian y devuelve su Id
        private async Task<Guid> SeedBookAsync()
        {
            Authenticate(await RegisterAndGetTokenAsync("Librarian"));
            var resp = await Client.PostAsJsonAsync("/api/books", new
            {
                title = "DDD",
                author = "Evans",
                genre = "Software",
                isbn = "9780321125217",
                totalCopies = 1
            });
            var book = await resp.Content.ReadFromJsonAsync<BookResponse>();
            return book!.Id;
        }

        [Fact]
        public async Task Member_can_borrow_available_book()
        {
            var bookId = await SeedBookAsync();
            Authenticate(await RegisterAndGetTokenAsync("Member"));

            var resp = await Client.PostAsJsonAsync("/api/borrowings", new { bookId });

            resp.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Member_cannot_borrow_same_book_twice()
        {
            var bookId = await SeedBookAsync();
            Authenticate(await RegisterAndGetTokenAsync("Member", "twice@test.com"));

            await Client.PostAsJsonAsync("/api/borrowings", new { bookId });        // primero
            var second = await Client.PostAsJsonAsync("/api/borrowings", new { bookId }); // duplicado

            second.StatusCode.ShouldBe(HttpStatusCode.Conflict); // 409
        }

        private record BookResponse(Guid Id, string Title, string Author, string Genre,
            string Isbn, int TotalCopies, int AvailableCopies);
    }
}
