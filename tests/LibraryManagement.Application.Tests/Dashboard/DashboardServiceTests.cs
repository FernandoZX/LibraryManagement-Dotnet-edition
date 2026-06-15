using LibraryManagement.Application.Common.Ports;
using LibraryManagement.Application.Dashboard;
using LibraryManagement.Application.Dashboard.Dtos;
using Microsoft.Extensions.Time.Testing;
using Moq;
using Shouldly;
using Xunit;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Application.Tests.Dashboard
{
    public class DashboardServiceTests
    {
        [Fact]
        public async Task Member_dashboard_delegates_with_current_user_and_clock()
        {
            var repo = new Mock<IDashboardRepository>();
            var clock = new FakeTimeProvider(new DateTimeOffset(2026, 6, 15, 10, 0, 0, TimeSpan.Zero));
            var userId = Guid.NewGuid();
            var expected = new MemberDashboardDto(new List<MemberBorrowingDto>());
            repo.Setup(r => r.GetMemberDashboardAsync(userId, It.IsAny<DateTime>(), default))
                .ReturnsAsync(expected);

            var sut = new DashboardService(repo.Object, clock);
            var result = await sut.GetMemberDashboardAsync(userId);

            result.ShouldBe(expected);
            repo.Verify(r => r.GetMemberDashboardAsync(userId, clock.GetUtcNow().UtcDateTime, default), Times.Once);
        }
    }
}
