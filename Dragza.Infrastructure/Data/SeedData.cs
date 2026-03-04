using Dragza.Domain.Models;
using Dragza.Application.Data;
using Google;
using Microsoft.EntityFrameworkCore;

namespace Dragza.Infrastructure.Data
{
    public static class SeedData
    {
        public static async Task SeedRolesAsync(DragzaContext context)
        {
            if (context.Roles == null)
                return;

            var roles = new[] { "Admin", "Seller", "User" };

            foreach (var roleName in roles)
            {
                if (!await context.Roles.AnyAsync(r => r.Name == roleName))
                {
                    context.Roles.Add(new Role
                    {
                        Id = Guid.NewGuid(),
                        Name = roleName
                    });
                }
            }

            await context.SaveChangesAsync();
        }
    }
}
