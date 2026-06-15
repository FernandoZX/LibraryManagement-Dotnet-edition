using LibraryManagement.Application.Borrowings;
using LibraryManagement.Application.Borrowings.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class BorrowingsController : ApiControllerBase
    {
        private readonly IBorrowingService _borrowings;
        public BorrowingsController(IBorrowingService borrowings) => _borrowings = borrowings;

        [HttpPost]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Borrow(BorrowBookRequest request, CancellationToken ct)
        {
            var result = await _borrowings.BorrowAsync(CurrentUserId, request, ct);
            return result.IsSuccess ? Ok(result.Value) : HandleFailure(result.Error!);
        }

        [HttpPut("{id:guid}/return")]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> Return(Guid id, CancellationToken ct)
        {
            var result = await _borrowings.ReturnAsync(id, ct);
            return result.IsSuccess ? Ok(result.Value) : HandleFailure(result.Error!);
        }
    }
}
