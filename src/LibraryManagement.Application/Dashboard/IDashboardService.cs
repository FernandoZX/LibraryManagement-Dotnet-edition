using LibraryManagement.Application.Dashboard.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Application.Dashboard
{
    public interface IDashboardService
    {
        Task<LibrarianDashboardDto> GetLibrarianDashboardAsync(CancellationToken ct = default);
        Task<MemberDashboardDto> GetMemberDashboardAsync(Guid userId, CancellationToken ct = default);
    }
}
