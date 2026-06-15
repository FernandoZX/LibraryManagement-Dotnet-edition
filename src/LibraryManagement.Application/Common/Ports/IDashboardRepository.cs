using LibraryManagement.Application.Dashboard.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Application.Common.Ports
{
    public interface IDashboardRepository
    {
        Task<LibrarianDashboardDto> GetLibrarianDashboardAsync(DateTime now, CancellationToken ct = default);
        Task<MemberDashboardDto> GetMemberDashboardAsync(Guid userId, DateTime now, CancellationToken ct = default);
    }
}
