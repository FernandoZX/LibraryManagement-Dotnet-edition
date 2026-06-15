using LibraryManagement.Application.Books;
using LibraryManagement.Application.Books.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class BooksController : ApiControllerBase
    {
        private readonly IBookService _books;
        public BooksController(IBookService books) => _books = books;

        [HttpGet]
        public async Task<IActionResult> Search(
            [FromQuery] string? title, [FromQuery] string? author, [FromQuery] string? genre, CancellationToken ct)
            => Ok(await _books.SearchAsync(title, author, genre, ct));

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var result = await _books.GetByIdAsync(id, ct);
            return result.IsSuccess ? Ok(result.Value) : HandleFailure(result.Error!);
        }

        [HttpPost]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> Create(CreateBookRequest request, CancellationToken ct)
        {
            var result = await _books.CreateAsync(request, ct);
            return result.IsSuccess
                ? CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value)
                : HandleFailure(result.Error!);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> Update(Guid id, UpdateBookRequest request, CancellationToken ct)
        {
            var result = await _books.UpdateAsync(id, request, ct);
            return result.IsSuccess ? Ok(result.Value) : HandleFailure(result.Error!);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            var result = await _books.DeleteAsync(id, ct);
            return result.IsSuccess ? NoContent() : HandleFailure(result.Error!);
        }
    }
}
