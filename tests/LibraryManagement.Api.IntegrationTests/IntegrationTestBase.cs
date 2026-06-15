using LibraryManagement.Application.Auth.Dtos;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;

namespace LibraryManagement.Api.IntegrationTests
{
    [Collection("Api")]
    public abstract class IntegrationTestBase : IAsyncLifetime
    {
        protected readonly LibraryApiFactory Factory;
        protected readonly HttpClient Client;

        protected IntegrationTestBase(LibraryApiFactory factory)
        {
            Factory = factory;
            Client = factory.CreateClient();
        }

        // Limpia la base antes de cada test → aislamiento total
        public async Task InitializeAsync() => await Factory.ResetDatabaseAsync();
        public Task DisposeAsync() => Task.CompletedTask;

        // Registra un usuario con el rol pedido y devuelve su token (usa la propia API)
        protected async Task<string> RegisterAndGetTokenAsync(string role, string? email = null)
        {
            email ??= $"{role.ToLower()}-{Guid.NewGuid():N}@test.com";
            var resp = await Client.PostAsJsonAsync("/api/auth/register", new
            {
                email,
                password = "Password123!",
                fullName = $"Test {role}",
                role
            });
            resp.EnsureSuccessStatusCode();
            var auth = await resp.Content.ReadFromJsonAsync<AuthResponse>();
            return auth!.Token;
        }

        protected void Authenticate(string token) =>
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}
