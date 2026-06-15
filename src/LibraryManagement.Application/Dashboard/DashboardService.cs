using LibraryManagement.Application.Common.Ports;
using LibraryManagement.Application.Dashboard.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Application.Dashboard
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _dashboard;
        private readonly TimeProvider _clock;

        public DashboardService(IDashboardRepository dashboard, TimeProvider clock)
        {
            _dashboard = dashboard;
            _clock = clock;
        }

        public Task<LibrarianDashboardDto> GetLibrarianDashboardAsync(CancellationToken ct = default) =>
            _dashboard.GetLibrarianDashboardAsync(_clock.GetUtcNow().UtcDateTime, ct);

        public Task<MemberDashboardDto> GetMemberDashboardAsync(Guid userId, CancellationToken ct = default) =>
            _dashboard.GetMemberDashboardAsync(userId, _clock.GetUtcNow().UtcDateTime, ct);
    }
}
