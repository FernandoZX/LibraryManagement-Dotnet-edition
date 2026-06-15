using System.Net;
using System.Net.Http.Json;
using Shouldly;
using Xunit;

namespace LibraryManagement.Api.IntegrationTests
{
    public class AuthEndpointsTests : IntegrationTestBase
    {
        public AuthEndpointsTests(LibraryApiFactory factory) : base(factory) { }

        [Fact]
        public async Task Register_returns_token()
        {
            var resp = await Client.PostAsJsonAsync("/api/auth/register", new
            {
                email = "new@test.com",
                password = "Password123!",
                fullName = "New",
                role = "Member"
            });

            resp.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Login_with_valid_credentials_succeeds()
        {
            await RegisterAndGetTokenAsync("Member", "login@test.com");

            var resp = await Client.PostAsJsonAsync("/api/auth/login", new
            {
                email = "login@test.com",
                password = "Password123!"
            });

            resp.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Login_with_wrong_password_returns_unauthorized()
        {
            await RegisterAndGetTokenAsync("Member", "wrong@test.com");

            var resp = await Client.PostAsJsonAsync("/api/auth/login", new
            {
                email = "wrong@test.com",
                password = "incorrect"
            });

            resp.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }
    }
}
