using LibraryManagement.Application.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LibraryManagement.Api.Controllers
{

    [ApiController]
    public class ApiControllerBase : ControllerBase
    {
        protected Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        protected IActionResult HandleFailure(Error error) => error.Type switch
        {
            ErrorType.Validation => BadRequest(new { error.Message }),
            ErrorType.NotFound => NotFound(new { error.Message }),
            ErrorType.Conflict => Conflict(new { error.Message }),
            ErrorType.Unauthorized => Unauthorized(new { error.Message }),
            ErrorType.Forbidden => StatusCode(StatusCodes.Status403Forbidden, new { error.Message }),
            _ => StatusCode(StatusCodes.Status500InternalServerError, new { error.Message })
        };
    }
}
