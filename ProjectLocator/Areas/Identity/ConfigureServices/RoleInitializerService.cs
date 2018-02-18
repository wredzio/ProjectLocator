using Microsoft.AspNetCore.Identity;
using ProjectLocator.Areas.Identity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectLocator.Areas.Identity.ConfigureServices
{
    public static class RoleInitializerService
    {
        public static async Task SeedRoles(this RoleManager<ApplicationRole> roleManager)
        {
            foreach (var role in Enum.GetNames(typeof(Roles)))
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new ApplicationRole(role));
                }
            }
        }
    }
}
