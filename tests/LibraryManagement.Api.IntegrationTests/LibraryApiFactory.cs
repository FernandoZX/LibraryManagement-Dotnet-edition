using System.Data.Common;
using LibraryManagement.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Respawn;
using Xunit;

namespace LibraryManagement.Api.IntegrationTests
{
    public class LibraryApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        // Base de tests aparte para no pisar la de desarrollo (LibraryDb).
        // Ajusta el Server si tu instancia local no es "localhost".
        private const string ConnectionString =
        "Server=localhost;Database=LibraryDb_Tests;Trusted_Connection=True;TrustServerCertificate=True;";

        private DbConnection _connection = null!;
        private Respawner _respawner = null!;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing"); // no corre el seeder ni Scalar

            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<DbContextOptions<LibraryDbContext>>();
                services.AddDbContext<LibraryDbContext>(o => o.UseSqlServer(ConnectionString));
            });
        }

        public async Task InitializeAsync()
        {
            // Crea LibraryDb_Tests (si no existe) y aplica las migrations
            using (var scope = Services.CreateScope())
            {
                var ctx = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
                await ctx.Database.MigrateAsync();
            }

            // Respawn limpia las tablas entre tests, sin tocar el historial de migrations
            _connection = new SqlConnection(ConnectionString);
            await _connection.OpenAsync();
            _respawner = await Respawner.CreateAsync(_connection, new RespawnerOptions
            {
                DbAdapter = DbAdapter.SqlServer,
                TablesToIgnore = new Respawn.Graph.Table[] { "__EFMigrationsHistory" }
            });
        }

        public async Task ResetDatabaseAsync() => await _respawner.ResetAsync(_connection);

        public new async Task DisposeAsync() => await _connection.DisposeAsync();
    }
}
