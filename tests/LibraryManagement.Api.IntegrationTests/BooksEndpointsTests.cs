using Shouldly;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace LibraryManagement.Api.IntegrationTests
{
    public class BooksEndpointsTests : IntegrationTestBase
    {
        public BooksEndpointsTests(LibraryApiFactory factory) : base(factory) { }

        private static object SampleBook() => new
        {
            title = "Clean Architecture",
            author = "Martin",
            genre = "Software",
            isbn = "9780134494166",
            totalCopies = 3
        };

        [Fact]
        public async Task Member_cannot_create_book()
        {
            Authenticate(await RegisterAndGetTokenAsync("Member"));

            var resp = await Client.PostAsJsonAsync("/api/books", SampleBook());

            resp.StatusCode.ShouldBe(HttpStatusCode.Forbidden); // 403
        }

        [Fact]
        public async Task Librarian_can_create_book()
        {
            Authenticate(await RegisterAndGetTokenAsync("Librarian"));

            var resp = await Client.PostAsJsonAsync("/api/books", SampleBook());

            resp.StatusCode.ShouldBe(HttpStatusCode.Created); // 201
        }

        [Fact]
        public async Task Search_by_genre_returns_matching_books()
        {
            Authenticate(await RegisterAndGetTokenAsync("Librarian"));
            await Client.PostAsJsonAsync("/api/books", SampleBook());

            var resp = await Client.GetAsync("/api/books?genre=Software");
            var books = await resp.Content.ReadFromJsonAsync<List<BookResponse>>();

            resp.StatusCode.ShouldBe(HttpStatusCode.OK);
            books!.ShouldContain(b => b.Genre == "Software");
        }

        private record BookResponse(Guid Id, string Title, string Author, string Genre,
            string Isbn, int TotalCopies, int AvailableCopies);
    }
}
