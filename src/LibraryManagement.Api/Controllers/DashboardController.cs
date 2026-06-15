using LibraryManagement.Application.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController : ApiControllerBase
    {
        private readonly IDashboardService _dashboard;
        public DashboardController(IDashboardService dashboard) => _dashboard = dashboard;

        [HttpGet("librarian")]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> Librarian(CancellationToken ct) =>
            Ok(await _dashboard.GetLibrarianDashboardAsync(ct));

        [HttpGet("member")]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Member(CancellationToken ct) =>
            Ok(await _dashboard.GetMemberDashboardAsync(CurrentUserId, ct));
    }
}
