using FrontToBack.DAL;
using FrontToBack.Helpers;
using FrontToBack.Models;
using FrontToBack.Services.Basket;
using FrontToBack.Services.Product;
using Microsoft.AspNetCore.Identity;

namespace FrontToBack
{
    public static class ServiceRegistration
    {
        public static void FrontToBackServiceRegistration(this IServiceCollection services)
        {
            services.AddControllersWithViews()
                .AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                );
            services.AddHttpContextAccessor();
            services.AddScoped<IBasketProductCount, BasketProductCount>();
            services.AddScoped<IProductGet, ProductGet>();
            services.AddIdentity<AppUser, IdentityRole>(options=>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;

                options.User.RequireUniqueEmail = true;

                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(20);
                options.Lockout.MaxFailedAccessAttempts = 3;

            }).AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders()
            .AddErrorDescriber<CustomIdentityErrorDescriber>();

        }
    }
}
