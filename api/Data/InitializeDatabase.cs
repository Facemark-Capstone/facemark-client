// David Wahid
using System;
using Microsoft.AspNetCore.Identity;
using shared.Models;

namespace api.Data
{
    public static class InitializeDatabase
    {
        public static void Seed(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            SeedRoles(roleManager);
            //SeedUsers(userManager);
        }

        private static void SeedUser(UserManager<User> userManager, User user, string password, string role)
        {
            try
            {
                if (userManager.FindByEmailAsync(user.Email).Result == null)
                {
                    var result = userManager.CreateAsync(user, password).Result;
                    if (result.Succeeded)
                    {
                        System.Console.WriteLine($"User created with email {user.Email} successfully...");

                        userManager.AddToRoleAsync(user, role).Wait();

                    }
                    else
                        System.Console.WriteLine($"User NOT created with email {user.Email} ...");
                }
                else
                {
                    System.Console.WriteLine($"User NOT created with email {user.Email}. Duplicate email ...");
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Error Seeding users ..." + e.Message);
            }

        }

        private static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            var roles = Enum.GetValues(typeof(EUserRole));

            foreach (var role in roles)
            {
                EUserRole userRole = (EUserRole)role;
                try
                {
                    if (!roleManager.RoleExistsAsync(userRole.ToString()).Result)
                    {
                        IdentityRole newRole = new IdentityRole() { Name = userRole.ToString() };
                        var result = roleManager.CreateAsync(newRole).Result;

                        if (result.Succeeded)
                            Console.WriteLine($"Role {role} successfully seeded to DB...");
                        else
                            Console.WriteLine($"Role {role} could NOT be seeded to DB...");

                    }
                    else
                        Console.WriteLine($"Role {role} exists already, so not seeded to DB...");

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
