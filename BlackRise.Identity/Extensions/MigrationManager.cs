using BlackRise.Identity.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BlackRise.Identity.Extensions;

public static class MigrationManager
{
    public static WebApplication MigrateDatabase(this WebApplication app)
    {
        using(var scope = app.Services.CreateScope())
        {
            using(var context = scope.ServiceProvider.GetRequiredService<IdentityDbContext>())
            {
                context.Database.Migrate();
            }
        }

        return app;
    }
}
