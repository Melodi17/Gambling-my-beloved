using Gambling_my_beloved.Models;
using Microsoft.AspNetCore.Identity;

namespace Gambling_my_beloved;

public static class WebApplicationExtensions
{
    public static async Task<WebApplication> CreateRolesAsync(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        RoleManager<IdentityRole> roleManager = (RoleManager<IdentityRole>)scope.ServiceProvider.GetService(typeof(RoleManager<IdentityRole>));
        List<string> roles = Roles.AllRoles;

        foreach (string role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new(role));
        }

        return app;
    }

    public static async Task<WebApplication> CreateMasterUserAsync(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        UserManager<ApplicationUser> userManager =
            (UserManager<ApplicationUser>)scope.ServiceProvider.GetService(typeof(UserManager<ApplicationUser>));
        
        // find user by id
        ApplicationUser user = await userManager.FindByIdAsync("d86d5ce1-98ea-43b5-a63c-baf0264bdbed");
        await userManager.AddToRoleAsync(user, Roles.Administrator);
        
        return app;
    }
}